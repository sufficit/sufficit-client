# ReceitaNet Compatibility Anonymous Paths

## Summary

The ReceitaNet compatibility client section now marks its compatibility endpoints as anonymous-capable.

## Changed Paths

- `/gateway/receitanetcompatibility/test`
- `/gateway/receitanetcompatibility/testall`
- `/gateway/receitanetcompatibility/validatetoken`

## Reason

The Blazor compatibility page can now be shared with external users through a prefilled link. The SDK layer must therefore allow requests to these routes even when no access token is available in the current client session.

## Scope

- Only the compatibility test routes were marked as anonymous-capable.
- The rest of the ReceitaNet client surface remains authenticated by default.

## Validation

- IDE diagnostics for `ReceitaNetCompatibilityControllerSection.cs` reported no errors.
- `dotnet build src/Sufficit.Client.csproj --configuration Debug` completed successfully.