using System.Text.Json;
using boooooom.Entities;
using boooooom.Enums;
using boooooom.FileHelper;
using boooooom.Game;
using boooooom.JsonConverters;
using boooooom.Non_Entity_Classes;
using boooooom.Serializable;
using Action = boooooom.NonEntityClasses.Action;
    
namespace boooooom;

public class Program
{
    //create main loop which will play through all the game process
    //it will be used to create interface to talk to the code and select levels.
    private static List<List<string>> _levels = new() 
    {
        new List<string>{"Resources/L1Layout.txt", "Resources/L1Data.json"},
        new List<string>{"Resources/L2Layout.txt","Resources/L2Data.json"},
        new List<string>{"Resources/L3Layout.txt", "Resources/L3Data.json"},
    };

    public static GameStatus Status { get; set; } = GameStatus.Paused;
    
    static void Main(string[] args)
    {
        Console.Clear();
        Console.CursorVisible = false;
        
        var musicTask = new Task(() => MusicPlayer.Play());
        musicTask.Start();
        
        while (Status != GameStatus.Finished)
        {
            Console.WriteLine("Bomberman");
            
            Console.WriteLine("Press Space to start the Game!");
            
            Console.WriteLine("Press T to get instructions!");
            
            Console.WriteLine("Press R to close!");
            
            var key = Console.ReadKey();
            Console.WriteLine();
            
            if (key.Key == ConsoleKey.R) 
            {
                Status = GameStatus.Finished;
                Thread.Sleep(1000);
                Console.Clear();
            }
            else if (key.Key == ConsoleKey.T)
            {
                ShowInstructions();
            }
            else if (key.Key == ConsoleKey.Spacebar)
            {
                Console.Clear();
                var currentLevel = 0;
                
                var levelKey = ConsoleKey.Spacebar;
                
                while (levelKey != ConsoleKey.R)
                {
                    Status = GameStatus.Active;
                    var result = GameLoop(_levels[currentLevel]);
                    
                    Console.WriteLine("Select option");
                    if (result)
                    {
                        Console.WriteLine("Press Space to play next level");
                    }
                    Console.WriteLine("Press S to Replay this level");
                    
                    if (currentLevel != 0)
                    {
                        Console.WriteLine("Press P to play previous level");
                    }
                    Console.WriteLine("Press R to exit");
                    
                    levelKey = Console.ReadKey().Key;
                    
                    if (levelKey == ConsoleKey.Spacebar)
                    {
                        if (result && currentLevel <= 2)
                        {
                            currentLevel += 1;
                        }
                    }
                    else if (levelKey == ConsoleKey.P)
                    {
                        currentLevel = currentLevel >= 1 ? currentLevel - 1 : currentLevel;
                    }
                    else if (levelKey == ConsoleKey.S)
                    {
                        currentLevel = currentLevel;
                    }
                    else
                    {
                        levelKey = ConsoleKey.R;
                    }
                }
            }
            
            Console.Clear();
        }
    }
    private static void ShowInstructions()
    {
        Console.Clear();
        Console.WriteLine("Instructions:");
        Console.WriteLine("Our game is based on the famous game Bomberman and tells the story of a cat 😸 searching for fish 🐟, ");
        Console.WriteLine("while avoiding : 👾 (linear enemy moving along a line) and \U0001f9ff (chaotic enemy moving randomly).");
        Console.WriteLine("The cat uses bombs to destroy walls and enemies in the quest for fish.");
        Console.WriteLine("The cat needs to maintain its number of lives to avoid losing the game ❤️. Good luck!");
        Console.WriteLine();
        Console.WriteLine("Use W to move up");
        Console.WriteLine("Use S to move down");
        Console.WriteLine("Use A to move left");
        Console.WriteLine("Use D to move right");
        Console.WriteLine("Use Space to place a bomb");
        Console.WriteLine();
        Console.WriteLine("Press any key to return to the main menu");
        Console.ReadKey();
        Console.Clear();
    }

    private static Action ProcessKeyPress(ConsoleKeyInfo keyInfo)
    {
        Action action = null;

        switch (keyInfo.Key)
        {
            case ConsoleKey.W:
                action = new Action(ActionType.Move, new Coordinates(0, -1)); // Рух вгору
                break;
            case ConsoleKey.S:
                action = new Action(ActionType.Move, new Coordinates(0, 1)); // Рух вниз
                break;
            case ConsoleKey.A:
                action = new Action(ActionType.Move, new Coordinates(-1, 0)); // Рух вліво
                break;
            case ConsoleKey.D:
                action = new Action(ActionType.Move, new Coordinates(1, 0)); // Рух вправо
                break;
            case ConsoleKey.Spacebar:
                action = new Action(ActionType.PlaceBomb, new Coordinates(0, 0)); // Встановлення бомби
                break;
            default:
                break;
        }

        return action;
    }

    private static bool GameLoop(List<string> pathes)
    {
        string layoutPath = pathes[0];
        string levelDataPath = pathes[1];
        
        string jsonString = File.ReadAllText(levelDataPath);
        var options = new JsonSerializerOptions
        {
            Converters = { new EnemyListConverter() },
            WriteIndented = true
        };

        var levelSettings = JsonSerializer.Deserialize<LevelSettings>(jsonString, options);
        
        var playerCoords = new Coordinates(levelSettings.PlayerX,  levelSettings.PlayerY);

        var levelInit = new LevelInitializer();
        var field = levelInit.ParseField(levelSettings.Height, levelSettings.Width, layoutPath);

        var gameProcess = new GameProcess(playerCoords, levelSettings.Threshold, levelSettings.Enemies)
        {
            Field = field,
            PlayerCoords = playerCoords
        };
        
        var player = new PlayerEntity();

        var playerLives = player.Lives;
        
        Console.Clear();
        GameRender.Draw(field);
        GameRender.DrawScore(0, field.GetLength(0));
        GameRender.DrawLives(playerLives, field.GetLength(0));
        
        while (!gameProcess.IsGameOver)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                var action = ProcessKeyPress(keyInfo);

                gameProcess.Action = action;
            }
        }
        
        gameProcess.Timer.StopTimer();
        Thread.Sleep(100);
        Status = GameStatus.Paused;
        Console.Clear();
        Console.WriteLine("Game over");
        
        return gameProcess.Win;
    }
}