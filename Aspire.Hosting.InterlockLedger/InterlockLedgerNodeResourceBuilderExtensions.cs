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

using Aspire.Hosting.ApplicationModel;

using static Aspire.Hosting.StringHelpers;

namespace Aspire.Hosting;

/// <summary>
/// Provides extension methods for <see cref="IDistributedApplicationBuilder"/> to add IL2 Node resources to the application.
/// </summary>
public static class InterlockLedgerNodeResourceBuilderExtensions
{
    /// <summary>
    /// Adds an IL2 Node resource to the application.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="restPort">Optional port to expose for REST endpoint [defaults to 32032]</param>
    /// <param name="restPort">Optional port to expose for P2P endpoint [defaults to 32033]</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/> for chaining.</returns>
    public static IResourceBuilder<InterlockLedgerNodeResource> AddInterlockLedgerNode(this IDistributedApplicationBuilder builder,
                                                                                       [ResourceName] string name,
                                                                                       string dataFolderPath,
                                                                                       string ownerName,
                                                                                       string clientCertificatePassword,
                                                                                       string nodeCertificatePassword,
                                                                                       string emergencyKeyPassword,
                                                                                       string managerKeyPassword,
                                                                                       string ownerKeyPassword,
                                                                                       string? network = null,
                                                                                       int? restPort = null,
                                                                                       int? p2pPort = null) {
        var resource = new InterlockLedgerNodeResource(name, dataFolderPath, ownerName, clientCertificatePassword, network);
        return builder.AddResource(resource)
                      .WithImage("il2-slim-node")
                      .WithHttpsEndpoint(targetPort: 32032, port: restPort ?? 32032, name: InterlockLedgerNodeResource.RestEndpointName, isProxied: false)
                      .WithEndpoint(targetPort: 32033, port: p2pPort ?? 32033, name: InterlockLedgerNodeResource.P2pEndpointName, scheme: $"ilkl-{resource.Network}", isProxied: false, isExternal: true)
                      .WithBindMount(dataFolderPath, "/app/data")
                      .WithEnvironment("INTERLOCKLEDGER_PUBLIC_NODE_ADDRESS", NonBlank(name))
                      .WithEnvironment("INTERLOCKLEDGER_CLIENTNAME", ownerName)
                      .WithEnvironment("INTERLOCKLEDGER_CERTIFICATE_PASSWORD", NonBlank(nodeCertificatePassword))
                      .WithEnvironment("IL2MAKE_PASSWORD_CERT", clientCertificatePassword)
                      .WithEnvironment("IL2MAKE_PASSWORD_EMERGENCYKEY", NonBlank(emergencyKeyPassword))
                      .WithEnvironment("IL2MAKE_PASSWORD_MANAGER", NonBlank(managerKeyPassword))
                      .WithEnvironment("IL2MAKE_PASSWORD_OWNER", NonBlank(ownerKeyPassword))
               ;

    }


    /// <summary>
    /// Injects a connection string as an environment variable from the source resource into the destination resource, using the source resource's name as the connection string name.
    /// The format of the environment variable will be "ConnectionStrings__{sourceResourceName}={connectionString}".
    /// Also injects an environment variable as "INTERLOCKLEDGER_NODE={sourceResourceName}" for the client to know which connection string to look after.
    /// <para>
    /// Each resource defines the format of the connection string value. The
    /// underlying connection string value can be retrieved using <see cref="IResourceWithConnectionString.GetConnectionStringAsync(CancellationToken)"/>.
    /// </para>
    /// <para>
    /// Connection strings are also resolved by the configuration system (appSettings.json in the AppHost project, or environment variables). If a connection string is not found on the resource, the configuration system will be queried for a connection string
    /// using the resource's name.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The destination resource.</typeparam>
    /// <param name="builder">The resource where connection string will be injected.</param>
    /// <param name="node">The InterlockLedgerNode resource from which to extract the connection string.</param>
    /// <exception cref="DistributedApplicationException">Throws an exception if the connection string resolves to null. It can be null if the resource has no connection string, and if the configuration has no connection string for the source resource.</exception>
    /// <returns>A reference to the <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<T> WithInterlockLedgerNodeReference<T>(this IResourceBuilder<T> builder,
                                                                          IResourceBuilder<InterlockLedgerNodeResource> node) where T : IResourceWithEnvironment =>
        builder
            .WithReference(node)
            .WithEnvironment("INTERLOCKLEDGER_NODE", node.Resource.Name);

}
