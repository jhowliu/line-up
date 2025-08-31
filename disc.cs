using System.Diagnostics.SymbolStore;
using System.Linq.Expressions;

namespace Lineup
{
    public enum DiscType
    {
        Ordinary,
        Boring,
        Magnetic
    }

    public class Disc
    {
        public DiscType Type { get; set; }
        public string Symbol { get; set; }
        public int PlayerId { get; set; }
        public int Number { get; set; }

        public Disc(DiscType type, int playerId, int number, string symbol)
        {
            Type = type;
            Symbol = symbol;
            PlayerId = playerId;
            Number = number;
        }
    }

    public class OrdinaryDisc : Disc
    {
        public OrdinaryDisc(int playerId, int number) : base(DiscType.Ordinary, playerId, number, playerId == 1 ? " @ " : " # ") { }
    }

    public class BoringDisc : Disc
    {
        public BoringDisc(int playerId, int number) : base(DiscType.Boring, playerId, number, playerId == 1 ? " B " : " b ") { }
    }

    public class MagneticDisc : Disc
    {
        public MagneticDisc(int playerId, int number) : base(DiscType.Magnetic, playerId, number, playerId == 1 ? " M " : " m ") { }
    }
}