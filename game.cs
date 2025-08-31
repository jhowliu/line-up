using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Lineup
{
    class Game
    {
        private string[,] grid;
        private int rows;
        private int cols;

        public Game(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            grid = new string[rows, cols];
        }

        public void drawGrid()
        {
            for (int i = 0; i < rows; i++)
            {
                Console.Write("|");
                for (int j = 0; j < cols; j++)
                {
                    Console.Write("   ");
                    Console.Write("|");
                    // draw discs if exist
                }
                Console.WriteLine();
            }
        }

        public bool placeDisc(Disc disc, int col)
        {
            return false;
        }

        public bool endGame()
        {

            return false;
        }

        public bool save() {
            return false;
        }

        public bool load(string storagePath) {
            return false;
        }
    }
}
