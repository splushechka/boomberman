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
    public string GetDrawSymbol(bool hasBomb, bool hasPrize, bool isAffectedByExplosion)
    {
        if (hasBomb)
        {
            return "🙀"; // Символ переляканого котика, коли котик на одній клітинці з бомбою
        }
        else if (isAffectedByExplosion)
        {
            return "😿"; // Смайлик печалі, коли гравець знаходиться на одній клітинці з ворогом
        }

        else if (hasPrize)
        {
            return "😻"; // Закоханий котик, коли гравець і приз на одній клітинці
        }
        return "😸"; // Фігурка котика, коли гравець на клітинці
    }
}