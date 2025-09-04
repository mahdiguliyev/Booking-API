# Booking API — Technical Task

Booking API project that returns homes available for a given date range.  
All data is in-memory; no database required.

## Architecture

- **Booking.Api** — Minimal API endpoint that validates input and delegates to the service. Returns the required JSON shape.
- **Booking.Application** — DTOs and the `AvailabilityService` implementing filtering, fully unit-testable.
- **Booking.Domain** — core entities and interfaces (`Home`, `IHomeRepository`).
- **Booking.Infrastructure** — `InMemoryHomeRepository` with seeded data; replaceable with any other source.
- **Booking.Api.IntegrationTests** - xUnit tests using `WebApplicationFactory`

## Requirements

- Date Range Filtering: The user will provide a date range, and the API will return a list of homes that are available during that range.
- Home and Slot Data: Each home has its availability slots (dates) that are unique to that home.
- API Endpoint: The user will send a date range, and the server will return a list of homes available during that range.
- Performance: The system should be optimized to handle a large number of homes and dates efficiently.


## Filtering logic

1. **Early check for available slots**

	- If given date (or dates) is not in the home’s available slots, then exclude (`return null`).
	- This prevents unnecessary checks for the rest of the data.

2. **Homes availability**

	- A home is returned only if **all requested dates** are available.
	- If one day is unavailable, then home is excluded.

3. **Parallel processing**

	- Parallel filtering via **PLINQ** for large datasets for speed on large datasets.
	- `.AsParallel().WithCancellation(ct)` distributes the work across CPU cores.
	- Each home is checked independently.

4. **Output formating**

	- Dates are formatted to `yyyy-MM-dd` string once included.
	- Immutable `AvailableHomeDto` ensures clean contract to API.
	
	
## Performance and Optimization

1. **Lookup complexity**

	- `h.AvailableSlots.Contains(d)` is `O(1)` (if AvailableSlots is a `HashSet<DateOnly>`).
	- If it’s a `List<DateOnly>`, each .Contains is `O(n)`, which can degrade. Best to back it with a `HashSet<DateOnly>`.
	
	Code line: `new HashSet<DateOnly>(availableSlots).ToHashSet();`

2. **Short-circuiting**

	Prevent additional checking if one date is missing.

3. **Parallelism**

	Homes checked concurrently (near-linear scaling on multi-core systems).

4. **Memory usage**

	`requiredDates` is computed once, reused for all homes. The only allocations are per-home `slots` array.
	

## Run & test (quick start)

Requirements: **.NET 8 SDK**

```bash
dotnet restore
dotnet build
dotnet run --project src/Booking.Api/Booking.Api.csproj
```

Browse Swagger UI:
```
http://localhost:5000/swagger
```

Or call the endpoint directly:
```
GET http://localhost:5000/api/available-homes?startDate=2025-07-15&endDate=2025-07-16
```

## Example Response
```
{
  "status": "OK",
  "homes": [
    {
      "homeId": "123",
      "homeName": "Home 1",
      "availableSlots": [
        "2025-07-15",
        "2025-07-16"
      ]
    },
    {
      "homeId": "321",
      "homeName": "Home 4",
      "availableSlots": [
        "2025-07-15",
        "2025-07-16"
      ]
    }
  ]
}
```

## Testing
Run integration tests:

```bash
dotnet test
```