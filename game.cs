using System.Text.Json;

namespace Lineup
{
    public interface IGameTemplate
    {
        public void StartGameLoop();
        public bool EndGame();
        public string toJSON();

        static Game? LoadFromJSON(object data) => throw new NotImplementedException();
    }
    public class Game: IGameTemplate
    {
        private Disc?[][] grid;
        private int rows;
        private int cols;
        private Player player1;
        private Player player2;
        private Player currentPlayer;
        private IDiscFactory discFactory;
        private CommandInvoker commandInvoker;

        private IPlaceDiscStrategy strategy = new OrdinaryDiscStrategy();

        public Game(int rows = 6, int cols = 7, bool isPlayerVsComputer = false)
        {
            this.rows = rows;
            this.cols = cols;
            this.discFactory = new StandardLineUpDiscFactory();
            this.commandInvoker = new CommandInvoker();
            int totalOrdinaryDiscs = rows * cols / 2;

            // initiate 2d array
            grid = new Disc?[rows][];
            for (int r = 0; r < rows; r++)
            {
                grid[r] = new Disc?[cols];
            }

            player1 = new Player(totalOrdinaryDiscs, 1, discFactory);
            player2 = new Player(totalOrdinaryDiscs, 2, discFactory);
            if (isPlayerVsComputer)
            {
                player2 = new ComputerPlayer(totalOrdinaryDiscs, discFactory);
            }
            currentPlayer = player1;
        }

        // Getter methods for strategy pattern access
        public Disc?[][] GetGrid() => grid;
        public int GetRows() => rows;
        public int GetCols() => cols;
        public Player GetCurrentPlayer() => currentPlayer;
        public Player GetPlayer1() => player1;
        public Player GetPlayer2() => player2;

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
                    if (grid[i][j] == null)
                    {
                        Console.Write("   ");
                    }
                    else
                    {
                        Console.Write(grid[i][j]?.Symbol);
                    }
                    Console.Write("|");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public bool EndGame()
        {
            return CheckWinCondition() != null || IsGridFull();
        }

