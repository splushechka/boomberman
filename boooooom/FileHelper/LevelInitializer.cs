using boooooom.Cells;
using boooooom.Entities;

namespace boooooom.FileHelper;

public class LevelInitializer
{
    public Cell[,] ParseField(int height, int width, string path)
    {
        var result = new Cell[height, width];

        using (var reader = new StreamReader(path))
        {
            string line;
            int counterH = 0;
            while ((line = reader.ReadLine()) != null)
            {
                int counterW = 0;
                foreach (var symbol in line)
                {
                    result[counterH, counterW] = ParseCell(symbol);
                    counterW += 1;
                }

                counterH += 1;
            }
        }

        return result;
    }

    private Cell ParseCell(char symbol)
    {
        Cell result = new EmptyCell();
        switch (symbol)
        {
            case '#':
                result = new StoneWall();
                break;
            case 'E':
                result = new EmptyCell(true);
                break;
            case 'B':
                result = new BrickWall();
                break;
            case 'P':
                result = new BrickWall() { PrizeOnCell = new Prize(100) };
                break;
        }

        return result;
    }
}