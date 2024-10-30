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
using InterlockLedger.Rest.Client;
using InterlockLedger.Rest.Client.V14_2_2;

namespace AuditedTransactionsSystem.Web;

public class AuditTrailClient(NodeApiClient apiClient)
{
    public async Task<JsonDocumentModel?> AddToAuditTrialAsync(string userName, string traceIdentifier, string action, string changes) =>
        await apiClient.GetWritableJsonStore(null).Add(new AuditTrailEntry(userName, traceIdentifier, action, changes)).ConfigureAwait(false);

    public async Task<AuditTrailEntry?> GetAuditTrailEntryAsync(UniversalRecordReference reference) =>
        await apiClient.GetReadableJsonStore(reference).RetrieveDecodedAs<AuditTrailEntry>(reference.Serial).ConfigureAwait(false);
}
