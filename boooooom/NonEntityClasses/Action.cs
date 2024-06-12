using boooooom.Enums;
using boooooom.Non_Entity_Classes;

namespace boooooom.NonEntityClasses;

public class Action
{
    public ActionType Type { get; private set; }
    
    public Coordinates CoordinatesChange { get; private set; }

    public Action(ActionType type, Coordinates coordinatesChange)
    {
        Type = type;
        CoordinatesChange = coordinatesChange;
    }
}
