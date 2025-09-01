namespace Lineup
{
    public interface IPlaceDiscStrategy
    {
        bool PlaceDisc(Game game, Disc disc, int col);
    }

    public class OrdinaryDiscStrategy : IPlaceDiscStrategy
    {
        public bool PlaceDisc(Game game, Disc disc, int col)
        {
            // Boundary check
            if (col < 0 || col >= game.GetCols()) return false;

            // Find the place at the given column from bottom row to top
            for (int row = game.GetRows() - 1; row >= 0; row--)
            {
                if (game.GetGrid()[row][col] == null)
                {
                    game.GetGrid()[row][col] = disc;
                    return true;
                }
            }
            return false;
        }
    }

    public class BoringDiscStrategy : IPlaceDiscStrategy
    {
        public bool PlaceDisc(Game game, Disc disc, int col)
        {
            // Boundary check
            if (col < 0 || col >= game.GetCols()) return false;

            // Remove all discs in the given column
            for (int row = 0; row < game.GetRows(); row++)
            {
                if (game.GetGrid()[row][col] != null)
                {
                    Disc? removedDisc = game.GetGrid()[row][col];
                    game.GetGrid()[row][col] = null;

                    if (removedDisc?.PlayerId == 1)
                        game.GetPlayer1().ReturnDisc(removedDisc.Type);
                    else if (removedDisc?.PlayerId == 2)
                        game.GetPlayer2().ReturnDisc(removedDisc.Type);
                }
            }

            game.GetGrid()[game.GetRows() - 1][col] = disc;
            return true;
        }
    }

    public class MagneticDiscStrategy : IPlaceDiscStrategy
    {
        public bool PlaceDisc(Game game, Disc disc, int col)
        {
            // Boundary check
            if (col < 0 || col >= game.GetCols()) return false;

            int currDisc = -1;
            for (int row = game.GetRows() - 1; row >= 0; row--)
            {
                if (game.GetGrid()[row][col] == null)
                {
                    game.GetGrid()[row][col] = disc;
                    currDisc = row;
                    break;
                }
            }

            if (currDisc == -1) return false;

            int nearestPlayerDisc = -1;
            int lastOpponentDisc = -1;

            // Find the nearest player and opponent's disc
            for (int row = currDisc + 1; row < game.GetRows(); row++)
            {
                if (game.GetGrid()[row][col] != null)
                {
                    if (game.GetGrid()[row][col]?.Type == DiscType.Ordinary)
                    {
                        if (game.GetGrid()[row][col]?.PlayerId == game.GetCurrentPlayer().PlayerId)
                        {
                            nearestPlayerDisc = nearestPlayerDisc == -1 ? row : nearestPlayerDisc;
                        }
                        else
                        {
                            // Cannot find the position that below the player disc
                            if (nearestPlayerDisc == -1)
                            {
                                lastOpponentDisc = row;
                            }
                        }
                    }
                }
            }

            Console.WriteLine($"first player pos: {nearestPlayerDisc}, last opponent disc: {lastOpponentDisc}");

            // Swap the position
            if (nearestPlayerDisc != -1 && lastOpponentDisc != -1 && nearestPlayerDisc > lastOpponentDisc)
            {
                (game.GetGrid()[nearestPlayerDisc][col], game.GetGrid()[lastOpponentDisc][col]) = (game.GetGrid()[lastOpponentDisc][col], game.GetGrid()[nearestPlayerDisc][col]);
            }

            return true;
        }
    }
}