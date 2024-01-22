namespace AgeOfHeroes.MapEditor
{
    public class SerializableTreasure : SerializableEntity
    {
        public int Gold, Gems, Exp;
        public SerializableTreasure(){}
        public SerializableTreasure(MapEditorTreasure mapEditorTreasure)
        {
            var treasureObject = mapEditorTreasure.treasureObject;
            Gold = treasureObject.goldValue;
            Gems = treasureObject.gemsValue;
            Exp = treasureObject.experienceValue;
            objectName = treasureObject.name;
            positionX = mapEditorTreasure.Position.x;
            positionY = mapEditorTreasure.Position.y;
        }
        public SerializableTreasure(AbstractTreasure treasure)
        {
            Gold = treasure.GoldValue;
            Gems = treasure.GemsValue;
            Exp = treasure.ExperienceValue;
            objectName = treasure.TreasureObject.name;
            positionX = treasure.Position.x;
            positionY = treasure.Position.y;
        }
    }
}