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
using static Aspire.Hosting.StringHelpers;

namespace Aspire.Hosting.ApplicationModel;

public sealed class InterlockLedgerNodeResource : ContainerResource, IResourceWithConnectionString
{
    // Constants used to refer to well known-endpoint names, this is specific
    // for each resource type. IL2Node exposes a REST endpoint and a P2P
    // endpoint.
    internal const string RestEndpointName = "rest";
    internal const string P2pEndpointName = "p2p";

    // An EndpointReference is a core .NET Aspire type used for keeping
    // track of endpoint details in expressions. Simple literal values cannot
    // be used because endpoints are not known until containers are launched.
    private EndpointReference? _restReference;

    public EndpointReference RestEndpoint => _restReference ??= new(this, RestEndpointName);

    // An EndpointReference is a core .NET Aspire type used for keeping
    // track of endpoint details in expressions. Simple literal values cannot
    // be used because endpoints are not known until containers are launched.
    private EndpointReference? _p2pReference;
    public EndpointReference P2pEndpoint => _p2pReference ??= new(this, P2pEndpointName);


    public InterlockLedgerNodeResource(string name, string dataFolderPath, string ownerName, string clientCertificatePassword, string? network) : base(name) {
        Network = network.WithDefault("MainNet").ToLowerInvariant();
        Owner = NonBlank(ownerName).ToLowerInvariant();
        ClientCertificatePassword = NonBlank(clientCertificatePassword);
        ClientCertificateFilePath = Path.Combine(FolderMustExist(dataFolderPath), "il2-node", "certificates", $"{Network}.{Owner}.rest.api.pfx");
    }

    internal string? ClientCertificatePassword { get; }
    internal string? ClientCertificateFilePath { get; }
    internal string Network { get; }
    internal string Owner { get; }

    // Required property on IResourceWithConnectionString. Represents a connection
    // string that applications can use to access the IL2Node server. In this case
    // the connection string is composed of the RestEndpoint endpoint reference.
    public ReferenceExpression ConnectionStringExpression =>
        ReferenceExpression.Create($"https://{RestEndpoint.Property(EndpointProperty.Host)}:{RestEndpoint.Property(EndpointProperty.Port)},{ClientCertificateFilePath},{ClientCertificatePassword},{Network}");
}
