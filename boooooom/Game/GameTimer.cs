using System.Timers;

namespace boooooom.Game;
public class GameTimer
{
    private System.Timers.Timer Timer { get; set; }

    public GameTimer(ElapsedEventHandler loopFunction, int interval = 300, bool autoReset = true)
    {
        Timer = new System.Timers.Timer(interval: interval);
        Timer.Elapsed += loopFunction;
        Timer.AutoReset = autoReset;
    }

    public void StartTimer()
    {
        Timer.Enabled = true;
    }

    public void StopTimer()
    {
        Timer.Stop();
        Timer.Dispose();
        Timer.Close();
    }
}