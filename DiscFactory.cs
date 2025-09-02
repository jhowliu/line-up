namespace Lineup
{
    // Factory allows different games to create their own different disc in the feature
    public interface IDiscFactory
    {
        OrdinaryDisc CreateOrdinaryDisc(int playerId, int number);
        BoringDisc CreateBoringDisc(int playerId, int number);
        MagneticDisc CreateMagneticDisc(int playerId, int number);
    }

    public class StandardLineUpDiscFactory : IDiscFactory
    {
        public OrdinaryDisc CreateOrdinaryDisc(int playerId, int number)
        {
            return new OrdinaryDisc(playerId, number);
        }

        public BoringDisc CreateBoringDisc(int playerId, int number)
        {
            return new BoringDisc(playerId, number);
        }

        public MagneticDisc CreateMagneticDisc(int playerId, int number)
        {
            return new MagneticDisc(playerId, number);
        }
    }
}