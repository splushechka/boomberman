using System.Timers;
using boooooom.Cells;
using boooooom.Entities;
using boooooom.Entities.Enemies;
using boooooom.Enums;
using boooooom.Non_Entity_Classes;
using Action = boooooom.NonEntityClasses.Action;

namespace boooooom.Game;

public class GameProcess
{
    private readonly int WinningScoreThreshold;
    
    private int loopCount = 0;
    
    private Coordinates playerStartCoords;
    public int Score { get; set; }
    
    public Cell[,] Field { get; set; }

    public Coordinates PlayerCoords { get; set; }
    
    public Action? Action { get; set; }
    
    public List<Enemy> Enemies { get; set; }
    
    public GameTimer Timer { get; set; }
    
    public bool IsGameOver { get; set; }
    
    public bool Win { get; set; }

    public GameProcess(Coordinates playerCoords, int winningScoreThreshold, List<Enemy> enemies)
    {
        PlayerCoords = playerCoords;
        playerStartCoords = playerCoords;
        Timer = new GameTimer(MainLoop);
        Timer.StartTimer();
        WinningScoreThreshold = winningScoreThreshold;
        Enemies = enemies; //[new ChaoticEnemy(new Coordinates(2, 5)), new LinearEnemy(true, new Coordinates(3, 2))];
    }
    
    private void RefreshEnemies()
    {
        Enemies.Clear();
        for (int y = 0; y < Field.GetLength(0); y++)
        {
            for (int x = 0; x < Field.GetLength(1); x++)
            {
                var cell = Field[y, x];
                foreach (var entity in cell.EntitiesOnCell)
                {
                    if (entity is Enemy enemy)
                    {
                        Enemies.Add(enemy);
                    }
                }
            }
        }
    }
    
