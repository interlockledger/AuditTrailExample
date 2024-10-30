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

using Aspire.InterlockLedger.Client;

using InterlockLedger.Rest.Client;
using InterlockLedger.Rest.Client.V14_2_2;

namespace Microsoft.Extensions.Hosting;

public class NodeApiClient
{
    private readonly RestNodeV14_2_2 _restNode;
    private readonly Lazy<IEnumerable<RestChainV14_2_2>> _chains;
    private readonly Lazy<IEnumerable<RestChainV14_2_2>> _writableJsonStoreChains;
    private readonly Lazy<NodeDetailsModel?> _nodeDetails;

    public NodeApiClient(RestNodeV14_2_2 restNode) {
        _restNode = restNode.Required();
        _nodeDetails = new(() => _restNode.GetDetailsAsync().Result);
        _chains = new(() => _restNode.GetChainsAsync().Result);
        _writableJsonStoreChains = new(() => _chains.Value.Where(static c => c.JsonStore.IsWritable));
    }

    public string Network => NodeDetails?.Network ?? "?";
    public NodeDetailsModel? NodeDetails => _nodeDetails.Value;
    public IEnumerable<RestChainV14_2_2> WritableJsonStoreChains => _writableJsonStoreChains.Value;
    public IJsonStore GetWritableJsonStore(string? specificChain) =>
        WritableJsonStoreChains.FirstOrDefault(c => specificChain.IsBlank() || c.Id == specificChain)?.JsonStore ?? NullJsonStore.Instance;

    public IJsonStore GetReadableJsonStore(UniversalRecordReference reference) {
        if (Network.Equals(reference.Network, StringComparison.OrdinalIgnoreCase)) {
            var store = _chains.Value.FirstOrDefault(c => c.Id == reference.ChainId)?.JsonStore;
            if (store is not null)
                return store;
        }
        return NullJsonStore.Instance;
    }
}



