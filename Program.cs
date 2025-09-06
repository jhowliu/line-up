namespace Lineup
{
    class Program
    {
        enum GameMode
        {
            PlayerVsPlayer = 1,
            PlayerVsComputer,
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, Welcome to Lineup. Please select options:");
            Console.WriteLine("1: Load Game\n2: New Game\n3: Test Mode");
            Console.Write(">> ");
            int option;
            string? rawInput = Console.ReadLine();
            while (!int.TryParse(rawInput, out option) || option <= 0 || option > 3)
            {
                Console.WriteLine("Please try again, please enter valid integer (1-3):");
                Console.WriteLine("1: Load Game\n2: New Game\n3: Test Mode");
                Console.Write(">> ");
                rawInput = Console.ReadLine();
            }

            if (option == 1)
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
                Console.WriteLine("Select Game Mode:");
                Console.WriteLine("1: Player vs Player");
                Console.WriteLine("2: Player vs Computer");
                Console.Write(">> ");
                int gameMode;
                rawInput = Console.ReadLine();
                while (!int.TryParse(rawInput, out gameMode) || gameMode <= 0 || gameMode > 2)
                {
                    Console.WriteLine("Please try again, please enter valid integer (1-2):");
                    Console.WriteLine("1: Player vs Player");
                    Console.WriteLine("2: Player vs Computer");
                    Console.Write(">> ");
                    rawInput = Console.ReadLine();
                }
                (int rows, int cols) = PromptForColsRows();
                Game game = new Game(rows, cols, (GameMode)gameMode == GameMode.PlayerVsComputer);
                game.StartGameLoop();
            }
            else if (option == 3)
            {
                Console.WriteLine("=== TEST MODE ===");
                Console.WriteLine("Enter grid size (default 6x7):");
                Console.Write("Rows (press enter for default): ");
                string? rowInput = Console.ReadLine();
                int rows;
                if (string.IsNullOrEmpty(rowInput))
                {
                    rows = 6;
                }
                else
                {
                    while (!int.TryParse(rowInput, out rows) || rows < 1)
                    {
                        Console.Write("Please input valid row number >> ");
                        rowInput = Console.ReadLine();
                    }
                }

                Console.Write("Columns (press enter for default): ");
                string? colInput = Console.ReadLine();
                int cols;
                if (string.IsNullOrEmpty(colInput))
                {
                    cols = Constant.DefaultColumnSize;
                }
                else
                {
                    while (!int.TryParse(colInput, out cols) || cols < 1)
                    {
                        Console.Write("Please input valid column number >> ");
                        colInput = Console.ReadLine();
                    }
                }

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

        private static (int, int) PromptForColsRows()
        {
            Console.WriteLine("Enter grid size (default 6x7):");
            Console.Write("Rows (press enter for default): ");
            string? rowInput = Console.ReadLine();
            int rows;
            if (string.IsNullOrEmpty(rowInput))
            {
                rows = Constant.DefaultRowSize;
            }
            else
            {
                while (!int.TryParse(rowInput, out rows) || rows < Constant.DefaultRowSize)
                {
                    Console.Write("Please input row number again (minimum 6) >> ");
                    rowInput = Console.ReadLine();
                }
            }

            Console.Write("Columns (press enter for default): ");
            string? colInput = Console.ReadLine();
            int cols;
            if (string.IsNullOrEmpty(colInput))
            {
                cols = Constant.DefaultColumnSize;
            }
            else
            {
                while (!int.TryParse(colInput, out cols) || cols < Constant.DefaultColumnSize)
                {
                    Console.Write("Please input column number again (minimum is 7) >> ");
                    colInput = Console.ReadLine();
                }
            }

            return (rows, cols);
        }
    }
}
