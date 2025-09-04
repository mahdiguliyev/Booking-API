using Booking.Application.DTOs;
using Booking.Application.Services.Abstract;
using Booking.Application.Utils;
using Booking.Domain.Repositories;

namespace Booking.Application.Services.Concrete;

public sealed class AvailabilityService(IHomeRepository repository) : IAvailabilityService
{
    private readonly IHomeRepository _repository = repository;

    public async Task<AvailableHomesResponse> GetAvailableHomesAsync(DateOnly start, DateOnly end, CancellationToken ct = default)
    {
        var requiredDates = DateRange.ClosedRange(start, end).ToArray();
        if (requiredDates.Length == 0)
            return new AvailableHomesResponse("OK", Array.Empty<AvailableHomeDto>());

        var homes = await _repository.GetActiveHomesAsync(ct);

        // Parallel filtering for speed on large datasets
        var results = homes
            .AsParallel()
            .WithCancellation(ct)
            .Select(h =>
            {
                // if date is not in the home available slots, then exclude (avoid unnecessary checks)
                foreach (var date in requiredDates)
                {
                    if (!h.AvailableSlots.Contains(date))
                        return null;
                }

                var slots = requiredDates.Select(d => d.ToString("yyyy-MM-dd")).ToArray();
                return new AvailableHomeDto(h.HomeId, h.HomeName, slots);
            })
            .Where(dto => dto is not null)
            .Cast<AvailableHomeDto>()
            .ToArray();

        return new AvailableHomesResponse("OK", results);
    }
}