using Booking.Domain.Entities;
using Booking.Domain.Repositories;

namespace Booking.Infrastructure.InMemory;

public sealed class InMemoryHomeRepository : IHomeRepository
{
    private readonly IReadOnlyList<Home> _homes;

    public InMemoryHomeRepository()
    {
        _homes = Seed();
    }

    public ValueTask<IReadOnlyList<Home>> GetActiveHomesAsync(CancellationToken ct = default) => ValueTask.FromResult(_homes);

    private static IReadOnlyList<Home> Seed()
    {
        var homes = new List<Home>
        {
            new Home("123", "Home 1", new [] { new DateOnly(2025, 7, 15), new DateOnly(2025, 7, 16), new DateOnly(2025, 7, 17) }),
            new Home("456", "Home 2", new [] { new DateOnly(2025, 7, 17), new DateOnly(2025, 7, 18), new DateOnly(2025, 7, 19) }),
            new Home("789", "Home 3", new [] { new DateOnly(2025, 7, 15), new DateOnly(2025, 7, 17) }),
            new Home("321", "Home 4", Enumerable.Range(0, 31).Select(i => new DateOnly(2025, 7, 1).AddDays(i)))
        };

        return homes;
    }
}