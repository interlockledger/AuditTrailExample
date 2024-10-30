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

using System.Security.Cryptography.X509Certificates;

using InterlockLedger.Rest.Client;
using InterlockLedger.Rest.Client.V14_2_2;

namespace Aspire.InterlockLedger.Client;

internal class NullJsonStore : IJsonStore
{
    public static readonly NullJsonStore Instance = new();
    public bool IsWritable => false;
    public string NetworkName { get; } = "?";
    public string ChainId { get; } = "?";

    public Task<AllowedReadersRecordModel?> AddAllowedReaders(AllowedReadersModel allowedReaders) =>
        Task.FromResult<AllowedReadersRecordModel?>(null);
    public Task<JsonDocumentModel?> Add<T>(T jsonDocument) =>
        Task.FromResult<JsonDocumentModel?>(null);
    public Task<JsonDocumentModel?> Add<T>(T jsonDocument, PublicKey readerKey, string readerKeyId) =>
        Task.FromResult<JsonDocumentModel?>(null);
    public Task<JsonDocumentModel?> Add<T>(T jsonDocument, RecordReference[] allowedReadersReferences) =>
        Task.FromResult<JsonDocumentModel?>(null);
    public Task<JsonDocumentModel?> Add<T>(T jsonDocument, string[] idOfChainsWithAllowedReaders) =>
        Task.FromResult<JsonDocumentModel?>(null);
    public Task<PageOfAllowedReadersRecordModel?> RetrieveAllowedReaders(string chain, string? contextId = null, bool lastToFirst = false, int page = 0, int pageSize = 10) =>
        Task.FromResult<PageOfAllowedReadersRecordModel?>(null);
    public Task<JsonDocumentModel?> Retrieve(ulong serial) =>
        Task.FromResult<JsonDocumentModel?>(null);
    public Task<T?> RetrieveDecodedAs<T>(ulong serial, X509Certificate2? certificate = null) where T : class =>
        Task.FromResult<T?>(null);
}
