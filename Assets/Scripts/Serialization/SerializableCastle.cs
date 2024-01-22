using System.Collections.Generic;

namespace AgeOfHeroes.MapEditor
{
    public class SerializableCastle : SerializableEntity
    {
        public int tier;
        public Fraction Fraction;
        public PlayerColor PlayerOwnerColor;
        public string castleMapName;
        public string castleInfoName;
        public Dictionary<string, SerializableBuilding> Buildings;
        public List<int> GarnisonCharactersIds = new List<int>();
        public float gemsValue;

        public SerializableCastle()
        {
            
        }
        
        public SerializableCastle(MapEditorCastle mapEditorCastle)
        {
            tier = mapEditorCastle.Tier;
            Fraction = mapEditorCastle.Fraction;
            PlayerOwnerColor = mapEditorCastle.PlayerOwnerColor;
            // castleMapName = mapEditorCastle.castleMapName;
            castleInfoName = mapEditorCastle.castleObject.internalName;
            objectName = mapEditorCastle.castleObject.name;
            positionX = mapEditorCastle.Position.x;
            positionY = mapEditorCastle.Position.y;
            Buildings = new Dictionary<string, SerializableBuilding>();
            UniqueId = mapEditorCastle.UniqueId;
            gemsValue = 0f;
        }

        public SerializableCastle(Castle castle)
        {
            tier = castle.Tier;
            Fraction = castle.Player.Fraction;
            PlayerOwnerColor = castle.PlayerOwnerColor;
            castleMapName = castle.castleMapName;
            castleInfoName = castle._castleObject.internalName;
            objectName = castle._castleObject.name;
            positionX = castle.Position.x;
            positionY = castle.Position.y;
            UniqueId = castle.UniqueId;
            gemsValue = castle.GemsValue;
            Buildings = new Dictionary<string, SerializableBuilding>();
            var shopBuildings = castle.ShopBuildings;
            var specialBuildings = castle.SpecialBuildings;
            foreach (var shop in shopBuildings)
            {
                var building = shop.Value;
                if (!building.IsBuilt)
                    continue;
                SerializableBuilding serializableBuilding = new SerializableBuilding(building);
                Buildings.Add(building.internalName, serializableBuilding);
            }
            
            foreach (var special in specialBuildings)
            {
                var building = special.Value;
                if (!building.IsBuilt)
                    continue;
                SerializableBuilding serializableBuilding = new SerializableBuilding(building);
                Buildings.Add(building.internalName, serializableBuilding);
            }

            var garnisonCharacters = castle.garnisonCharacters;
            foreach (var garnisonCharacter in garnisonCharacters)
            {
                GarnisonCharactersIds.Add(garnisonCharacter.UniqueId);
            }
        }
    }
}