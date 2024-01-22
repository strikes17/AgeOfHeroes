using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using AgeOfHeroes.Spell;
using Ludiq.OdinSerializer.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace AgeOfHeroes.Editor
{
    public class BuildingEditor
    {
        private List<string> buildingClasses, buildingFullNameClasses;
        private string[] buildingsClassesEnumStrings;
        private int selectedBuildingType = 0;
        private AbstractBuilding buildingInstance;
        private FieldInfo[] fieldInfos;
        private JsonSerializerSettings settings = new JsonSerializerSettings();
        private Vector2 scrollViewValue, recentBuildingsScrollView;
        private List<string> recentBuildingsList;
        private List<string> allAvailableBuildings;
        private List<int> selectedBuildingsList;
        private string[] allAvailableBuildingsNames;

        public BuildingEditor()
        {
            settings.TypeNameHandling = TypeNameHandling.All;
            settings.Formatting = Formatting.Indented;
            buildingClasses = new List<string>();
            buildingFullNameClasses = new List<string>();
            var targetAssembly = Assembly.GetAssembly(typeof(AbstractBuilding));
            var allTypesInAssembly = targetAssembly.GetTypes();
            var allBuildingsTypes = allTypesInAssembly.Where(x => x.IsSubclassOf(typeof(AbstractBuilding)) && !x.IsAbstract).ToList();
            allBuildingsTypes.ForEach(x =>
            {
                buildingClasses.Add(x.Name);
                buildingFullNameClasses.Add(x.FullName);
            });

            buildingsClassesEnumStrings = buildingClasses.ToArray();

            UpdateRecentBuildingsList();
            UpdateAvailableBuildingsList();
        }

        public void OpenBuildingJson(string filePath)
        {
            var fileContents = File.ReadAllText(filePath);
            var json = JsonConvert.DeserializeObject<AbstractBuilding>(fileContents, settings);
            buildingInstance = json;
            fieldInfos = buildingInstance.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            selectedBuildingsList = new List<int>();
            json.requiredBuiltBuildings.ForEach(x => selectedBuildingsList.Add(allAvailableBuildings.IndexOf(x)));
            if(!(buildingInstance is SpecialBuilding))
                return;
            var specialBuilding = (SpecialBuilding) buildingInstance;
            if (specialBuilding == null) return;
            if (string.IsNullOrEmpty(specialBuilding.IconPath))
                return;
            var path = specialBuilding.IconPath.Split('+');
            var res = Resources.LoadAll<Sprite>($"Sprites/{path[0]}");
            res.ForEach(x =>
            {
                if (x.name == path[1])
                {
                    specialBuilding.EditorIcon = x;
                    return;
                }
            });
            InitGuardsDictionary();
        }

        private void UpdateRecentBuildingsList()
        {
            recentBuildingsList = new List<string>();
            var buildingDirInfo = new DirectoryInfo($"{Application.dataPath}/Resources/Buildings");
            var allBuildingFiles = buildingDirInfo.GetFiles("*.json", SearchOption.AllDirectories);
            var orderedFiles = from file in allBuildingFiles orderby file.CreationTime ascending select file;
            recentBuildingsList.AddRange(orderedFiles.Select(x => Path.GetFileNameWithoutExtension(x.Name)).ToList());
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

        private void InitGuardsDictionary()
        {
            if(buildingInstance is not CastleTierSpecialBuilding)return;
            var tierBuilding = buildingInstance as CastleTierSpecialBuilding;
            if (tierBuilding.StartingGuards.Count == 0)
            {
                tierBuilding.StartingGuards = new Dictionary<int, CastleTierSpecialBuilding.CastleGuardInfo>()
                {
                    { 1, new CastleTierSpecialBuilding.CastleGuardInfo() },
                    { 2, new CastleTierSpecialBuilding.CastleGuardInfo() },
                    { 3, new CastleTierSpecialBuilding.CastleGuardInfo() },
                    { 4, new CastleTierSpecialBuilding.CastleGuardInfo() },
                    { 5, new CastleTierSpecialBuilding.CastleGuardInfo() },
                    { 6, new CastleTierSpecialBuilding.CastleGuardInfo() },
                };
            }
        }

        public void DrawGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();

            selectedBuildingType = EditorGUILayout.Popup("Building Type", selectedBuildingType, buildingsClassesEnumStrings);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create"))
            {
                var spellTypeString = buildingFullNameClasses[selectedBuildingType];
                var instance = Activator.CreateInstance("AgeOfHeroes", spellTypeString);
                buildingInstance = instance.Unwrap() as AbstractBuilding;
                fieldInfos = buildingInstance.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                selectedBuildingsList = new List<int>();
                InitGuardsDictionary();
            }

            if (GUILayout.Button("Open"))
            {
                var filePath = EditorUtility.OpenFilePanel("Select building.json", $"{Application.dataPath}/Resources/Buildings", "json");
                OpenBuildingJson(filePath);
            }

            EditorGUILayout.EndHorizontal();

            if (buildingInstance != null)
            {
                buildingInstance.fraction = (Fraction) EditorGUILayout.EnumPopup(buildingInstance.fraction);
                GlobalEditorHelper.DrawFields(fieldInfos, buildingInstance,scrollViewValue, true);
                var specialBuilding = buildingInstance as SpecialBuilding;
                if (specialBuilding != null)
                {
                    var icon = specialBuilding.EditorIcon;
                    if (icon != null)
                        specialBuilding.IconPath = $"{Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(icon))}+{icon.name}";
                }

                EditorGUILayout.Space(10f);
                EditorGUILayout.LabelField("Required Buildings: ");
                for (int i = 0; i < buildingInstance.requiredBuiltBuildings.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    selectedBuildingsList[i] = EditorGUILayout.Popup(selectedBuildingsList[i], allAvailableBuildingsNames);
                    buildingInstance.requiredBuiltBuildings[i] = allAvailableBuildings[selectedBuildingsList[i]];
                    if (GUILayout.Button("-"))
                    {
                        buildingInstance.requiredBuiltBuildings.RemoveAt(i);
                        selectedBuildingsList.RemoveAt(i);
                    }

                    EditorGUILayout.EndHorizontal();
                }

                if (GUILayout.Button("+"))
                {
                    buildingInstance.requiredBuiltBuildings.Add(string.Empty);
                    selectedBuildingsList.Add(0);
                }

                if (buildingInstance is CastleTierSpecialBuilding)
                {
                    var tierBuilding = buildingInstance as CastleTierSpecialBuilding;
                    EditorGUILayout.Space(10f);
                    for (int i = 1; i <= 6; i++)
                    {
                        var guardAtTier = tierBuilding.StartingGuards[i];
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField($"{i})");
                        guardAtTier.FightingPoints = EditorGUILayout.IntField(guardAtTier.FightingPoints);
                        guardAtTier.IsElite = EditorGUILayout.Toggle(guardAtTier.IsElite);
                        EditorGUILayout.EndHorizontal();
                    }
                }

                EditorGUILayout.Space(10f);

                if (GUILayout.Button("Save To JSON"))
                {
                    if (string.IsNullOrEmpty(buildingInstance.internalName))
                    {
                        EditorUtility.DisplayDialog("Ошибка", "Заполните internalName!", "ок");
                        return;
                    }

                    if (string.IsNullOrEmpty(buildingInstance.description))
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        fieldInfos.ForEach(x =>
                        {
                            string name = x.Name;
                            var value = x.GetValue(buildingInstance);
                            stringBuilder.AppendLine($"{name}: {value}");
                        });
                        buildingInstance.description = stringBuilder.ToString();
                    }

                    var buildingJson = JsonConvert.SerializeObject(buildingInstance, settings);
                    File.WriteAllText($"{Application.dataPath}/Resources/Buildings/{buildingInstance.internalName}.json", buildingJson);
                    UpdateRecentBuildingsList();
                    UpdateAvailableBuildingsList();
                    AssetDatabase.Refresh();
                }
            }

            EditorGUILayout.EndVertical();

            #region Recent Buildings

            EditorGUILayout.BeginVertical();
            using (var v = new EditorGUILayout.VerticalScope(GUI.skin.label))
            {
                recentBuildingsScrollView = EditorGUILayout.BeginScrollView(recentBuildingsScrollView);
                EditorGUILayout.LabelField("Recent:");
                EditorGUILayout.Space(10f);
                for (int i = 0; i < recentBuildingsList.Count; i++)
                {
                    if (GUILayout.Button(recentBuildingsList[i]))
                    {
                        // bool agree = EditorUtility.DisplayDialog("Внимание!", "Вы точно хотите переключиться на другое строение?", "Да", "Нет");
                        // if (!agree)
                        //     break;

                        var filePath = $"{Application.dataPath}/Resources/Buildings/{recentBuildingsList[i]}.json";
                        OpenBuildingJson(filePath);
                    }
                }

                EditorGUILayout.EndScrollView();
            }

            EditorGUILayout.EndVertical();

            #endregion

            EditorGUILayout.EndHorizontal();
        }
    }
}