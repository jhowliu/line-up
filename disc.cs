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

        public Disc(DiscType type, int playerId, string symbol)
        {
            Type = type;
            Symbol = symbol;
            PlayerId = playerId;
        }
    }

    public class OrdinaryDisc : Disc
    {
        public OrdinaryDisc(int playerId, string symbol) : base(DiscType.Ordinary, playerId, symbol) { }
    }

    public class BoringDisc : Disc
    {
        public BoringDisc(int playerId, string symbol) : base(DiscType.Boring, playerId, symbol) { }
    }

    public class MagneticDisc : Disc
    {
        public MagneticDisc(int playerId, string symbol) : base(DiscType.Magnetic, playerId, symbol) { }
    }
}