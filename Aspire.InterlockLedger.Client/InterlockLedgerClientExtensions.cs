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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Microsoft.Extensions.Hosting;

public static class InterlockLedgerClientExtensions
{
    private const string _defaultConfigSectionName = "Aspire:InterlockLedger:Client";

    public static IServiceCollection AddInterlockLedgerClient(this IServiceCollection services, string host, string certificatePath, string certificatePassword, ushort? port = null,
        Action<InterlockLedgerClientSettings>? configureSettings = null,
        Action<ConfigurationOptions>? configureOptions = null) {
        services.AddSingleton(new RestNodeV14_2_2(new X509Certificate2(certificatePath.Required(), certificatePassword.Required()), port ?? 32032, host).Required());
        return services;
    }

    public static IServiceCollection AddInterlockLedgerClientHealthChecks(this IServiceCollection services) {
        services.AddSingleton<InterlockLedgerClientHealthCheck>();
        services.AddHealthChecks().AddCheck<InterlockLedgerClientHealthCheck>("node", HealthStatus.Unhealthy, ["node"], TimeSpan.FromSeconds(5));
        return services;
    }

    public static WebApplication MapInterlockLedgerClientDefaultEndpoints(this WebApplication app) {
        if (app.Environment.IsDevelopment()) {
            app.MapHealthChecks("/healthOfNode", new HealthCheckOptions {
                Predicate = r => r.Tags.Contains("node")
            });
        }
        return app;
    }
}

