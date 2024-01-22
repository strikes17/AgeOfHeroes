using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using AgeOfHeroes.Spell;
using Ludiq.OdinSerializer.Utilities;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;

namespace AgeOfHeroes.Editor
{
    public class CombatAbilitiesEditorWindow : EditorWindow
    {
        [MenuItem("Ilya/Combat Abilities Editor")]
        private static void Init()
        {
            CombatAbilitiesEditorWindow window = (CombatAbilitiesEditorWindow) EditorWindow.GetWindow(typeof(CombatAbilitiesEditorWindow));
            window.titleContent.text = "Combat Abilities";
            window.titleContent.image = EditorResources.Load<Texture2D>("icons/ca_editor_icon.png");
            window.Show();
        }

        private void OnEnable()
        {
            Setup();
        }

        private void Setup()
        {
            settings.TypeNameHandling = TypeNameHandling.All;
            settings.Formatting = Formatting.Indented;
            combatAbilityClasses = new List<string>();
            combatAbilitiesFullNameClasses = new List<string>();
            var targetAssembly = Assembly.GetAssembly(typeof(SpecialAbility));
            var allTypesInAssembly = targetAssembly.GetTypes();
            var allCombatAbilitiyTypes = allTypesInAssembly.Where(x => x.IsSubclassOf(typeof(SpecialAbility)) && !x.IsAbstract).ToList();
            allCombatAbilitiyTypes.ForEach(x =>
            {
                combatAbilityClasses.Add(x.Name);
                combatAbilitiesFullNameClasses.Add(x.FullName);
            });

            combatAbilitiesClassesEnumStrings = combatAbilityClasses.ToArray();

            UpdateRecentCombatAbilitiesList();
        }

        private void OnGUI()
        {
            DrawGUI();
        }

        private List<string> combatAbilityClasses, combatAbilitiesFullNameClasses;
        private string[] combatAbilitiesClassesEnumStrings;
        private int selectedCombatAbilityType = 0;
        private SpecialAbility _specialAbility;
        private FieldInfo[] fieldInfos;
        private JsonSerializerSettings settings = new JsonSerializerSettings();
        private Vector2 scrollViewValue, recentCombatAbilitiesScrollView;
        private List<string> recentCombatAbilitiesList;

        public void OpenCombatAbilitiesJson(string filePath)
        {
            var fileContents = File.ReadAllText(filePath);
            var json = JsonConvert.DeserializeObject<SpecialAbility>(fileContents, settings);
            _specialAbility = json;
            fieldInfos = _specialAbility.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            if (string.IsNullOrEmpty(_specialAbility.spriteIconPath))
                return;
            var path = _specialAbility.spriteIconPath.Split('+');
            var res = Resources.LoadAll<Sprite>($"Sprites/{path[0]}");
            res.ForEach(x =>
            {
                if (x.name == path[1])
                {
                    _specialAbility.spriteIcon = x;
                    return;
                }
            });
        }

        private void UpdateRecentCombatAbilitiesList()
        {
            recentCombatAbilitiesList = new List<string>();
            var combatAbilsDirInfo = new DirectoryInfo($"{Application.dataPath}/Resources/CombatAbilities");
            var allCombatAbilsFiles = combatAbilsDirInfo.GetFiles("*.json", SearchOption.AllDirectories);
            recentCombatAbilitiesList.AddRange(allCombatAbilsFiles.Select(x => Path.GetFileNameWithoutExtension(x.Name)).ToList());
        }

        private void SaveToJSON()
        {
            if (string.IsNullOrEmpty(_specialAbility.internalName))
            {
                EditorUtility.DisplayDialog("Ошибка", "Заполните internalName!", "ок");
                return;
            }

            // if (string.IsNullOrEmpty(combatAbility.description))
            // {
            //     StringBuilder stringBuilder = new StringBuilder();
            //     fieldInfos.ForEach(x =>
            //     {
            //         string name = x.Name;
            //         var value = x.GetValue(combatAbility);
            //         stringBuilder.AppendLine($"{name}: {value}");
            //     });
            //     combatAbility.description = stringBuilder.ToString();
            // }

            var combatAbilityJson = JsonConvert.SerializeObject(_specialAbility, settings);
            File.WriteAllText($"{Application.dataPath}/Resources/CombatAbilities/{_specialAbility.internalName}.json", combatAbilityJson);
            UpdateRecentCombatAbilitiesList();
            AssetDatabase.Refresh();
        }

        public void DrawGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();

