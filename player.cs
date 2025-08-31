namespace Lineup
{
    public class Player
    {
        protected int ordinaryDiscs = 0;
        protected int boringDiscs = 2;
        protected int magneticDiscs = 2;
        public string Symbol { get; protected set; }
        public int PlayerId { get; protected set; }

        public Player(int numOfOrdDiscs, string symbol, int playerId = 0)
        {
            ordinaryDiscs = numOfOrdDiscs;
            Symbol = symbol;
            PlayerId = playerId;
        }

        public virtual Disc? MakeMove(int column, DiscType discType = DiscType.Ordinary)
        {
            switch (discType)
            {
                case DiscType.Ordinary:
                    if (ordinaryDiscs > 0)
                    {
                        ordinaryDiscs--;
                        return new OrdinaryDisc(PlayerId, Symbol);
                    }
                    break;
                case DiscType.Boring:
                    if (boringDiscs > 0)
                    {
                        boringDiscs--;
                        return new BoringDisc(PlayerId, Symbol);
                    }
                    break;
                case DiscType.Magnetic:
                    if (magneticDiscs > 0)
                    {
                        magneticDiscs--;
                        return new MagneticDisc(PlayerId, Symbol);
                    }
                    break;
            }

            return null;
        }

        public void ReturnDisc(DiscType discType)

        {
            switch (discType)
            {
                case DiscType.Ordinary:
                    ordinaryDiscs++;
                    break;
                case DiscType.Boring:
                    boringDiscs++;
                    break;
                case DiscType.Magnetic:
                    magneticDiscs++;
                    break;
            }
        }
    }

    public class ComputerPlayer : Player
    {
        public ComputerPlayer(int numOfOrdDiscs, string symbol) : base(numOfOrdDiscs, symbol, 2) { }

        public override Disc MakeMove(int column, DiscType discType = DiscType.Ordinary)
        {
            Random random = new Random();
            int discChoice = random.Next(0, 3);
            
            DiscType chosenType = (DiscType)discChoice;
            return base.MakeMove(column, chosenType);
        }
    }
}