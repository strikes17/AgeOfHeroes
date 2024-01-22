namespace AgeOfHeroes.MapEditor
{
    public class SerializableResourceDwelling : SerializableDwelling
    {
        public int goldValue;
        public SerializableResourceDwelling(){}

        public SerializableResourceDwelling(MapEditorDwelling mapEditorDwelling) : base(mapEditorDwelling)
        {

        }
        
        public SerializableResourceDwelling(DwellBuildingBehaviour dwellBuildingBehaviour) : base(dwellBuildingBehaviour)
        {
        }
    }
}