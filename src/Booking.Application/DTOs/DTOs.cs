namespace Booking.Application.DTOs;

public sealed record AvailableHomeDto(string HomeId, string HomeName, IReadOnlyList<string> AvailableSlots);

public sealed record AvailableHomesResponse(string Status, IReadOnlyList<AvailableHomeDto> Homes);