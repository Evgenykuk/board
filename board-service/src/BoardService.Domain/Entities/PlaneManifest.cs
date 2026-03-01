using System;

namespace BoardService.Domain.Entities;

public class PlaneManifest
{
    public string PlaneId { get; set; } = null!;
    public string PassengerId { get; set; } = null!;
    public DateTime BoardedAt { get; set; } = DateTime.UtcNow;
}
