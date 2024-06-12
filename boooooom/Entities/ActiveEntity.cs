using boooooom.Enums;

namespace boooooom.Entities;

public abstract class ActiveEntity
{
    public int Lives { get; set; }
    
    public abstract ActiveEntityType GetEntityType();
    
    public virtual void MinusLiveEntity()
    {
        Lives--;
    }

    public virtual bool IsDead()
    {
        return Lives <= 0;
    }

    public abstract ActiveEntity Clone();
}