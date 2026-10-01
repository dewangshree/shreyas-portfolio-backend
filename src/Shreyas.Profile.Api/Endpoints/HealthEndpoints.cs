using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Shreyas.Profile.Api.Endpoints;

public static class HealthEndpoints
{
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/health", async (HealthCheckService healthChecks, CancellationToken cancellationToken) =>
        {
            var report = await healthChecks.CheckHealthAsync(cancellationToken);

            return report.Status == HealthStatus.Healthy
                ? Results.Text("Healthy")
                : Results.Text("Unhealthy", statusCode: StatusCodes.Status503ServiceUnavailable);
        })
            .WithName("GetHealth")
            .WithSummary("Checks application health")
            .WithDescription("Returns HTTP 200 when the application is healthy.")
            .Produces(StatusCodes.Status200OK, contentType: "text/plain")
            .Produces(StatusCodes.Status503ServiceUnavailable, contentType: "text/plain")
            .AllowAnonymous();
        return endpoints;
    }
}
