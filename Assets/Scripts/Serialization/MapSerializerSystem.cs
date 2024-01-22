using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using AgeOfHeroes.MapEditor;
using Newtonsoft.Json;
using Redcode.Moroutines;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Networking;

namespace AgeOfHeroes
{
    public class MapSerializerSystem : MonoBehaviour
    {
        public static MapSerializerSystem MapSerializer
        {
            get => _instance;
        }

        private static MapSerializerSystem _instance;

        private void Awake()
        {
            _instance = GetComponent<MapSerializerSystem>();
            string userMapsDirectory = GlobalStrings.USER_MAPS_DIRECTORY;
            string userMapsPreviewsDirectory = $"{userMapsDirectory}/Previews";
            if (!Directory.Exists(userMapsDirectory))
                Directory.CreateDirectory(userMapsDirectory);
            if (!Directory.Exists(userMapsPreviewsDirectory))
                Directory.CreateDirectory(userMapsPreviewsDirectory);
            DontDestroyOnLoad(gameObject);
        }

        public IEnumerator LoadInternalMap(string mapName, Action<SerializableMap> onMapLoaded)
        {
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.Formatting = Formatting.Indented;
            var mapResourcesPath = $"Internal Maps/{mapName}";
            var loadAsync = Resources.LoadAsync(mapResourcesPath, typeof(TextAsset));
            loadAsync.completed += (operation) =>
            {
                var mapFileContents = loadAsync.asset as TextAsset;
                var jsonObject =
                    JsonConvert.DeserializeObject<SerializableMap>(mapFileContents.text,
                        jsonSerializerSettings);
                onMapLoaded.Invoke(jsonObject);
            };
            yield return null;
        }

        public IEnumerator LoadMap(string mapName, Action<SerializableMap> onMapLoaded,
            MapCategory mapCategory = MapCategory.Original)
        {
            var mapFullPath = string.Empty;
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.Formatting = Formatting.Indented;
            switch (mapCategory)
            {
                case MapCategory.Original:
                    mapFullPath = $"{Application.streamingAssetsPath}/Maps/{mapName}.json";
                    yield return Moroutine.Run(WWWRequestSystem.Request.TextFile(mapFullPath, map =>
                    {
                        var mapFileContents = map;
                        var jsonObject =
                            JsonConvert.DeserializeObject<SerializableMap>(mapFileContents,
                                jsonSerializerSettings);
                        onMapLoaded.Invoke(jsonObject);
                    }));
                    break;
                case MapCategory.Custom:
                    mapFullPath = $"{GlobalStrings.USER_MAPS_DIRECTORY}/{mapName}.json";
                    var mapFileContents = File.ReadAllText(mapFullPath);
                    // Debug.Log("filecontents " + mapFileContents);
                    var jsonObject =
                        JsonConvert.DeserializeObject<SerializableMap>(mapFileContents,
                            jsonSerializerSettings);
                    // Debug.Log($"Loaded {jsonObject.MapInfo.Name}");
                    onMapLoaded.Invoke(jsonObject);
                    break;
                case MapCategory.SavedGame:
                    mapFullPath = $"{GlobalStrings.SAVED_GAMES_PATH}/{mapName}.json";
                    yield return Moroutine.Run(WWWRequestSystem.Request.TextFile(mapFullPath, map =>
                    {
                        var mapFileContents = map;
                        var jsonObject =
                            JsonConvert.DeserializeObject<SerializableMap>(mapFileContents,
                                jsonSerializerSettings);
                        onMapLoaded.Invoke(jsonObject);
                    }));
                    break;
            }
        }

        public IEnumerator GetAvailableMaps(Action<List<SerializableMap>> onMapsReady)
        {
            var allMaps = new List<SerializableMap>();
            string mapsContainerInfoPath = GlobalStrings.MAPS_CONTAINER_PATH;
            MapsContainerFile mapsContainerFile = null;
            yield return Moroutine.Run(WWWRequestSystem.Request.TextFile(mapsContainerInfoPath, (fileContents) =>
            {
                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
                jsonSerializerSettings.Formatting = Formatting.Indented;
                mapsContainerFile =
                    JsonConvert.DeserializeObject<MapsContainerFile>(fileContents, jsonSerializerSettings);
            })).WaitForComplete();
            foreach (var mapPairedInfo in mapsContainerFile.maps)
            {
                yield return Moroutine.Run(LoadMap(mapPairedInfo.Item1, map => allMaps.Add(map))).WaitForComplete();
            }

            var customMapsPath = GlobalStrings.USER_MAPS_DIRECTORY;
            var customMapsDirectoryInfo = new DirectoryInfo(customMapsPath);
            var customMapsFiles = customMapsDirectoryInfo.GetFiles("*.json", SearchOption.TopDirectoryOnly).ToList();
            foreach (var mapFile in customMapsFiles)
            {
                var mapName = Path.GetFileNameWithoutExtension(mapFile.Name);
                Debug.Log($"I see {mapName}");
                yield return Moroutine.Run(LoadMap(mapName, map =>
                {
                    Debug.Log($"{map.SerializableMatchInfo.MapInfo.Name} here is!");
                    allMaps.Add(map);
                }, MapCategory.Custom)).WaitForComplete();
            }

            onMapsReady.Invoke(allMaps);
            yield break;
        }
    }
}