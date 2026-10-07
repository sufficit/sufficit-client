using Sufficit.Access;
using Sufficit.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers;

/// <summary>Business recipes served by sufficit-endpoints, independent from the generic Identity app.</summary>
public sealed class EntitlementPresetsControllerSection : AuthenticatedControllerSection
{
    public const string Endpoint = "/access/entitlementpresets";
    private readonly JsonSerializerOptions json;
    public EntitlementPresetsControllerSection(IAuthenticatedControllerBase cb) : base(cb) { json = cb.Json; }

    public async Task<IReadOnlyList<EntitlementPresetProfile>> List(CancellationToken token = default)
    {
        var presets = (await RequestMany<EntitlementPresetProfile>(Message(HttpMethod.Get, Endpoint), token)).ToArray();
        if (presets.Any(x => x == null || string.IsNullOrWhiteSpace(x.Key) || string.IsNullOrWhiteSpace(x.Version)
            || x.Directives == null || x.Directives.Any(d => d == null || string.IsNullOrWhiteSpace(d.Key)))
            || presets.Select(x => x.Key).Distinct(StringComparer.OrdinalIgnoreCase).Count() != presets.Length)
            throw new InvalidOperationException("Invalid entitlement-preset catalog.");
        return presets;
    }
    public Task<EntitlementPresetProfile?> Get(string key, CancellationToken token = default)
        => Request<EntitlementPresetProfile>(Message(HttpMethod.Get, Path(key)), token);
    public Task<EntitlementPresetProfile?> Create(string key, SaveEntitlementPreset request, CancellationToken token = default)
        => Send(HttpMethod.Post, Path(key), request, token);
    public Task<EntitlementPresetProfile?> Update(string key, SaveEntitlementPreset request, CancellationToken token = default)
        => Send(HttpMethod.Put, Path(key), request, token);
    public async Task Delete(string key, string version, CancellationToken token = default)
        => await Request<object>(Message(HttpMethod.Delete, $"{Path(key)}?version={Uri.EscapeDataString(version)}"), token);
    public Task<EntitlementPresetProfile?> SaveItem(string key, string directiveKey, SaveEntitlementPresetItem request,
        CancellationToken token = default) => Send(HttpMethod.Put, $"{Path(key)}/items/{Uri.EscapeDataString(directiveKey)}", request, token);
    public Task<EntitlementPresetProfile?> DeleteItem(string key, string directiveKey, string version, CancellationToken token = default)
        => Request<EntitlementPresetProfile>(Message(HttpMethod.Delete,
            $"{Path(key)}/items/{Uri.EscapeDataString(directiveKey)}?version={Uri.EscapeDataString(version)}"), token);
    private Task<EntitlementPresetProfile?> Send<T>(HttpMethod method, string path, T payload, CancellationToken token)
    {
        var message = Message(method, path); message.Content = JsonContent.Create(payload, null, json);
        return Request<EntitlementPresetProfile>(message, token);
    }
    private static HttpRequestMessage Message(HttpMethod method, string path) => new HttpRequestMessage(method, new Uri(path, UriKind.Relative));
    private static string Path(string key) => $"{Endpoint}/{Uri.EscapeDataString(key)}";
}
