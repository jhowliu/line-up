using System.Text.Json;

namespace Lineup
{
    public interface IGameTemplate
    {
        public void StartGameLoop();
        public bool EndGame();
        public string ToJSON();

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
        private int winningThreshold;
        private StandardLineUpDiscFactory discFactory;
        private CommandInvoker commandInvoker;

        private IPlaceDiscStrategy strategy = new OrdinaryDiscStrategy();

        public Game(int rows = 6, int cols = 7, bool isPlayerVsComputer = false)
        {
            this.rows = rows;
            this.cols = cols;
            discFactory = new();
            commandInvoker = new();
            int totalOrdinaryDiscs = rows * cols / 2;
            winningThreshold = Convert.ToInt32(rows * cols * 0.1);

            // initiate 2d array
            grid = new Disc?[rows][];
            for (int r = 0; r < rows; r++)
            {
                grid[r] = new Disc?[cols];
            }

            player1 = new Player(1).
                SetOrdinaryDisc(discFactory.CreateOrdinaryDisc(1, totalOrdinaryDiscs)).
                SetMagneticDisc(discFactory.CreateMagneticDisc(1, 2)).
                SetBoringDisc(discFactory.CreateBoringDisc(1, 2));
            player2 = new Player(2).
                SetOrdinaryDisc(discFactory.CreateOrdinaryDisc(2, totalOrdinaryDiscs)).
                SetMagneticDisc(discFactory.CreateMagneticDisc(2, 2)).
                SetBoringDisc(discFactory.CreateBoringDisc(2, 2));

            if (isPlayerVsComputer)
            {
                player2 = new ComputerPlayer().
                    SetOrdinaryDisc(discFactory.CreateOrdinaryDisc(2, totalOrdinaryDiscs)).
                    SetMagneticDisc(discFactory.CreateMagneticDisc(2, 2)).
                    SetBoringDisc(discFactory.CreateBoringDisc(2, 2));
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
                if (j < 10)
                {
                    Console.Write($" {j + 1} ");
                }
                else
                {
                     Console.Write($"{j + 1} ");
                }
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
                        if (CheckNInRow(row, col, disc.PlayerId))
                            return disc.PlayerId;
                    }
                }
            }

            return null;
        }

        private bool CheckNInRow(int startRow, int startCol, int playerId)
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

                for (int i = 1; i < winningThreshold; i++)
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

                if (count >= winningThreshold) return true;
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
            Console.WriteLine($"1. Place Ordinary Disc (remain: {currentPlayer.OrdinaryDisc?.Number})");
            Console.WriteLine($"2. Place Boring Disc (remain: {currentPlayer.BoringDisc?.Number})");
            Console.WriteLine($"3. Place Magnetic Disc (remain: {currentPlayer.MagneticDisc?.Number})");
            Console.WriteLine("4. Save Game");
            Console.WriteLine("5. Help");
            Console.WriteLine("6. Exit Game");
        }

        private ICommand? CreateCommandFromAction(int action)
        {
            return action switch
            {
                >= 1 and <= 3 => CreatePlaceDiscCommand(action),
                4 => new SaveGameCommand(this),
                5 => new ShowHelpCommand(this.winningThreshold),
                6 => new ExitGameCommand(),
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
                Console.WriteLine($"Computer Player {currentPlayer.PlayerId} chose column: {col + 1}");
            }
            else
            {
                Console.Write("Please enter the column number that you want to place >> ");
                if (!int.TryParse(Console.ReadLine(), out int userInput))
                {
                    return null;
                }
                // Convert from 1-based user input to 0-based internal index
                col = userInput - 1;
                
                // Validate column range
                if (col < 0 || col >= cols)
                {
                    Console.WriteLine($"Invalid column! Please enter a number between 1 and {cols}.");
                    return null;
                }
            }

            Disc? disc = currentPlayer.MakeMove(action);
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

        public string ToJSON()
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
                game.player1 = Player.DeserializePlayer(player1Element);

                var player2Element = jsonData.GetProperty("player2");
                game.player2 = Player.DeserializePlayer(player2Element);

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
        public void RunTestMode(string input) 
        {
            Console.WriteLine("=== STARTING TEST MODE ===");
            Console.WriteLine($"Input sequence: {input}");
            
            // Parse the input string
            string[] plays = input.Split(',');
            
            foreach (string play in plays)
            {
                Console.WriteLine($"player 1 ord num: {player1.OrdinaryDisc?.Number}");
                Console.WriteLine($"player 2 ord disc num: {player2.OrdinaryDisc?.Number}");
                string trimmedPlay = play.Trim();
                if (string.IsNullOrEmpty(trimmedPlay)) continue;
                
                // Parse disc type and column
                char discTypeChar = char.ToUpper(trimmedPlay[0]);
                if (!int.TryParse(trimmedPlay.Substring(1), out int column))
                {
                    Console.WriteLine($"Invalid play format: {trimmedPlay}");
                    continue;
                }
                
                // Convert to 0-based index
                int col = column - 1;
                
                // Validate column
                if (col < 0 || col >= cols)
                {
                    Console.WriteLine($"Invalid column {column} in play: {trimmedPlay}");
                    continue;
                }
                
                // Determine disc type
                DiscType discType;
                switch (discTypeChar)
                {
                    case 'O':
                        discType = DiscType.Ordinary;
                        break;
                    case 'B':
                        discType = DiscType.Boring;
                        break;
                    case 'M':
                        discType = DiscType.Magnetic;
                        break;
                    case 'E':
                        Console.WriteLine($"Exploding disc not implemented yet, skipping play: {trimmedPlay}");
                        continue;
                    default:
                        Console.WriteLine($"Unknown disc type '{discTypeChar}' in play: {trimmedPlay}");
                        continue;
                }
                
                // Execute the play
                Console.WriteLine($"Player {currentPlayer.PlayerId} plays {discType} disc in column {column}");

                // Get the appropriate disc from the player
                Disc? disc = currentPlayer.MakeMove(discType);
                if (disc != null)
                {
                    PlaceDiscCommand command = new PlaceDiscCommand(this, disc, col, currentPlayer);
                    bool success = commandInvoker.ExecuteCommand(command);
                    
                    if (success)
                    {
                        // Show the grid after each move
                        DrawGrid();
                        
                        // Check if game ended
                        if (EndGame())
                        {
                            DisplayGameResult();
                            return;
                        }
                        
                        // Switch player
                        SwitchCurrentPlayer();
                    }
                    else
                    {
                        Console.WriteLine($"Failed to place disc in column {column}");
                    }
                }
                else
                {
                    Console.WriteLine($"Player {currentPlayer.PlayerId} is out of {discType} discs");
                }
            }
            
            // Final game state
            if (!EndGame())
            {
                Console.WriteLine("=== TEST SEQUENCE COMPLETED ===");
                DrawGrid();
            }
        }

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
