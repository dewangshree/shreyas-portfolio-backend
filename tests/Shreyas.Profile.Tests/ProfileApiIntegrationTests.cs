using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Shreyas.Profile.Tests;

public sealed class ProfileApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProfileApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Development")).CreateClient();
    }

    [Fact]
    public async Task Health_returns_ok()
    {
        var response = await _client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Quote_of_the_day_returns_a_valid_curated_response()
    {
        var response = await _client.GetAsync("/api/quotes/today");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var quote = document.RootElement;

        quote.GetProperty("text").GetString().Should().NotBeNullOrWhiteSpace();
        quote.GetProperty("author").GetString().Should().NotBeNullOrWhiteSpace();
        quote.GetProperty("date").GetDateTime().Should().NotBe(default);
        quote.GetProperty("source").GetString().Should().Be("curated");
    }

    [Fact]
    public async Task Unknown_api_route_returns_not_found()
    {
        var response = await _client.GetAsync("/api/unknown");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
