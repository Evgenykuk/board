using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BoardService.Domain.Entities;
using BoardService.Infrastructure.Data;
using System.ComponentModel.DataAnnotations;

namespace BoardService.Api.Controllers;

[ApiController]
[Route("v1/planes")]
[Produces("application/json")]
public class PlanesController : ControllerBase
{
    private readonly BoardDbContext _context;

    public PlanesController(BoardDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Список бортов с фильтрацией
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<Plane>>> GetPlanes(
        [FromQuery] string? state,
        [FromQuery] string? flightId)
    {
        var query = _context.Planes.AsQueryable();

        if (!string.IsNullOrEmpty(state))
            query = query.Where(p => p.State == state);

        if (!string.IsNullOrEmpty(flightId))
            query = query.Where(p => p.FlightId == flightId);

        return await query.ToListAsync();
    }

    /// <summary>
    /// Получить борт по planeId
    /// </summary>
    [HttpGet("{planeId}")]
    public async Task<ActionResult<Plane>> GetPlane(string planeId)
    {
        var plane = await _context.Planes.FindAsync(planeId);
        if (plane == null)
            return NotFound(new { code = "not_found", message = "Plane not found" });

        return plane;
    }

    /// <summary>
    /// Получить борт по flightId
    /// </summary>
    [HttpGet("by-flight/{flightId}")]
    public async Task<ActionResult<Plane>> GetPlaneByFlight(string flightId)
    {
        var plane = await _context.Planes
            .FirstOrDefaultAsync(p => p.FlightId == flightId);

        if (plane == null)
            return NotFound(new { code = "not_found", message = "Plane not found for flight" });

        return plane;
    }

    /// <summary>
    /// Создать новый борт (идемпотентно)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Plane>> CreatePlane([FromBody] CreatePlaneRequest request)
    {
        if (await _context.Planes.AnyAsync(p => p.PlaneId == request.PlaneId))
        {
            var existing = await _context.Planes.FindAsync(request.PlaneId);
            return Ok(existing); // Идемпотентность
        }

        var plane = new Plane
        {
            PlaneId = request.PlaneId,
            FlightId = request.FlightId,
            Capacity = request.Capacity,
            FuelLevel = request.FuelLevel,
            FuelRequired = request.FuelRequired,
            State = "InHangar",
            LocationNode = request.StartNode,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Planes.Add(plane);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPlane), new { planeId = plane.PlaneId }, plane);
    }
}

public class CreatePlaneRequest
{
    [Required] public string PlaneId { get; set; } = null!;
    public string? FlightId { get; set; }
    [Required] public int Capacity { get; set; }
    [Required] public int FuelLevel { get; set; }
    [Required] public int FuelRequired { get; set; }
    [Required] public string StartNode { get; set; } = null!;
}
