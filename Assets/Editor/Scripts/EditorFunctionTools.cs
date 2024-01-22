using System;
using System.IO;
using AgeOfHeroes.MapEditor;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace AgeOfHeroes.Editor
{
    public static class EditorFunctionTools
    {
        public static JsonSerializerSettings GetDefaultSerializationSettings()
        {
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.Formatting = Formatting.Indented;
            jsonSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            return jsonSerializerSettings;
        }
        
        [MenuItem("Ilya/Create Buildings Database Info")]
        public static void CreateBuildingsDatabaseInfo()
        {
            BuildingsDatabaseInfo buildingsDatabaseInfo = new BuildingsDatabaseInfo();
            var artifactsDirectoryPath = $"{Application.dataPath}/Resources/Buildings";
            JsonSerializerSettings jsonSerializerSettings = GetDefaultSerializationSettings();
            var directoryInfo = new DirectoryInfo(artifactsDirectoryPath);
            var files = directoryInfo.GetFiles("*.json", SearchOption.AllDirectories);
            foreach (var fileInfo in files)
            {
                string artifactName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                buildingsDatabaseInfo.Buildings.Add(artifactName);
            }

            var json = JsonConvert.SerializeObject(buildingsDatabaseInfo, jsonSerializerSettings);
            File.WriteAllText(GlobalStrings.BUILDINGS_DATABASE_PATH, json);

            AssetDatabase.Refresh();
        }

        [MenuItem("Ilya/Create Artifacts Database Info")]
        public static void CreateArtifactsDatabaseInfo()
        {
            ArtifactsDatabaseInfo artifactsDatabaseInfo = new ArtifactsDatabaseInfo();
            var artifactsDirectoryPath = $"{Application.dataPath}/Resources/Artifacts";
            JsonSerializerSettings jsonSerializerSettings = GetDefaultSerializationSettings();
            var artifactsDirectoryInfo = new DirectoryInfo(artifactsDirectoryPath);
            var artifactFiles = artifactsDirectoryInfo.GetFiles("*.asset", SearchOption.AllDirectories);
            foreach (var artifactFile in artifactFiles)
            {
                string artifactName = Path.GetFileNameWithoutExtension(artifactFile.Name);
                artifactsDatabaseInfo.Artifacts.Add(artifactName);
            }

            var json = JsonConvert.SerializeObject(artifactsDatabaseInfo, jsonSerializerSettings);
            File.WriteAllText(GlobalStrings.ARTIFACTS_DATABASE_PATH, json);

            AssetDatabase.Refresh();
        }

        [MenuItem("Ilya/Create Treasures Database Info")]
        public static void CreateTreasuresDatabaseInfo()
        {
            TreasuresDatabaseInfo treasuresDatabaseInfo = new TreasuresDatabaseInfo();
            var treasureDirectoryPath = $"{Application.dataPath}/Resources/Treasures";
            JsonSerializerSettings jsonSerializerSettings = GetDefaultSerializationSettings();
            var treasuresDirectoryInfo = new DirectoryInfo(treasureDirectoryPath);
            var treasuresFiles = treasuresDirectoryInfo.GetFiles("*.asset", SearchOption.AllDirectories);
            foreach (var treasureFile in treasuresFiles)
            {
                string treasureName = Path.GetFileNameWithoutExtension(treasureFile.Name);
                treasuresDatabaseInfo.Treasures.Add(treasureName);
            }

            var json = JsonConvert.SerializeObject(treasuresDatabaseInfo, jsonSerializerSettings);
            File.WriteAllText(GlobalStrings.TREASURES_DATABASE_PATH, json);

            AssetDatabase.Refresh();
        }

        [MenuItem("Ilya/Create Maps Info Container")]
        public static void CreateMapsInfoContainer()
        {
            MapsContainerFile mapsContainerFile = new MapsContainerFile();
            var mapsDirectoryPath = $"{Application.streamingAssetsPath}/Maps";
            DirectoryInfo directoryInfo = new DirectoryInfo(mapsDirectoryPath);
            var mapsFiles = directoryInfo.GetFiles("*.json", SearchOption.AllDirectories);
            foreach (var mapFile in mapsFiles)
            {
                mapsContainerFile.maps.Add((Path.GetFileNameWithoutExtension(mapFile.Name), MapCategory.Original));
            }

            JsonSerializerSettings jsonSerializerSettings = GetDefaultSerializationSettings();

            var json = JsonConvert.SerializeObject(mapsContainerFile, jsonSerializerSettings);
            File.WriteAllText(GlobalStrings.MAPS_CONTAINER_PATH, json);

            AssetDatabase.Refresh();
        }

        [MenuItem("Ilya/Create Characters Database Info")]
        public static void CreateCharactersDatabaseInfo()
        {
            JsonSerializerSettings jsonSerializerSettings = GetDefaultSerializationSettings();

            CharactersDatabaseInfo charactersDatabaseInfo = new CharactersDatabaseInfo();
            var fractionsStrings = Enum.GetNames(typeof(Fraction));
            foreach (var fractionString in fractionsStrings)
            {
                string path = $"{Application.dataPath}/Resources/Characters/{fractionString}";
                bool isDirectoryExists = Directory.Exists(path);
                if (!isDirectoryExists)
                    continue;
                var directoryInfo = new DirectoryInfo(path);
                var charactersFiles = directoryInfo.GetFiles("*.asset", SearchOption.TopDirectoryOnly);
                foreach (var characterFile in charactersFiles)
                {
                    string characterName = Path.GetFileNameWithoutExtension(characterFile.Name);
                    var fraction = Enum.Parse<Fraction>(fractionString);
                    charactersDatabaseInfo.Characters[fraction].Add(characterName);
                }
            }

            CharactersEditorWindow.AddHeroes(charactersDatabaseInfo);
            var jsonFile = JsonConvert.SerializeObject(charactersDatabaseInfo, jsonSerializerSettings);
            string jsonTargetPath = $"{Application.dataPath}/Resources/Settings/CharactersInfo.json";
            File.WriteAllText(jsonTargetPath, jsonFile);
        }

        [MenuItem("Ilya/Create Terrain Database Info")]
        public static void CreateTerrainDatabaseInfo()
        {
            JsonSerializerSettings jsonSerializerSettings = GetDefaultSerializationSettings();
            TerrainDatabaseInfo terrainDatabaseInfo = new TerrainDatabaseInfo();
            string path = $"{Application.dataPath}/Resources/Terrain";
            bool isDirectoryExists = Directory.Exists(path);
            var directoryInfo = new DirectoryInfo(path);
            var terrainTilesFiles = directoryInfo.GetFiles("*.asset", SearchOption.TopDirectoryOnly);
            foreach (var terrainTileFile in terrainTilesFiles)
            {
                string terrainTileName = Path.GetFileNameWithoutExtension(terrainTileFile.Name);
                terrainDatabaseInfo.TerrainTileObjects.Add(terrainTileName);
            }

            var jsonFile = JsonConvert.SerializeObject(terrainDatabaseInfo, jsonSerializerSettings);
            string jsonTargetPath = $"{Application.dataPath}/Resources/Settings/TerrainTilesInfo.json";
            File.WriteAllText(jsonTargetPath, jsonFile);
        }
    }
}