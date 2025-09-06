namespace Lineup
{
    public interface ICommand
    {
        bool Execute();
        bool CanExecute();
        void Undo(); // For future undo functionality
        void Redo(); // For futrue redo functionality
        string Description { get; }
    }

    public class ExitGameCommand : ICommand
    {
        public string Description { get; private set; }

        public ExitGameCommand() {
            Description = "Exit Game.";
        }

        public bool Execute()
        {
            if (!CanExecute()) return false;
            Environment.Exit(0);
            return true;
        }

        public void Redo() { }

        public void Undo() { }

        public bool CanExecute() {
            return true;
        }
    }

    public class PlaceDiscCommand : ICommand
    {
        private readonly Game game;
        private readonly Disc disc;
        private readonly int column;
        private readonly Player player;
        public string Description { get; private set; }

        public PlaceDiscCommand(Game game, Disc disc, int column, Player player)
        {
            this.game = game;
            this.disc = disc;
            this.column = column;
            this.player = player;
            Description = $"Place {disc.Type} disc in column {column} by Player {player.PlayerId}";
        }

        public bool Execute()
        {
            if (!CanExecute()) return false;

            // Set strategy based on disc type
            IPlaceDiscStrategy discStrategy = disc.Type switch
            {
                DiscType.Ordinary => new OrdinaryDiscStrategy(),
                DiscType.Boring => new BoringDiscStrategy(),
                DiscType.Magnetic => new MagneticDiscStrategy(),
                _ => new OrdinaryDiscStrategy()
            };

            game.SetPlaceDiscStrategy(discStrategy);
            bool success = game.ExecutePlaceDisc(disc, column);
            if (success)
            {
                game.GetCurrentPlayer().DeductDisc(disc.Type);
            }
            return success;
        }

        public bool CanExecute()
        {
            return disc != null &&
                   column >= 0 &&
                   column < game.GetCols() &&
                   !game.EndGame();
        }

        public void Undo()
        {
            // Future implementation for undo functionality
            // Would need to store previous game state
        }

        public void Redo()
        {
            // Future implementation for redo functionality
            // Would need to store previous game state
        }
    }

    public class SaveGameCommand : ICommand
    {
        private readonly Game game;
        private readonly string filePath;
        public string Description { get; private set; }

        public SaveGameCommand(Game game, string filePath = "record.json")
        {
            this.game = game;
            this.filePath = filePath;
            Description = $"Save game to {filePath}";
        }

        public bool Execute()
        {
            if (!CanExecute()) return false;
            
            bool success = FileManager.SaveGame(game, filePath);
            if (success)
            {
                Console.WriteLine("Game saved successfully.");
            }
            else
            {
                Console.WriteLine("Failed to save game.");
            }
            return success;
        }

        public bool CanExecute()
        {
            return game != null && !string.IsNullOrEmpty(filePath);
        }

        public void Undo()
        {
            // Undo save would be complex - might involve deleting saved file
            // For now, not implemented
        }

        public void Redo()
        {
            // Future implementation for redo functionality
            // Would need to store previous game state
        }
    }

    public class ShowHelpCommand : ICommand
    {
        public string Description { get; private set; }

        private int winningThreshold;

        public ShowHelpCommand(int winningThreshold)
        {
            this.winningThreshold = winningThreshold;
            Description = "Show help information";
        }

        public bool Execute()
        {
            if (!CanExecute()) return false;
            Console.WriteLine("============= HELP =============");
            Console.WriteLine($"Goal: Get consecutive {winningThreshold} discs in a row (horizontal, vertical, or diagonal)");
            Console.WriteLine("Disc Types:");
            Console.WriteLine("1. Ordinary(O): Falls to lowest available space.");
            Console.WriteLine("2. Boring(B): Removes all discs in column, then places itself.");
            Console.WriteLine("3. Magnetic(M): Special disc and pull up the nearest below disc.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return true;
        }

        public bool CanExecute()
        {
            return true; // Help can always be shown
        }

        public void Undo()
        {
            // Help display doesn't need undo
        }

        public void Redo()
        {
            // Help display doesn't need undo
        }
    }

    // Command invoker to manage and execute commands
    public class CommandInvoker
    {
        private readonly List<ICommand> commandHistory;

        public CommandInvoker()
        {
            commandHistory = new List<ICommand>();
        }

        public bool ExecuteCommand(ICommand command)
        {
            if (command.CanExecute())
            {
                bool result = command.Execute();
                if (result)
                {
                    // keep command history for redo and undo in the assignment 2
                    commandHistory.Add(command);
                    Console.WriteLine($"Executed: {command.Description}");
                }
                return result;
            }
            return false;
        }

        public void UndoLastCommand()
        {
            if (commandHistory.Count > 0)
            {
                var lastCommand = commandHistory.Last();
                lastCommand.Undo();
                commandHistory.RemoveAt(commandHistory.Count - 1);
                Console.WriteLine($"Undid: {lastCommand.Description}");
            }
        }

        public void ShowCommandHistory()
        {
            Console.WriteLine("=== COMMAND HISTORY ===");
            for (int i = 0; i < commandHistory.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {commandHistory[i].Description}");
            }
        }

        public int GetCommandCount() => commandHistory.Count;
    }
}