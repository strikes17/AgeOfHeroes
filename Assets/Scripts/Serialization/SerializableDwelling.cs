namespace AgeOfHeroes.MapEditor
{
    public class SerializableDwelling : SerializableEntity
    {
        public PlayerColor PlayerColor;
        
        public SerializableDwelling(){}

        public SerializableDwelling(MapEditorDwelling mapEditorDwelling)
        {
            PlayerColor = mapEditorDwelling.PlayerColor;
            positionX = mapEditorDwelling.Position.x;
            positionY = mapEditorDwelling.Position.y;
            objectName = mapEditorDwelling.DwellBuilding.internalName;
        }

        public SerializableDwelling(DwellBuildingBehaviour dwellBuildingBehaviour)
        {
            PlayerColor = dwellBuildingBehaviour.Player.Color;
            positionX = dwellBuildingBehaviour.Position.x;
            positionY = dwellBuildingBehaviour.Position.y;
            objectName = dwellBuildingBehaviour.Building.internalName;
        }
    }
}