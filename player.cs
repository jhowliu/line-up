using System.Text.Json;
namespace Lineup
{
    public class Player
    {
        public Disc OrdinaryDisc { get; protected set; }
        public Disc BoringDisc { get; protected set; }
        public Disc MagneticDisc { get; protected set; }
        public int PlayerId { get; protected set; }
        public bool IsComputer { get; protected set; }
        protected IDiscFactory discFactory;

        public Player(int numOfOrdDiscs, int playerId = 1, IDiscFactory? factory = null)
        {
            discFactory = factory ?? new StandardLineUpDiscFactory();
            OrdinaryDisc = discFactory.CreateOrdinaryDisc(playerId, numOfOrdDiscs);
            BoringDisc = discFactory.CreateBoringDisc(playerId, 2);
            MagneticDisc = discFactory.CreateMagneticDisc(playerId, 2);
            PlayerId = playerId;
            IsComputer = false;
        }

        public virtual Disc? MakeMove(int column, DiscType discType = DiscType.Ordinary)
        {
            switch (discType)
            {
                case DiscType.Ordinary:
                    if (OrdinaryDisc.Number > 0)
                    {
                        OrdinaryDisc.Number--;
                        return OrdinaryDisc;
                    }
                    break;
                case DiscType.Boring:
                    if (BoringDisc.Number > 0)
                    {
                        BoringDisc.Number--;
                        return BoringDisc;
                    }
                    break;
                case DiscType.Magnetic:
                    if (MagneticDisc.Number > 0)
                    {
                        MagneticDisc.Number--;
                        return MagneticDisc;
                    }
                    break;
            }

            return null;
        }

        public static Player DeserializePlayer(JsonElement playerElement, IDiscFactory? factory = null)
        {
            int playerId = playerElement.GetProperty("PlayerId").GetInt32();
            bool isComputer = playerElement.GetProperty("IsComputer").GetBoolean();

            var ordinaryElement = playerElement.GetProperty("OrdinaryDisc");
            int ordinaryNumber = ordinaryElement.GetProperty("Number").GetInt32();

            Player player = isComputer ? new ComputerPlayer(ordinaryNumber, factory) : new Player(ordinaryNumber, playerId, factory);

            // Update disc numbers from saved state
            var boringElement = playerElement.GetProperty("BoringDisc");
            player.BoringDisc.Number = boringElement.GetProperty("Number").GetInt32();

            var magneticElement = playerElement.GetProperty("MagneticDisc");
            player.MagneticDisc.Number = magneticElement.GetProperty("Number").GetInt32();

            return player;
        }

        public void ReturnDisc(DiscType discType)

        {
            switch (discType)
            {
                case DiscType.Ordinary:
                    OrdinaryDisc.Number++;
                    break;
                case DiscType.Boring:
                    BoringDisc.Number++;
                    break;
                case DiscType.Magnetic:
                    MagneticDisc.Number++;
                    break;
            }
        }
    }

    public class ComputerPlayer : Player
    {
        public ComputerPlayer(int numOfOrdDiscs, IDiscFactory? factory = null) : base(numOfOrdDiscs, 2, factory)
        {
            IsComputer = true;
        }

        public override Disc? MakeMove(int column, DiscType discType)
        {
            return base.MakeMove(column, discType);
        }
    }
}