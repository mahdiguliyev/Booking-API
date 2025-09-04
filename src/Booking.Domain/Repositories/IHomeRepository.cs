using Booking.Domain.Entities;

namespace Booking.Domain.Repositories;

public interface IHomeRepository
{
    ValueTask<IReadOnlyList<Home>> GetActiveHomesAsync(CancellationToken ct = default);
}