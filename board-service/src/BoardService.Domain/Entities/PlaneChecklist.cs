namespace BoardService.Domain.Entities;

public class PlaneChecklist
{
    public string PlaneId { get; set; } = null!;
    public bool NeedFuel { get; set; }
    public bool NeedCatering { get; set; } = true;
    public bool NeedBus { get; set; } = true;
    public bool NeedFollowme { get; set; } = true;
    public bool FuelDone { get; set; }
    public bool CateringDone { get; set; }
    public bool BusDone { get; set; }
    public bool FollowmeDone { get; set; }
    public int PassengersExpected { get; set; }
    public int PassengersBoarded { get; set; }
    public DateTime UpdatedAt { get; set; }
}