            selectedCombatAbilityType = EditorGUILayout.Popup("Spell Type", selectedCombatAbilityType, combatAbilitiesClassesEnumStrings);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create"))
            {
                var spellTypeString = combatAbilitiesFullNameClasses[selectedCombatAbilityType];
                var instance = Activator.CreateInstance("AgeOfHeroes", spellTypeString);
                _specialAbility = instance.Unwrap() as SpecialAbility;
                fieldInfos = _specialAbility.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            }

            if (GUILayout.Button("Open"))
            {
                var filePath = EditorUtility.OpenFilePanel("Select combat ability.json", $"{Application.dataPath}/Resources/CombatAbilities", "json");
                OpenCombatAbilitiesJson(filePath);
            }

            EditorGUILayout.EndHorizontal();

            if (_specialAbility != null)
            {
                GlobalEditorHelper.DrawFields(fieldInfos, _specialAbility,scrollViewValue);
                var icon = _specialAbility.spriteIcon;
                if (icon != null)
                {
                    _specialAbility.spriteIconPath = $"{Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(icon))}+{icon.name}";
                }

                _specialAbility.combatAbilityOrder = (CombatAbilityOrder) EditorGUILayout.EnumPopup("Combat Ability Order", (CombatAbilityOrder) _specialAbility.combatAbilityOrder);
                _specialAbility.combatAbilityType = (CombatAbilityType) EditorGUILayout.EnumPopup("Combat Ability Type", (CombatAbilityType) _specialAbility.combatAbilityType);

                if (GUILayout.Button("Update description"))
                {
                    UpdateDescription();
                }

                if (GUILayout.Button("Save To JSON"))
                {
                    SaveToJSON();
                }
            }

            EditorGUILayout.EndVertical();

            #region Recent CombatAbilities

            EditorGUILayout.BeginVertical();
            using (var v = new EditorGUILayout.VerticalScope(GUI.skin.label))
            {
                recentCombatAbilitiesScrollView = EditorGUILayout.BeginScrollView(recentCombatAbilitiesScrollView);
                EditorGUILayout.LabelField("Recent:");
                EditorGUILayout.Space(10f);
                for (int i = 0; i < recentCombatAbilitiesList.Count; i++)
                {
                    if (GUILayout.Button(recentCombatAbilitiesList[i]))
                    {
                        if (_specialAbility != null)
                            SaveToJSON();
                        var filePath = $"{Application.dataPath}/Resources/CombatAbilities/{recentCombatAbilitiesList[i]}.json";
                        OpenCombatAbilitiesJson(filePath);
                    }
                }

                EditorGUILayout.EndScrollView();
            }

            EditorGUILayout.EndVertical();

            #endregion

            EditorGUILayout.EndHorizontal();
        }

        private void UpdateDescription()
        {
            string positiveColor = $"<color=#{ColorUtility.ToHtmlStringRGB(Colors.positiveBuffColor)}>";
            string negativeColor = $"<color=#{ColorUtility.ToHtmlStringRGB(Colors.negativeBuffColor)}>";
            string description = _specialAbility.description;
            string firstPattern = @"(\-|\+)(\d*)(\.|\d*)(\d*)";
            var matches = Regex.Matches(description, firstPattern);
            foreach (Match match in matches)
            {
                string str = match.Value;
                if (str.Contains("+"))
                {
                    string newString = $"{positiveColor}{str}</color>";
                    description = Regex.Replace(description, firstPattern, newString);
                }
                else if (str.Contains("-"))
                {
                    string newString = $"{negativeColor}{str}</color>";
                    description = Regex.Replace(description, firstPattern, newString);
                }
            }

            string dispellablePattern = @"\((Not(\s*)(\w*)|Dispellable(\s*))\)";
            var dispellableMatch = Regex.Match(description, dispellablePattern);
            if (!string.IsNullOrEmpty(dispellableMatch.Value))
            {
                description = Regex.Replace(description, dispellablePattern, string.Empty);
            }

            // string dispellableColorString = $"<color=#{ColorUtility.ToHtmlStringRGB(Colors.dispellableColor)}>";
            // string undispellableColorString = $"<color=#{ColorUtility.ToHtmlStringRGB(Colors.undispellableColor)}>";
            // string targetColorString = combatAbility.isDispellable ? dispellableColorString : undispellableColorString;
            // string targetDispellableString = combatAbility.isDispellable ? "(Dispellable)" : "(Not Dispellable)";
            // string dispellableString = $" {targetColorString}{targetDispellableString}</color>";
            // description += dispellableString;

            _specialAbility.description = description;
        }
    }
}