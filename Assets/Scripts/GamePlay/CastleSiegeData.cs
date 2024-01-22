namespace AgeOfHeroes
{
    public class CastleSiegeData
    {
        public delegate void OnSiegeEventDelegate(CastleSiegeData castleSiegeData);

        public Player loserPlayer;
        public Castle siegedCastle;
        public SiegeResult SiegeResult;

        public PlayableTerrain SiegedCastleTerrain
        {
            set
            {
                attackingPlayerData.SiegedCastleTerrain = value;
                defendingPlayerData.SiegedCastleTerrain = value;
            }
            get => attackingPlayerData.SiegedCastleTerrain;
        }

        public PlayerSiegeData attackingPlayerData = new PlayerSiegeData();
        public PlayerSiegeData defendingPlayerData = new PlayerSiegeData();
    }
}