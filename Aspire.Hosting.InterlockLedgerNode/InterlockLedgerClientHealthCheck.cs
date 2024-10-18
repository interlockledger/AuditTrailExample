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

using InterlockLedger.Rest.Client.V14_2_2;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Microsoft.Extensions.Hosting;

public class InterlockLedgerClientHealthCheck(RestNodeV14_2_2 restNode) : IHealthCheck
{
    private readonly RestNodeV14_2_2 _restNode = restNode.Required();

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) =>
        CheckAsync(cancellationToken);
    public HealthCheckResult Healthy() => CheckAsync(CancellationToken.None).WaitResult();

    private async Task<HealthCheckResult> CheckAsync(CancellationToken cancellationToken) {
        try {
            var checkTask = _restNode.GetDetailsAsync();
            await checkTask.WaitAsync(cancellationToken).ConfigureAwait(false);
            var details = checkTask.Result.Required();
            return HealthCheckResult.Healthy($"Node {details.Name} [{details.Id}]");
        } catch (Exception ex) {
            return HealthCheckResult.Unhealthy("Can't access IL2 Node", ex);
        }
    }
}

