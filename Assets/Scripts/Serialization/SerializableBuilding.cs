namespace AgeOfHeroes.MapEditor
{
    public class SerializableBuilding
    {
        public string InternalName;
        public int Level;
        public float RecruitmentsAvailable;
        
        public SerializableBuilding(){}

        public SerializableBuilding(CharacterShopBuilding shopBuilding)
        {
            InternalName = shopBuilding.internalName;
            Level = shopBuilding.Level;
            RecruitmentsAvailable = shopBuilding.RecruitmentsAvailable;
        }
        
        public SerializableBuilding(SpecialBuilding specialBuilding)
        {
            InternalName = specialBuilding.internalName;
            Level = 1;
        }
    }
}