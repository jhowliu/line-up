using System;

namespace Lineup
{
    class Program {
        enum GameMode
        {
            PlayerVsPlayer = 1,
            PlayerVsComputer,
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, Welcome to Lineup. Please select options:");
            Console.WriteLine("0: Load Game\n1: New Game\n2: Test Mode");
            Console.Write(">> ");
            int option = Convert.ToInt32(Console.ReadLine());

            if (option == 1)
            {
                Console.WriteLine("Select Game Mode:");
                Console.WriteLine("1: Player vs Player");
                Console.WriteLine("2: Player vs Computer");
                Console.Write(">> ");
                int gameMode = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Enter grid size (default 6x7):");
                Console.Write("Rows (press enter for default): ");
                string? rowInput = Console.ReadLine();
                int rows = string.IsNullOrEmpty(rowInput) ? 6 : Convert.ToInt32(rowInput);
                while (rows < Constant.DefaultRowSize)
                {
                    Console.Write("Please input row number again (minimum 6) >> ");
                    rows = Convert.ToInt32(Console.ReadLine());
                }

                Console.Write("Columns (press enter for default): ");
                string? colInput = Console.ReadLine();
                int cols = string.IsNullOrEmpty(colInput) ? 7 : Convert.ToInt32(colInput);
                while (cols < Constant.DefaultColumnSize)
                {
                    Console.Write("Please input column number again (minimum is 7) >> ");
                    cols = Convert.ToInt32(Console.ReadLine());
                }

                Game game = new Game(rows, cols, (GameMode)gameMode == GameMode.PlayerVsComputer);
                game.StartGameLoop();
            }
            else if (option == 0)
            {
                Game? game = FileManager.LoadGame("record.json");
                if (game != null)
                {
                    game.StartGameLoop();
                }
                else
                {
                    Console.WriteLine("Cannot load the game record. please create a new game.");
                }
            }
            else if (option == 2)
            {
                Console.WriteLine("=== TEST MODE ===");
                Console.WriteLine("Enter grid size (default 6x7):");
                Console.Write("Rows (press enter for default): ");
                string? rowInput = Console.ReadLine();
                int rows = string.IsNullOrEmpty(rowInput) ? 6 : int.Parse(rowInput);

                Console.Write("Columns (press enter for default): ");
                string? colInput = Console.ReadLine();
                int cols = string.IsNullOrEmpty(colInput) ? 7 : int.Parse(colInput);

                Game game = new Game(rows, cols);

                Console.WriteLine("Enter test sequence (comma-separated moves like O4,B5,M3):");
                Console.Write(">> ");
                string? testInput = Console.ReadLine();

                if (!string.IsNullOrEmpty(testInput))
                {
                    game.RunTestMode(testInput);
                }
                else
                {
                    Console.WriteLine("No test input provided.");
                }
            }
            else
            {
                Console.WriteLine("Invalid option. Please restart and select 0, 1, or 2.");
            }
        }
    }
}
