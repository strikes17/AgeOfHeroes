using AgeOfHeroes.AI;

namespace AgeOfHeroes.MapEditor
{
    public class SerializableAIPlayerData
    {
        public int EconomicsGold;
        
        public SerializableAIPlayerData(){}

        public SerializableAIPlayerData(BasicAIPlayerController aiPlayerController)
        {
            EconomicsGold = aiPlayerController.EconomicsGold;
        }
    }
}