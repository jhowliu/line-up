namespace Lineup
{
    class Game
    {
        private string[,] grid;
        private int rows;
        private int cols;

        public Game(int rows = 6, int cols = 7)
        {
            this.rows = rows;
            this.cols = cols;
            grid = new string[rows, cols];
        }

        public void drawGrid()
        {
            Console.WriteLine();
            for (int i = 0; i < rows; i++)
            {
                Console.Write("|");
                for (int j = 0; j < cols; j++)
                {
                    if (grid[i,j] == "") {
                        Console.Write("   ");
                    }
                    else {
                        Console.Write(grid[i, j]);
                    }
                    Console.Write("|");
                    // draw discs if exist
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public bool placeDisc(Disc disc, int col)
        {
            return false;
        }

        public bool endGame()
        {

            return false;
        }

        public bool save()
        {
            return false;
        }

        public bool load(string storagePath)
        {
            return false;
        }
    }
}
