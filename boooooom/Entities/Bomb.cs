using System.Timers;
using Timer = System.Timers.Timer;

namespace boooooom.Entities;

public class Bomb
{
    private bool exploded;
    private Timer detonationTimer;

    public int Radius { get; init; }
    public const int DetonationDelay = 3000;

    public Bomb()
    {
        detonationTimer = new Timer(DetonationDelay);
        detonationTimer.Elapsed += DetonationTimerElapsed;
        detonationTimer.AutoReset = false;
        detonationTimer.Start();
        Radius = 1;
    }

    private void DetonationTimerElapsed(object sender, ElapsedEventArgs e)
    {
        exploded = true;
        detonationTimer.Stop();
    }

    public string GetDrawSymbol()
    {
        return "💣";
    }

    public bool IsExploded()
    {
        return exploded;
    }
}
