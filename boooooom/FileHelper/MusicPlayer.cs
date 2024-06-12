using boooooom.Enums;
using NAudio.Wave;

namespace boooooom.FileHelper;

public class MusicPlayer
{
    public static void Play()
    {
        while (Program.Status != GameStatus.Finished)
        {
            var music = new MusicPlayer();
            music.PlayMusic(Program.Status);
            
            while (Program.Status== GameStatus.Paused)
            {
                Thread.Sleep(1000);
            }
        }
    }
    
    private void PlayMusic(GameStatus status)
    {
        var path = "Resources/Song.wav";
        
        try
        {
            using var audioFile = new Mp3FileReader(path);
            using var outputDevice = new WaveOutEvent();
            
            outputDevice.Init(audioFile);
            outputDevice.Volume = 0.1F;
            outputDevice.Play();
            
            while (Program.Status != GameStatus.Finished)
            {
                if (Program.Status == GameStatus.Paused)
                {
                    outputDevice.Stop();
                    break;
                }
                    
                if (outputDevice.PlaybackState == PlaybackState.Stopped)
                {
                    if (status == 0)
                    {
                        audioFile.Seek(0, SeekOrigin.Begin);
                        outputDevice.Play();
                    }
                }
                   
                Thread.Sleep(100);
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }
}