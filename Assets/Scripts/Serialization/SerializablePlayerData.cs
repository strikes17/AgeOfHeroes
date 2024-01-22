namespace AgeOfHeroes.MapEditor
{
    public class SerializablePlayerData
    {
        public SerializablePlayerData(){}
        public SerializablePlayerData(Player player)
        {
            Gold = player.Gold;
            Gems = player.Gems;
            PlayerColor = player.Color;
            AIPlayerData = new SerializableAIPlayerData(player.AIPlayerController);
        }

        public SerializableAIPlayerData AIPlayerData;
        public PlayerColor PlayerColor;
        public int Gold, Gems;
    }
}