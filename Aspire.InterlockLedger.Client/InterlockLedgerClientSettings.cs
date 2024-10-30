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

using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.Hosting;

public sealed class InterlockLedgerClientSettings
{
    internal const string DefaultConfigSectionName = "Aspire:InterlockLedger:Client";

    /// <summary>
    /// Gets or sets the comma-delimited configuration string used to connect to the InterlockLedger node.
    /// </summary>
    public InterlockLedger.Rest.Client.ConnectionString? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets a boolean value that indicates whether the InterlockLedger health check is disabled or not.
    /// </summary>
    /// <value>
    /// The default value is <see langword="false"/>.
    /// </value>
    public bool DisableHealthChecks { get; set; }
}

internal static class IConfigurationExtensions
{
    public static InterlockLedgerClientSettings GetInterlockLedgerClientSettings(this IConfiguration configuration) {
        var settings = new InterlockLedgerClientSettings();
        configuration
               .GetSection(InterlockLedgerClientSettings.DefaultConfigSectionName)
               .Bind(settings);
        return settings;
    }
}