        private int? CheckWinCondition()
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    Disc? disc = grid[row][col];
                    if (disc != null)
                    {
                        if (CheckFourInRow(row, col, disc.PlayerId))
                            return disc.PlayerId;
                    }
                }
            }

            return null;
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
                        grid[newRow][newCol] != null && grid[newRow][newCol]?.PlayerId == playerId)
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
                    if (grid[row][col] == null) return false;
                }
            }
            return true;
        }

        // Template Method Pattern - Main game loop template
        public void StartGameLoop()
        {
            InitializeGameLoop();

            while (!EndGame())
            {
                ExecuteGameTurn();
            }

            DisplayGameResult();
        }

        // Template method steps
        private void InitializeGameLoop()
        {
            Console.WriteLine("Game started!");
        }

        private void ExecuteGameTurn()
        {
            DisplayGameState();

            int action;
            if (currentPlayer.IsComputer)
            {
                Random random = new Random();
                action = random.Next(1, 4); // Computer only places discs (actions 1-3)
                Console.WriteLine($"Computer Player {currentPlayer.PlayerId} chose action: {action}");
            }
            else
            {
                Console.Write("Enter your action >> ");
                if (!int.TryParse(Console.ReadLine(), out action))
                {
                    Console.WriteLine("Invalid input, try again!!");
                    return;
                }
            }

            // Perform action using commend methods
            ICommand? command = CreateCommandFromAction(action);
            if (command != null)
            {
                bool success = commandInvoker.ExecuteCommand(command);
                // switch player if finished disc placement
                if (success && IsDiscPlacementAction(action))
                {
                    SwitchCurrentPlayer();
                }
                else if (!success && IsDiscPlacementAction(action))
                {
                    Console.WriteLine("Invalid input, try again!!");
                }
            }
            else
            {
                Console.WriteLine("Invalid input, try again!!");
            }
        }

        private void DisplayGameState()
        {
            DrawGrid();
            Console.WriteLine($"Current Player: {currentPlayer.PlayerId}");
            Console.WriteLine("Choose action:");
            Console.WriteLine($"1. Place Ordinary Disc (remain: {currentPlayer.OrdinaryDisc.Number})");
            Console.WriteLine($"2. Place Boring Disc (remain: {currentPlayer.BoringDisc.Number})");
            Console.WriteLine($"3. Place Magnetic Disc (remain: {currentPlayer.MagneticDisc.Number})");
            Console.WriteLine("4. Save Game");
            Console.WriteLine("5. Help");
        }

        private ICommand? CreateCommandFromAction(int action)
        {
            return action switch
            {
                4 => new SaveGameCommand(this),
                5 => new ShowHelpCommand(),
                >= 1 and <= 3 => CreatePlaceDiscCommand(action),
                _ => null
            };
        }

        private PlaceDiscCommand? CreatePlaceDiscCommand(int action)
        {
            int col;
            if (currentPlayer.IsComputer)
            {
                Random random = new Random();
                col = random.Next(0, cols);
                Console.WriteLine($"Computer Player {currentPlayer.PlayerId} chose column: {col}");
            }
            else
            {
                Console.Write("Please enter the column number that you want to place >> ");
                if (!int.TryParse(Console.ReadLine(), out col))
                {
                    return null;
                }
            }

            Disc ? disc = currentPlayer.MakeMove(col, (DiscType)(action - 1));

            if (disc != null)
            {
                return new PlaceDiscCommand(this, disc, col, currentPlayer);
            }

            Console.WriteLine("Unable to create disc. Player may be out of that disc type.");
            return null;
        }

        private bool IsDiscPlacementAction(int action)
        {
            return action >= 1 && action <= 3;
        }

        private void SwitchCurrentPlayer()
        {
            currentPlayer = currentPlayer == player1 ? player2 : player1;
        }

        private void DisplayGameResult()
        {
            DrawGrid();
            int? winnerId = CheckWinCondition();

            if (winnerId != null)
                Console.WriteLine($"Game Over! Player {winnerId} wins!");
            else
                Console.WriteLine("Game Over! It's a draw!");
        }

        public string toJSON()
        {
            return JsonSerializer.Serialize(new
            {
                rows,
                cols,
                grid,
                player1,
                player2,
                currentPlayer,
            });
        }

        public static Game? LoadFromJSON(object data)
        {
            try
            {
                JsonElement jsonData = (JsonElement)data;

                int rows = jsonData.GetProperty("rows").GetInt32();
                int cols = jsonData.GetProperty("cols").GetInt32();

                Game game = new Game(rows, cols);

                // Deserialize grid
                var grid = jsonData.GetProperty("grid");
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        if (grid[i][j].ValueKind != JsonValueKind.Null)
                        {
                            game.grid[i][j] = Disc.LoadFromJSON(grid[i][j]);
                        }
                    }
                }

                // Deserialize players
                var player1Element = jsonData.GetProperty("player1");
                game.player1 = Player.DeserializePlayer(player1Element, game.discFactory);

                var player2Element = jsonData.GetProperty("player2");
                game.player2 = Player.DeserializePlayer(player2Element, game.discFactory);

                // Set current player
                var currentPlayer = jsonData.GetProperty("currentPlayer");
                int currentPlayerId = currentPlayer.GetProperty("PlayerId").GetInt32();
                game.currentPlayer = currentPlayerId == 1 ? game.player1 : game.player2;

                return game;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading game from JSON: {ex.Message}");
                return null;
            }
        }
        public void RunTestMode(string input) { }

        public void SetPlaceDiscStrategy(IPlaceDiscStrategy strategy)
        {
            this.strategy = strategy;
        }

        public bool ExecutePlaceDisc(Disc disc, int col)
        {
            return this.strategy.PlaceDisc(this, disc, col);
        }
    }
}
