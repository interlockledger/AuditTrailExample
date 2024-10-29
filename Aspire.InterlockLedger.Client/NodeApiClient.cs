// ******************************************************************************************************************************
//
// Copyright (c) 2024 InterlockLedger
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// ******************************************************************************************************************************

using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;

using InterlockLedger.Rest.Client;
using InterlockLedger.Rest.Client.V14_2_2;

namespace Microsoft.Extensions.Hosting;

public class NodeApiClient(RestNodeV14_2_2 restNode)
{
    private const int _jsonStoreAppId = 8;
    private readonly RestNodeV14_2_2 _restNode = restNode.Required();

    public Task<NodeDetailsModel?> NodeDetails(CancellationToken cancellationToken = default) =>
        _restNode.GetDetailsAsync();

    public async Task<IEnumerable<string>> GetChainsThatCanStoreJsonDocumentsAsync() =>
        (await GetChainsThatCanStoreForAppAsync(_jsonStoreAppId).ConfigureAwait(false)).Select(static c => c.Id).ToArray();
    public async Task<string?> AddToAuditTrialAsync(string userName, string traceIdentifier, string action, string changes, string? specificChain = null) {
        var chain = await GetJsonStoreChain(specificChain).ConfigureAwait(false);
        if (chain is null)
            return null;
        var jsonDocument = await chain.JsonStore.AddAsync(new AuditTrailEntry(userName, traceIdentifier, action, changes)).ConfigureAwait(false);
        return jsonDocument?.Reference?.ToString();
    }

    private async Task<RestChainV14_2_2?> GetJsonStoreChain(string? specificChain) {
        var chains = await GetChainsThatCanStoreForAppAsync(_jsonStoreAppId).ConfigureAwait(false);
        return chains.FirstOrDefault(c => specificChain.IsBlank() || c.Id == specificChain);
    }

    public async Task<AuditTrailEntry?> GetAuditTrailEntryAsync(string reference) {
        string[] parts = reference.Split("@");
        var chain = await GetJsonStoreChain(parts[0]).ConfigureAwait(false);
        if (chain is null)
            return null;
        var jsonDocument = await chain.JsonStore.RetrieveAsync(ulong.Parse(parts[1])).ConfigureAwait(false);
        if (jsonDocument is null) return null;
        string? json = jsonDocument.EncryptedJson?.DecodedWith(_restNode.Certificate)?.Trim('\0');
        return json is null ? null : JsonSerializer.Deserialize<AuditTrailEntry?>(json, Globals.JsonSettings);
    }

    private async Task<IEnumerable<RestChainV14_2_2>> GetChainsThatCanStoreForAppAsync(ulong appId) {
        var validChains = new List<RestChainV14_2_2>();
        var chains = await _restNode.GetChainsAsync().ConfigureAwait(false);
        foreach (var chain in chains) {
            var summary = await chain.GetSummaryAsync().ConfigureAwait(false);
            if (summary is null || summary.IsClosedForNewTransactions || summary.LicenseIsExpired || summary.LicensedApps.None(a => a == appId))
                continue;
            var keys = await chain.GetPermittedKeysAsync().ConfigureAwait(false);
            if (summary.ActiveApps.Contains(appId) && keys is not null && keys.Any(k => MatchesClientCertificate(k.PublicKey) && k.Permissions.Any(ap => ap.AppId == appId))) {
                validChains.Add(chain);
            }
        }
        validChains.Sort((x, y) => string.Compare(x.Id, y.Id, StringComparison.OrdinalIgnoreCase));
        return validChains;
    }

    private bool MatchesClientCertificate(string publicKey) =>
        true || // TODO implement a working comparison
        publicKey == _restNode.Certificate.ToPubKeyHash();

}

internal static class Globals
{
    public static readonly JsonSerializerOptions JsonSettings = new() {
        AllowTrailingCommas = true,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReadCommentHandling = JsonCommentHandling.Skip,
        WriteIndented = true,
        PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace,
    };
}

