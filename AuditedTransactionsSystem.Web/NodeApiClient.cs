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

using System.Text.Json;

namespace AuditedTransactionsSystem.Web;

public class NodeApiClient(HttpClient httpClient)
{
    public Task<NodeDetails?> NodeDetails(CancellationToken cancellationToken = default) => httpClient.GetFromJsonAsync<NodeDetails>("/node", cancellationToken);
}

public class NodeDetails
{
    public string? Color { get; set; }

    public required string Id { get; set; }

    public string? Name { get; set; }

    public required string Network { get; set; }

    public string? OwnerId { get; set; }

    public string? OwnerName { get; set; }

    public string? PeerAddress { get; set; }

    public IEnumerable<string>? Roles { get; set; }


    public string? ResolvedPeerAddress { get; set; }

    public Versions? SoftwareVersions { get; set; }

    public string? Extras { get; set; }

    public IEnumerable<string>? Chains { get; set; }

    public Dictionary<string, string>? Extensions { get; set; }

    private static readonly JsonSerializerOptions _options = new() { WriteIndented = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault };
    public override string ToString() => JsonSerializer.Serialize(this, _options);
}

public class Versions
{
    public string? CoreLibs { get; set; }

    public string? Tags { get; set; }

    public string? Node { get; set; }

    public string? Main { get; set; }

    public string NodeVersion => Main ?? Node ?? "?";

    public string? Peer2peer { get; set; }
}
