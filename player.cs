using System.Text.Json;
namespace Lineup
{
    public class Player
    {
        public Disc? OrdinaryDisc { get; protected set; }
        public Disc? BoringDisc { get; protected set; }
        public Disc? MagneticDisc { get; protected set; }
        public int PlayerId { get; protected set; }
        public bool IsComputer { get; protected set; }

        public Player(int playerId = 1)
        {
            PlayerId = playerId;
            IsComputer = false;
        }

        public Player SetOrdinaryDisc(OrdinaryDisc disc)
        {
            OrdinaryDisc = disc;
            return this;
        }
        public Player SetBoringDisc(BoringDisc disc)
        {
            BoringDisc = disc;
            return this;
        }

        public Player SetMagneticDisc(MagneticDisc disc)
        {
            MagneticDisc = disc;
            return this;
        }

        public static Player DeserializePlayer(JsonElement playerElement, IDiscFactory? factory = null)
        {
            int playerId = playerElement.GetProperty("PlayerId").GetInt32();
            bool isComputer = playerElement.GetProperty("IsComputer").GetBoolean();

            Player player = isComputer ? new ComputerPlayer() : new Player(playerId);

            player.OrdinaryDisc = Disc.LoadFromJSON(playerElement.GetProperty("OrdinaryDisc"));
            player.BoringDisc = Disc.LoadFromJSON(playerElement.GetProperty("BoringDisc"));
            player.MagneticDisc = Disc.LoadFromJSON(playerElement.GetProperty("MagneticDisc"));

            return player;
        }

        public void ReturnDisc(DiscType discType)
        {
            switch (discType)
            {
                case DiscType.Ordinary:
                    if (OrdinaryDisc != null) OrdinaryDisc.Number++;
                    break;
                case DiscType.Boring:
                    if (BoringDisc != null) BoringDisc.Number++;
                    break;
                case DiscType.Magnetic:
                    if (MagneticDisc != null) MagneticDisc.Number++;
                    break;
            }
        }
        public void DeductDisc(DiscType discType)
        {
            switch (discType)
            {
                case DiscType.Ordinary:
                    if (OrdinaryDisc != null) OrdinaryDisc.Number--;
                    break;
                case DiscType.Boring:
                    if (BoringDisc != null) BoringDisc.Number--;
                    break;
                case DiscType.Magnetic:
                    if (MagneticDisc != null) MagneticDisc.Number--;
                    break;
            }
        }
    }

    public class ComputerPlayer : Player
    {
        public ComputerPlayer() : base(2)
        {
            IsComputer = true;
        }
    }
}