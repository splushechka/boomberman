using boooooom.Enums;

namespace boooooom.Entities;

public class PlayerEntity : ActiveEntity
{
    public PlayerEntity()
    {
        Lives = 3;
    }

    public override ActiveEntityType GetEntityType()
    {
        return ActiveEntityType.Player;
    }

    public override ActiveEntity Clone()
    {
        return new PlayerEntity() { Lives = Lives };
    }
}