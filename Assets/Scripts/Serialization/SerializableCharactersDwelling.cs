namespace AgeOfHeroes.MapEditor
{
    public class SerializableCharactersDwelling : SerializableDwelling
    {
        public SerializableCharactersDwelling(){}

        public SerializableCharactersDwelling(MapEditorDwelling mapEditorDwelling) : base(mapEditorDwelling)
        {
            
        }
        
        public SerializableCharactersDwelling(DwellBuildingBehaviour dwellBuildingBehaviour) : base(dwellBuildingBehaviour)
        {
        }
    }
}