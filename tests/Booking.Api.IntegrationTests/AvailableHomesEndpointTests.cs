using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Booking.Application.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Booking.Api.IntegrationTests;

public class AvailableHomesEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AvailableHomesEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(_ => {});
    }

    [Fact]
    public async Task Returns_Homes_That_Cover_All_Range()
    {
        var client = _factory.CreateClient();

        var resp = await client.GetAsync("/api/available-homes?startDate=2025-07-15&endDate=2025-07-16");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await resp.Content.ReadAsStringAsync();
        content.Should().Contain(@"""status"":""OK""");

        var parsed = JsonSerializer.Deserialize<AvailableHomesResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        parsed.Should().NotBeNull();
        parsed!.Homes.Should().NotBeNull();

        // Home 1 has 15th and 16th; Home 4 covers the whole month

        parsed.Homes.Select(h => h.HomeId).Should().Contain(new[] { "123", "321" });
        parsed.Homes.Should().OnlyContain(h => h.AvailableSlots.Contains("2025-07-15") && h.AvailableSlots.Contains("2025-07-16"));
    }

    [Fact]
    public async Task Excludes_Homes_With_Missing_Dates()
    {
        var client = _factory.CreateClient();

        var resp = await client.GetAsync("/api/available-homes?startDate=2025-07-15&endDate=2025-07-17");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var parsed = await resp.Content.ReadFromJsonAsync<AvailableHomesResponse>();
        parsed.Should().NotBeNull();

        // Home 789 has 15 and 17 only, so should not be included

        parsed!.Homes.Select(h => h.HomeId).Should().NotContain("789");
    }

    [Fact]
    public async Task Returns_400_For_Invalid_Input()
    {
        var client = _factory.CreateClient();

        var resp = await client.GetAsync("/api/available-homes?startDate=2025-07-20&endDate=2025-07-10");
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var resp2 = await client.GetAsync("/api/available-homes?startDate=20250710&endDate=2025-07-20");
        resp2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}