using Booking.Application.DTOs;

namespace Booking.Application.Services.Abstract;

public interface IAvailabilityService
{
    Task<AvailableHomesResponse> GetAvailableHomesAsync(DateOnly start, DateOnly end, CancellationToken ct = default);
}