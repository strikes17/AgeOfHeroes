using UnityEngine;

namespace AgeOfHeroes
{
    public static class GlobalStrings
    {
        public static string MAPS_CONTAINER_PATH = $"{Application.streamingAssetsPath}/maps.json";
        public static string USER_MAPS_DIRECTORY = $"{Application.persistentDataPath}/CustomMaps";
        public static string USER_SAVES_DIR = $"{Application.persistentDataPath}/Saves";
        public static string ORIGINAL_MAPS_DIRECTORY = $"{Application.streamingAssetsPath}/Maps";
        public static string TREASURES_DATABASE_PATH = $"{Application.dataPath}/Resources/Settings/treasures.json";
        public static string ARTIFACTS_DATABASE_PATH = $"{Application.dataPath}/Resources/Settings/artifacts.json";
        public static string BUILDINGS_DATABASE_PATH = $"{Application.dataPath}/Resources/Settings/buildings.json";
        public static string TREASURES_DATABASE_RESOURCES_PATH = $"Settings/treasures";
        public static string ARTIFACTS_DATABASE_RESOURCES_PATH = $"Settings/artifacts";
        public static string SAVED_GAMES_PATH = $"{Application.persistentDataPath}/saved_games";
    }
}