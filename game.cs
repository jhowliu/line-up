using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace Lineup
{
    public class Game
    {
        private Disc?[,] grid;
        private int rows;
        private int cols;
        private Player player1;
        private Player player2;
        private Player currentPlayer;
        private bool isPlayerVsComputer;

        public Game(int rows = 6, int cols = 7, bool isPlayerVsComputer = false)
        {
            this.rows = rows;
            this.cols = cols;
            this.isPlayerVsComputer = isPlayerVsComputer;
            int totalOrdinaryDiscs = rows * cols / 2;

            grid = new Disc[rows, cols];
            player1 = new Player(totalOrdinaryDiscs, 1);
            player2 = new Player(totalOrdinaryDiscs, 2);
            if (isPlayerVsComputer)
            {
                player2 = new ComputerPlayer(totalOrdinaryDiscs);
            }
            currentPlayer = player1;
        }
        public void DrawGrid()
        {
            Console.WriteLine();

            Console.Write(" ");
            for (int j = 0; j < cols; j++)
            {
                Console.Write($" {j} ");
                if (j < cols - 1) Console.Write(" ");
            }
            Console.WriteLine();

            for (int i = 0; i < rows; i++)
            {
                Console.Write("|");
                for (int j = 0; j < cols; j++)
                {
                    if (grid[i, j] == null)
                    {
                        Console.Write("   ");
                    }
                    else
                    {
                        Console.Write(grid[i, j].Symbol);
                    }
                    Console.Write("|");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public bool PlaceDisc(Disc disc, int col)
        {
            // boundary check
            if (col < 0 || col >= cols) return false;

            // special disc
            if (disc.Type == DiscType.Boring)
            {
                return PlaceBoringDisc(disc, col);
            }

            // find the place at the given column from bottom row to top
            for (int row = rows - 1; row >= 0; row--)
            {
                if (grid[row, col] == null)
                {
                    grid[row, col] = disc;
                    return true;
                }
            }
            return false;
        }

        private bool PlaceBoringDisc(Disc disc, int col)
        {
            // remove all discs in the given column
            for (int row = 0; row < rows; row++)
            {
                if (grid[row, col] != null)
                {
                    Disc? removedDisc = grid[row, col];
                    grid[row, col] = null;

                    if (removedDisc?.PlayerId == 1)
                        player1.ReturnDisc(removedDisc.Type);
                    else if (removedDisc?.PlayerId == 2)
                        player2.ReturnDisc(removedDisc.Type);
                }
            }

            grid[rows - 1, col] = disc;
            return true;
        }

        private bool PlaceMagDisc(Disc disc, int col)
        {
            return true;
        }

        public bool EndGame()
        {
            return CheckWinCondition() || IsGridFull();
        }

        private bool CheckWinCondition()
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    Disc? disc = grid[row, col];
                    if (disc != null)
                    {
                        if (CheckFourInRow(row, col, disc.PlayerId))
                            return true;
                    }
                }
            }
            return false;
        }

        private bool CheckFourInRow(int startRow, int startCol, int playerId)
        {
            // Game will check if the given disc that player placed contibuted to end game.
            int[][] directions = {
                [0, 1], [0, -1], // horizontal
                [1, 0], [-1, 0], // vertical
                [1, 1], [1, -1], // diagonal 
            };

            // check each direction from the give point until found the four consecutive discs
            foreach (var direction in directions)
            {
                int count = 1;
                int dRow = direction[0];
                int dCol = direction[1];

                for (int i = 1; i < 4; i++)
                {
                    int newRow = startRow + i * dRow;
                    int newCol = startCol + i * dCol;
                    // boundary check
                    if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols &&
                        grid[newRow, newCol] != null && grid[newRow, newCol]?.PlayerId == playerId)
                    {
                        count++;
                    }
                    else break;
                }

                if (count >= 4) return true;
            }
            return false;
        }

        private bool IsGridFull()
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (grid[row, col] == null) return false;
                }
            }
            return true;
        }

        public void StartGameLoop()
        {
            Random random = new Random();
            int action;
            int col;
            Disc? disc;

            while (!EndGame())
            {
                DrawGrid();
                Console.WriteLine($"Current Player: {currentPlayer.PlayerId})");
                Console.WriteLine("Choose action:");
                Console.WriteLine($"1. Place Ordinary Disc (remain: {currentPlayer.OrdinaryDisc.Number})");
                Console.WriteLine($"2. Place Boring Disc (remain: {currentPlayer.BoringDisc.Number})");
                Console.WriteLine($"3. Place Magnetic Disc (remain: {currentPlayer.MagneticDisc.Number})");
                Console.WriteLine("4. Save Game");
                Console.WriteLine("5. Help");
                if (currentPlayer.IsComputer)
                {
                    action = random.Next(0, 3);
                }
                else
                {
                    Console.Write($"Enter your action >> ");
                    action = Convert.ToInt32(Console.ReadLine());
                }


                if (action == 4)
                {
                    Save();
                    continue;
                }
                else if (action == 5)
                {
                    ShowHelp();
                    continue;
                }
                else if (action >= 1 && action <= 3)
                {
                    DiscType discType = action switch
                    {
                        1 => DiscType.Ordinary,
                        2 => DiscType.Boring,
                        3 => DiscType.Magnetic,
                        _ => DiscType.Ordinary
                    };

                    if (currentPlayer.IsComputer)
                    {
                        col = random.Next(0, 7);
                    }
                    else
                    {
                        Console.Write("Please enter the column number that you wanna place >> ");
                        col = Convert.ToInt32(Console.ReadLine());
                    }

                    disc = currentPlayer.MakeMove(col, discType);
                    if (disc != null && PlaceDisc(disc, col))
                    {
                        if (EndGame())
                        {
                            break;
                        }
                        // swtich player
                        currentPlayer = currentPlayer == player1 ? player2 : player1;
                    }
                    else
                    {
                        Console.WriteLine("Invalid move! Try again.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input! Use format: column action (e.g., '3 1')");
                }
            }

            DrawGrid();
            if (CheckWinCondition())
            {
                Console.WriteLine($"Game Over! Player {currentPlayer.PlayerId} wins!");
            }
            else
            {
                Console.WriteLine("Game Over! It's a draw!");
            }
        }

        private void ShowHelp()
        {
            Console.WriteLine("=== HELP ===");
            Console.WriteLine("Goal: Get consecutive 4 discs in a row (horizontal, vertical, or diagonal)");
            Console.WriteLine("Disc Types:");
            Console.WriteLine("1. Ordinary(O): Falls to lowest available space.");
            Console.WriteLine("2. Boring(B): Removes all discs in column, then places itself.");
            Console.WriteLine("3. Magnetic(M): Special disc and pull up the nearest below disc.");
            Console.WriteLine("Input format: [symbol+column]");
            Console.WriteLine("Example: 'O1' places ordinary disc in column 1");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public bool Save()
        {
            return false;
        }

        public bool Load(string filePath)
        {
            return false;
        }

        public void RunTestMode(string input) {}
    }
}
