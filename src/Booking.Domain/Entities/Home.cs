namespace Booking.Domain.Entities;

public sealed class Home
{
    public string HomeId { get; }
    public string HomeName { get; }
    // Immutable set for fast lookups
    public IReadOnlySet<DateOnly> AvailableSlots { get; }

    public Home(string homeId, string homeName, IEnumerable<DateOnly> availableSlots)
    {
        HomeId = homeId;
        HomeName = homeName;
        AvailableSlots = new HashSet<DateOnly>(availableSlots).ToHashSet();
    }
}