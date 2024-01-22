using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace AgeOfHeroes.MapEditor
{
    public class MapEditorDatabase
    {
        public static MapEditorDatabase Instance => instance = instance == null ? new MapEditorDatabase() : instance;
        private static MapEditorDatabase instance = new MapEditorDatabase();

        private readonly Dictionary<Fraction, Dictionary<string, CharacterObject>> _loadedCharacters =
            new()
            {
                { Fraction.Human, new Dictionary<string, CharacterObject>() },
                { Fraction.Undead, new Dictionary<string, CharacterObject>() },
                { Fraction.Inferno, new Dictionary<string, CharacterObject>() },
                { Fraction.Mages, new Dictionary<string, CharacterObject>() },
                { Fraction.None, new Dictionary<string, CharacterObject>() },
            };

        private readonly Dictionary<Fraction, Dictionary<string, HeroObject>> _loadedHeroes =
            new()
            {
                { Fraction.Human, new Dictionary<string, HeroObject>() },
                { Fraction.Undead, new Dictionary<string, HeroObject>() },
                { Fraction.Inferno, new Dictionary<string, HeroObject>() },
                { Fraction.Mages, new Dictionary<string, HeroObject>() },
                { Fraction.None, new Dictionary<string, HeroObject>() },
            };

        private Dictionary<string, TerrainTileObject> _loadedTerrainTiles = new Dictionary<string, TerrainTileObject>();

        private Dictionary<string, AbstractBuilding> _loadedBuildings = new Dictionary<string, AbstractBuilding>();

        public List<TerrainTileObject> GetAllTerrainTileObjects()
        {
            return _loadedTerrainTiles.Values.ToList();
        }

        public List<AbstractBuilding> GetAllBuildings()
        {
            return _loadedBuildings.Values.ToList();
        }

        public List<CharacterObject> GetAllCharactersFromFraction(Fraction fraction)
        {
            var sorted = _loadedCharacters[fraction].Values.OrderBy(x => x.tier).ToList();
            return sorted;
        }

        public List<HeroObject> GetAllHeroesFromFraction(Fraction fraction)
        {
            var sorted = _loadedHeroes[fraction].Values.OrderBy(x => x.tier).ToList();
            return sorted;
        }

        public MapEditorDatabase()
        {
            LoadCharacters();
            LoadHeroes();
            LoadTerrainTiles();
            LoadBuildings();
        }

        private void LoadBuildings()
        {
            JsonSerializerSettings jsonSerializerSettings = GlobalVariables.GetDefaultSerializationSettings();

            string databasePath = "Settings/buildings";
            var fileContentsAsset = Resources.Load<TextAsset>(databasePath);
            var fileContents = fileContentsAsset.text;
            var jsonObject = JsonConvert.DeserializeObject<BuildingsDatabaseInfo>(fileContents, jsonSerializerSettings);
            foreach (var name in jsonObject.Buildings)
            {
                var abstractBuilding = ResourcesBase.GetBuilding(name);
                if (abstractBuilding != null)
                    _loadedBuildings.TryAdd(name, abstractBuilding);
            }
        }

        private void LoadTerrainTiles()
        {
            JsonSerializerSettings jsonSerializerSettings = GlobalVariables.GetDefaultSerializationSettings();

            string databasePath = "Settings/TerrainTilesInfo";
            var fileContentsAsset = Resources.Load<TextAsset>(databasePath);
            var fileContents = fileContentsAsset.text;
            var jsonObject = JsonConvert.DeserializeObject<TerrainDatabaseInfo>(fileContents, jsonSerializerSettings);
            foreach (var name in jsonObject.TerrainTileObjects)
            {
                var terrainTileObject = ResourcesBase.GetTerrainTileObject(name);
                if (terrainTileObject != null)
                    _loadedTerrainTiles.TryAdd(name, terrainTileObject);
            }
        }

        private void LoadCharacters()
        {
            JsonSerializerSettings jsonSerializerSettings = GlobalVariables.GetDefaultSerializationSettings();

            string databasePath = "Settings/CharactersInfo";
            var fileContentsAsset = Resources.Load<TextAsset>(databasePath);
            var fileContents = fileContentsAsset.text;
            var jsonObject =
                JsonConvert.DeserializeObject<CharactersDatabaseInfo>(fileContents, jsonSerializerSettings);
            foreach (var fraction in jsonObject.Characters)
            {
                var charactersNames = fraction.Value;
                foreach (var characterName in charactersNames)
                {
                    var characterObject = ResourcesBase.GetCharacterObject(characterName, fraction.Key);
                    _loadedCharacters[fraction.Key].Add(characterName, characterObject);
                }
            }
        }

        private void LoadHeroes()
        {
            JsonSerializerSettings jsonSerializerSettings = GlobalVariables.GetDefaultSerializationSettings();

            string databasePath = "Settings/CharactersInfo";
            var fileContentsAsset = Resources.Load<TextAsset>(databasePath);
            var fileContents = fileContentsAsset.text;
            var jsonObject =
                JsonConvert.DeserializeObject<CharactersDatabaseInfo>(fileContents, jsonSerializerSettings);
            foreach (var fraction in jsonObject.Heroes)
            {
                var heroesNames = fraction.Value;
                foreach (var heroName in heroesNames)
                {
                    var heroObject = ResourcesBase.GetHeroObject(heroName, fraction.Key);
                    _loadedHeroes[fraction.Key].Add(heroName, heroObject);
                }
            }
        }
    }
}