using Booking.Application.DTOs;
using Booking.Application.Services.Abstract;
using Booking.Application.Services.Concrete;
using Booking.Domain.Repositories;
using Booking.Infrastructure.InMemory;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IHomeRepository, InMemoryHomeRepository>();
builder.Services.AddSingleton<IAvailabilityService, AvailabilityService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/available-homes", async (string? startDate, string? endDate, IAvailabilityService service, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(startDate) || string.IsNullOrWhiteSpace(endDate))
        return Results.BadRequest(new { error = "startDate and endDate are required in yyyy-MM-dd format." });

    if (!DateOnly.TryParseExact(startDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var start))
        return Results.BadRequest(new { error = "Invalid startDate format. Use yyyy-MM-dd." });
    if (!DateOnly.TryParseExact(endDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var end))
        return Results.BadRequest(new { error = "Invalid endDate format. Use yyyy-MM-dd." });
    if (end < start)
        return Results.BadRequest(new { error = "endDate must be on or after startDate." });

    var response = await service.GetAvailableHomesAsync(start, end, ct);
    return Results.Ok(response);
})
.WithName("GetAvailableHomes")
.Produces<AvailableHomesResponse>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.WithOpenApi();

app.Run();

public partial class Program { }