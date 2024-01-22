using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Ludiq.OdinSerializer.Utilities;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace AgeOfHeroes.Editor
{
    public class CastleEditor
    {
        public List<string> allCastleInfoFiles;
        public string[] allCastleInfoFilesNames;

        private List<string> buildings;
        private List<string> allAvailableBuildings;
        public string[] allAvailableBuildingsNames;
        private List<int> selectedBuildingsIndexes;
        private CastleInfo _castleInfo;
        private JsonSerializerSettings settings = new JsonSerializerSettings();
        private Vector2 recentCastlesScrollView, mainScrollView;
        private List<string> recentCastlesList;
        private List<bool> isBuildingBuilt = new List<bool>();
        private List<bool> isBuildingRestricted = new List<bool>();
        private List<int> buildingLevel = new List<int>();

        public CastleEditor()
        {
            settings.TypeNameHandling = TypeNameHandling.All;
            settings.Formatting = Formatting.Indented;
            UpdateAvailableBuildingsList();
            UpdateAvailableCastleInfoFiles();
            UpdateRecentCastlesList();
        }

        public void UpdateAvailableCastleInfoFiles()
        {
            allCastleInfoFiles = new List<string>();
            var buildingDirInfo = new DirectoryInfo($"{Application.dataPath}/Resources/Castles");
            var allBuildingFiles = buildingDirInfo.GetFiles("*.json", SearchOption.AllDirectories);
            allCastleInfoFiles.AddRange(allBuildingFiles.Select(x => Path.GetFileNameWithoutExtension(x.Name))
                .ToList());
            allCastleInfoFilesNames = allCastleInfoFiles.ToArray();
        }

        private void UpdateAvailableBuildingsList()
        {
            allAvailableBuildings = new List<string>();
            var buildingDirInfo = new DirectoryInfo($"{Application.dataPath}/Resources/Buildings");
            var allBuildingFiles = buildingDirInfo.GetFiles("*.json", SearchOption.AllDirectories);
            var orderedFiles = from file in allBuildingFiles orderby file.CreationTime ascending select file;
            allAvailableBuildings.AddRange(orderedFiles.Select(x => Path.GetFileNameWithoutExtension(x.Name)).ToList());
            allAvailableBuildingsNames = allAvailableBuildings.ToArray();
        }

        private void UpdateRecentCastlesList()
        {
            recentCastlesList = new List<string>();
            var buildingDirInfo = new DirectoryInfo($"{Application.dataPath}/Resources/Castles");
            var allBuildingFiles = buildingDirInfo.GetFiles("*.json", SearchOption.AllDirectories);
            recentCastlesList.AddRange(allBuildingFiles.Select(x => Path.GetFileNameWithoutExtension(x.Name)).ToList());
        }

        private void SaveCurrentCastle()
        {
            if (_castleInfo == null)
                return;
            _castleInfo.Buildings.Clear();
            buildings.ForEach(x => { _castleInfo.Buildings.Add(x); });

            _castleInfo.IsBuildingBuilt = isBuildingBuilt;
            _castleInfo.IsBuildingRestricted = isBuildingRestricted;
            _castleInfo.BuildingLevel = buildingLevel;
            var path = $"{Application.dataPath}/Resources/Castles/{_castleInfo.internalName}.json";
            var castleJson = JsonConvert.SerializeObject(_castleInfo, settings);
            File.WriteAllText(path, castleJson);
            AssetDatabase.Refresh();
        }

        private void OpenCastleJson(string filePath)
        {
            var fileContents = File.ReadAllText(filePath);
            var json = JsonConvert.DeserializeObject<CastleInfo>(fileContents, settings);
            _castleInfo = json;
            buildings = new List<string>();
            selectedBuildingsIndexes = new List<int>();
            json.Buildings.ForEach(x =>
            {
                buildings.Add(x);
                selectedBuildingsIndexes.Add(allAvailableBuildings.IndexOf(x));
            });
            isBuildingBuilt = _castleInfo.IsBuildingBuilt;
            isBuildingRestricted = _castleInfo.IsBuildingRestricted;
            if (_castleInfo.BuildingLevel.Count == 0)
            {
                int count = buildings.Count;
                for (int i = 0; i < count; i++)
                    _castleInfo.BuildingLevel.Add(1);
            }

            buildingLevel = _castleInfo.BuildingLevel;
            UpdateAvailableBuildingsList();
            UpdateRecentCastlesList();
        }

        public void DrawGUI()
        {
            mainScrollView = EditorGUILayout.BeginScrollView(mainScrollView);
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Create"))
            {
                buildings = new List<string>();
                _castleInfo = new CastleInfo();
                selectedBuildingsIndexes = new List<int>();
                UpdateAvailableBuildingsList();
                UpdateRecentCastlesList();
            }

            if (GUILayout.Button("Open"))
            {
                var filePath = EditorUtility.OpenFilePanel("Select castle.json",
                    $"{Application.dataPath}/Resources/Castles", "json");
                if (string.IsNullOrEmpty(filePath)) return;


                OpenCastleJson(filePath);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10f);

            if (_castleInfo != null)
            {
                _castleInfo.Fraction =
                    (Fraction)EditorGUILayout.EnumPopup(new GUIContent("Fraction: "), _castleInfo.Fraction);
                _castleInfo.internalName =
                    EditorGUILayout.TextField(new GUIContent("Internal Name: "), _castleInfo.internalName);
                EditorGUILayout.Space(10f);

                EditorGUILayout.LabelField("Buildings: ");
                for (int i = 0; i < buildings.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    selectedBuildingsIndexes[i] =
                        EditorGUILayout.Popup(selectedBuildingsIndexes[i], allAvailableBuildingsNames);
                    buildings[i] = allAvailableBuildingsNames[selectedBuildingsIndexes[i]];
                    if (GUILayout.Button("-"))
                    {
                        selectedBuildingsIndexes.RemoveAt(i);
                        isBuildingBuilt.RemoveAt(i);
                        isBuildingRestricted.RemoveAt(i);
                        buildingLevel.RemoveAt(i);
                        buildings.RemoveAt(i);
                    }

                    EditorGUILayout.EndHorizontal();
                    isBuildingBuilt[i] = EditorGUILayout.Toggle(new GUIContent("Is Built"), isBuildingBuilt[i]);
                    isBuildingRestricted[i] =
                        EditorGUILayout.Toggle(new GUIContent("Is Restricted"), isBuildingRestricted[i]);
                    buildingLevel[i] = EditorGUILayout.IntField(new GUIContent("Level"), buildingLevel[i]);
                }

                if (GUILayout.Button("+"))
                {
                    selectedBuildingsIndexes.Add(0);
                    isBuildingBuilt.Add(false);
                    isBuildingRestricted.Add(false);
                    buildingLevel.Add(1);
                    buildings.Add(string.Empty);
                }

                EditorGUILayout.Space(10f);

                if (GUILayout.Button("Save To Json"))
                {
                    SaveCurrentCastle();
                }
            }

            EditorGUILayout.EndVertical();

            #region Recent Castles

            EditorGUILayout.BeginVertical();
            using (var v = new EditorGUILayout.VerticalScope(GUI.skin.label))
            {
                recentCastlesScrollView = EditorGUILayout.BeginScrollView(recentCastlesScrollView);
                EditorGUILayout.LabelField("Recent:");
                EditorGUILayout.Space(10f);
                for (int i = 0; i < recentCastlesList.Count; i++)
                {
                    if (GUILayout.Button(recentCastlesList[i]))
                    {
                        SaveCurrentCastle();
                        var filePath = $"{Application.dataPath}/Resources/Castles/{recentCastlesList[i]}.json";
                        OpenCastleJson(filePath);
                    }
                }

                EditorGUILayout.EndScrollView();
            }

            EditorGUILayout.EndVertical();

            #endregion

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();
        }
    }
}