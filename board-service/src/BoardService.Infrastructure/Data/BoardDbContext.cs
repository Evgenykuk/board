using BoardService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BoardService.Infrastructure.Data;

public class BoardDbContext : DbContext
{
    public BoardDbContext(DbContextOptions<BoardDbContext> options) : base(options) { }

    public DbSet<Plane> Planes => Set<Plane>();
    public DbSet<PlaneChecklist> PlaneChecklists => Set<PlaneChecklist>();
    public DbSet<PlaneManifest> PlaneManifests => Set<PlaneManifest>();
    public DbSet<ProcessedEvent> ProcessedEvents => Set<ProcessedEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Plane>(entity =>
        {
            entity.HasKey(e => e.PlaneId);
            entity.Property(e => e.PlaneId).HasColumnName("plane_id");
            entity.Property(e => e.FlightId).HasColumnName("flight_id");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.FuelLevel).HasColumnName("fuel_level");
            entity.Property(e => e.FuelRequired).HasColumnName("fuel_required");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.LocationNode).HasColumnName("location_node");
            entity.Property(e => e.RouteId).HasColumnName("route_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.ToTable("planes");
        });

        modelBuilder.Entity<PlaneChecklist>(entity =>
        {
            entity.HasKey(e => e.PlaneId);
            entity.Property(e => e.PlaneId).HasColumnName("plane_id");
            entity.Property(e => e.NeedFuel).HasColumnName("need_fuel");
            entity.Property(e => e.NeedCatering).HasColumnName("need_catering");
            entity.Property(e => e.NeedBus).HasColumnName("need_bus");
            entity.Property(e => e.NeedFollowme).HasColumnName("need_followme");
            entity.Property(e => e.FuelDone).HasColumnName("fuel_done");
            entity.Property(e => e.CateringDone).HasColumnName("catering_done");
            entity.Property(e => e.BusDone).HasColumnName("bus_done");
            entity.Property(e => e.FollowmeDone).HasColumnName("followme_done");
            entity.Property(e => e.PassengersExpected).HasColumnName("passengers_expected");
            entity.Property(e => e.PassengersBoarded).HasColumnName("passengers_boarded");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.ToTable("plane_checklist");
        });

        modelBuilder.Entity<PlaneManifest>(entity =>
        {
            entity.HasKey(e => new { e.PlaneId, e.PassengerId });
            entity.Property(e => e.PlaneId).HasColumnName("plane_id");
            entity.Property(e => e.PassengerId).HasColumnName("passenger_id");
            entity.Property(e => e.BoardedAt).HasColumnName("boarded_at");

            entity.ToTable("plane_manifest");
        });

        modelBuilder.Entity<ProcessedEvent>(entity =>
        {
            entity.HasKey(e => e.EventId);
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.ProcessedAt).HasColumnName("processed_at");

            entity.ToTable("processed_events");
        });
    }
}
