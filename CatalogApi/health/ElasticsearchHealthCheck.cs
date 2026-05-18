using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CatalogApi.health;

public class ElasticsearchHealthCheck(ElasticsearchClient client) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var response = await client.Cluster.HealthAsync(cancellationToken: cancellationToken);

        return response.IsSuccess()
            ? HealthCheckResult.Healthy($"status: {response.Status}")
            : HealthCheckResult.Unhealthy("Elasticsearch não responde");
    }
}