    public void MainLoop(object sender, ElapsedEventArgs e)
    {
        var changedCells = new List<(Coordinates coords, Cell cell)>();
        loopCount++;

        if (loopCount % 2 == 0)
        {
            foreach (var enemy in Enemies)
            {
                var currentCoords = enemy.CurrentCoords;
                var currentCell = Field[currentCoords.Y, currentCoords.X];
                var moveChange = enemy.Move();
                var newCoords = new Coordinates(currentCoords.X + moveChange.X, currentCoords.Y + moveChange.Y);

                // Перевірка меж поля
                if (newCoords.X >= 0 && newCoords.X < Field.GetLength(1) && newCoords.Y >= 0 && newCoords.Y < Field.GetLength(0))
                {
                    var newCell = Field[newCoords.Y, newCoords.X];

                    // Перевірка прохідності нової клітинки
                    if (newCell.CanActiveEntityStep())
                    {
                        currentCell.EntitiesOnCell.Remove(enemy);
                        newCell.EntitiesOnCell.Add(enemy);
                        enemy.CurrentCoords = newCoords;



                        changedCells.Add((currentCoords, currentCell));
                        changedCells.Add((newCoords, newCell));
                    }
                    else if (enemy is LinearEnemy linearEnemy)
                    {
                        linearEnemy.ReverseDirection();

                    }
                }
            }

            // Перевірка на зіткнення з гравцем
            foreach (var enemy in Enemies)
            {
                var playerCoords = PlayerCoords;
                var playerCell = Field[playerCoords.Y, playerCoords.X];

                if (enemy.CurrentCoords.Equals(playerCoords))
                {
                    foreach (var entity in playerCell.EntitiesOnCell)
                    {
                        if (entity is PlayerEntity player)
                        {
                            player.MinusLiveEntity();
                            break;
                        }
                    }

                    var clonedPlayer = playerCell.EntitiesOnCell.Find(e => e.GetEntityType() == ActiveEntityType.Player)?.Clone();
                    Field[playerStartCoords.Y, playerStartCoords.X].EntitiesOnCell.Add(clonedPlayer);
                    playerCell.EntitiesOnCell.RemoveAll(e => e is PlayerEntity);
                    changedCells.Add((playerStartCoords, Field[playerStartCoords.Y, playerStartCoords.X]));
                    PlayerCoords = playerStartCoords;

                    GameRender.DrawLives(clonedPlayer.Lives, Field.GetLength(0));
                    GameRender.DrawChanges(changedCells);

                    if (clonedPlayer.IsDead())
                    {
                        Timer.StopTimer();
                        IsGameOver = true;
                    }
                    else
                    {
                        GameRender.DrawChanges(changedCells);
                        GameRender.DrawLives(clonedPlayer.Lives, Field.GetLength(0));
                    }
                }
            }

            GameRender.DrawChanges(changedCells);
        }

        // Логіка реакції геймпроцесу на дії гравця
        if (Action != null)
        {
            var currentCell = Field[PlayerCoords.Y, PlayerCoords.X];
            if (Action.Type == ActionType.Move)
            {
                var newCoords = new Coordinates(PlayerCoords.X + Action.CoordinatesChange.X, PlayerCoords.Y + Action.CoordinatesChange.Y);

                if (newCoords.X >= 0 && newCoords.X < Field.GetLength(1) && newCoords.Y >= 0 && newCoords.Y < Field.GetLength(0))
                {
                    var newCell = Field[newCoords.Y, newCoords.X];
                    if (newCell.CanActiveEntityStep())
                    {
                        var entity = currentCell.EntitiesOnCell.Select((x, idx) => (idx, x)).FirstOrDefault(pair => pair.x.GetEntityType() == ActiveEntityType.Player);
                        newCell.EntitiesOnCell.Add(entity.x);
                        currentCell.EntitiesOnCell.RemoveAt(entity.idx);
                        changedCells.Add((PlayerCoords, currentCell));
                        PlayerCoords = newCoords;
                        changedCells.Add((newCoords, newCell));

                        if (newCell.PrizeOnCell is Heart)
                        {
                            var player = entity.x as PlayerEntity;
                            player.Lives += 1;
                            newCell.PrizeOnCell = null;
                            GameRender.DrawLives(player.Lives, Field.GetLength(0));
                        }

                        if (currentCell.HasPrize(out _))
                        {
                            currentCell.PrizeOnCell = null;
                        }

                        if (newCell.HasPrize(out var prizeValue))
                        {
                            Score += prizeValue;
                            newCell.PrizeOnCell.Collect();
                            GameRender.DrawScore(Score, Field.GetLength(0));

                            if (Score >= WinningScoreThreshold)
                            {
                                HandleWin();
                            }
                        }
                    }
                }
            }
            else if (Action.Type == ActionType.PlaceBomb)
            {
                if (currentCell.BombOnCell == null)
                {
                    if (currentCell is EmptyCell emptyCell)
                    {
                        emptyCell.PlaceBomb();
                        changedCells.Add((PlayerCoords, currentCell));
                    }
                    else if (currentCell is BrickWall brickWall && brickWall.IsDestroyed)
                    {
                        brickWall.BombOnCell = new Bomb();
                        changedCells.Add((PlayerCoords, currentCell));
                    }
                }
            }

            Action = null;
        }
        
        for (int y = 0; y < Field.GetLength(0); y++)
        {
            for (int x = 0; x < Field.GetLength(1); x++)
            {
                var cell = Field[y, x];
                if (cell.IsAffectedByExplosion)
                {
                    cell.IsAffectedByExplosion = false;
                    changedCells.Add((new Coordinates(x, y), cell));
                }
            }
        }

        var explodedBombs = new List<(Coordinates coords, Bomb bomb)>();

        for (int y = 0; y < Field.GetLength(0); y++)
        {
            for (int x = 0; x < Field.GetLength(1); x++)
            {
                var cell = Field[y, x];
                if (cell.BombOnCell != null && cell.BombOnCell.IsExploded())
                {
                    explodedBombs.Add((new Coordinates(x, y), cell.BombOnCell));
                }
            }
        }

        foreach (var bombData in explodedBombs)
        {
            var bomb = bombData.bomb;
            for (int y = bombData.coords.Y - bomb.Radius; y <= bombData.coords.Y + bomb.Radius; y++)
            {
                if (y < 0 || y >= Field.GetLength(0))
                {
                    continue;
                }

                for (int x = bombData.coords.X - bomb.Radius; x <= bombData.coords.X + bomb.Radius; x++)
                {
                    if (x < 0 || x >= Field.GetLength(1))
                    {
                        continue;
                    }

                    var cell = Field[y, x];
                    if (cell.CanCellExplode())
                    {
                        cell.ExplodeCell(Field);
                        changedCells.Add((new Coordinates(x, y), cell));

                        // Логіка обробки ворогів на клітинці
                        var enemiesToRemove = new List<ActiveEntity>();
                        foreach (var entity in cell.EntitiesOnCell)
                        {
                            //entity.MinusLiveEntity();
                            GameRender.DrawLives(entity.Lives, Field.GetLength(0));

                            if (entity.IsDead())
                            {
                                if (entity.GetEntityType() == ActiveEntityType.Player)
                                {
                                    IsGameOver = true;
                                    GameRender.DrawLives(entity.Lives, Field.GetLength(0));
                                }
                                else if (entity.GetEntityType() == ActiveEntityType.Enemy)
                                {
                                    enemiesToRemove.Add(entity);
                                    cell.PrizeOnCell = new Heart();

                                    Enemies.Remove(entity as Enemy);
                                }
                            }
                        }

                        foreach (var enemy in enemiesToRemove)
                        {
                            cell.EntitiesOnCell.Remove(enemy);
                        }
                    }
                }
            }

            RefreshEnemies();
            Field[bombData.coords.Y, bombData.coords.X].BombOnCell = null;
            Field[bombData.coords.Y, bombData.coords.X].IsAffectedByExplosion = true;
            changedCells.Add((new Coordinates(bombData.coords.X, bombData.coords.Y), Field[bombData.coords.Y, bombData.coords.X]));
        }

        GameRender.DrawChanges(changedCells);
    }

    private void HandleWin()
    {
        Win = true;
        Timer.StopTimer();
        Console.Clear();
        IsGameOver = true;
        Console.WriteLine("Congratulations! You win! Your Score: " + Score);
    }
}