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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Microsoft.Extensions.Hosting;

public static partial class InterlockLedgerClientExtensions
{
    /// <summary>
    /// Adds a client to an InterlockLedger Node
    /// </summary>
    /// <param name="builder">Builder for host app</param>
    /// <param name="configureSettings">Action to change <see cref="InterlockLedgerClientSettings"/> read from configuration at "Aspire:InterlockLedger:Client" path</param>
    /// <returns>The builder with needed pieces. Inject on your code the provided <see cref="NodeApiClient"/></returns>
    public static IHostApplicationBuilder AddInterlockLedgerClient(this IHostApplicationBuilder builder,
                                                                   Action<InterlockLedgerClientSettings>? configureSettings = null) {
        string connectionName = builder.Configuration["INTERLOCKLEDGER_NODE"].WithDefault("il2-node");
        string? connectionString = builder.Configuration.GetConnectionString(connectionName);
        var settings = builder.Configuration.GetInterlockLedgerClientSettings();
        if (!connectionString.IsBlank())
            settings.ConnectionString ??= ConnectionString.Parse(connectionString);
        configureSettings?.Invoke(settings);

        var restClient = new RestNodeV14_2_2(settings.ConnectionString.Required());
        var nodeApiClient = new NodeApiClient(restClient);
        builder.Services.AddSingleton(restClient);
        builder.Services.AddSingleton(nodeApiClient);

        if (!settings.DisableHealthChecks)
            builder.Services.AddHealthChecks().AddCheck<InterlockLedgerClientHealthCheck>("node", HealthStatus.Unhealthy, ["node"], TimeSpan.FromSeconds(5));

        return builder;
    }
}

