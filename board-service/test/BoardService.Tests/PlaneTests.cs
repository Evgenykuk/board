using Xunit;
using BoardService.Domain.Entities;

namespace BoardService.Tests;

public class PlaneTests
{
    [Fact]
    public void Should_Create_Plane_With_Valid_Data()
    {
        var plane = new Plane
        {
            PlaneId = "PL-1",
            Capacity = 180,
            FuelLevel = 2000,
            FuelRequired = 3000,
            State = "InHangar",
            LocationNode = "HANGAR-1"
        };

        Assert.Equal("PL-1", plane.PlaneId);
        Assert.Equal(180, plane.Capacity);
        Assert.True(plane.FuelLevel < plane.FuelRequired);
    }
}
