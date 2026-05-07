# Call Search Query Serialization Fix

## Summary

The telephony call search query serializer in `ToQueryStringExtensions` was not including several fields from `ICallSearchParameters`, including `Filter`.

As a result, UI flows that set a call search text filter appeared to apply the filter locally, but the API request did not carry `filter` in the query string. The backend therefore returned unfiltered results (commonly the default limit set).

## Root Cause

In `src/ToQueryStringExtensions.cs`, method:

- `ToQueryString(this ICallSearchParameters source)`

Only a subset of properties was serialized:

- `contextid`, `start`, `end`, `region`, `dids`, `billed`, `answered`, `limit`, `maxrecords`

Missing serialized properties:

- `protocol`
- `uniqueid`
- `filter`
- `tags`
- `timeout`

## Change Implemented

Extended `ToQueryString(this ICallSearchParameters source)` to include:

- `protocol` when non-empty
- `uniqueid` when non-empty
- `filter` when non-empty
- `tags` when present (`string.Join(",", source.Tags)`)
- `timeout` when positive (`TimeSpan` format `c`)

## Impact

- Telephony call search now sends `filter` to the API as expected.
- AI-assisted and manual call filtering flows now affect backend search results.
- Serialization coverage now matches the full `ICallSearchParameters` contract.
