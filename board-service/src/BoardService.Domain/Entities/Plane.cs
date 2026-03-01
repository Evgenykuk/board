using System;

namespace BoardService.Domain.Entities;

public class Plane
{
    public string PlaneId { get; set; } = null!;
    public string? FlightId { get; set; }
    public int Capacity { get; set; }
    public int FuelLevel { get; set; }
    public int FuelRequired { get; set; }
    public string State { get; set; } = null!;
    public string LocationNode { get; set; } = null!;
    public Guid? RouteId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
