namespace AgeOfHeroes.MapEditor
{
    public class SerializableMarker : SerializableEntity
    {
        public int Tier, Quantity, Level;
        public AIGuardMarkerCharacterType GuardMarkerCharacterType;
        public AIGuardMarkerPlayerStateType GuardMarkerPlayerStateType;
        
        public SerializableMarker()
        {
        }

        public SerializableMarker(MapEditorAISiegeGuardMarker mapEditorAIMarker)
        {
            positionX = mapEditorAIMarker.Position.x;
            positionY = mapEditorAIMarker.Position.y;
            Tier = mapEditorAIMarker.Tier;
            Quantity = mapEditorAIMarker.Quantity;
            Level = mapEditorAIMarker.Level;
        }

        public SerializableMarker(AIGuardMarker aiGuardMarker)
        {
            Tier = aiGuardMarker.Tier;
            Quantity = aiGuardMarker.Quantity;
            Level = aiGuardMarker.Level;
            positionX = aiGuardMarker.Position.x;
            positionY = aiGuardMarker.Position.y;
        }
    }
}