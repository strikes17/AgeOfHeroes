using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using AgeOfHeroes.Spell;
using Ludiq.OdinSerializer.Utilities;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace AgeOfHeroes.Editor
{
    public class BuffsEditor
    {
        private List<string> buffClasses, buffFullNameClasses;
        private string[] buffClassesEnumStrings;
        private int selectedSpellType = 0;
        private Buff buffInstance;
        private FieldInfo[] fieldInfos;
        private JsonSerializerSettings settings = new JsonSerializerSettings();
        private Vector2 scrollViewValue, recentBuffsScrollView;
        private List<string> recentBuffsList;

        public BuffsEditor()
        {
            settings.TypeNameHandling = TypeNameHandling.All;
            settings.Formatting = Formatting.Indented;
            buffClasses = new List<string>();
            buffFullNameClasses = new List<string>();
            var targetAssembly = Assembly.GetAssembly(typeof(Buff));
            var allTypesInAssembly = targetAssembly.GetTypes();
            var allBuffsTypes = allTypesInAssembly.Where(x => x.IsSubclassOf(typeof(Buff)) && !x.IsAbstract).ToList();
            allBuffsTypes.ForEach(x =>
            {
                buffClasses.Add(x.Name);
                buffFullNameClasses.Add(x.FullName);
            });

            buffClassesEnumStrings = buffClasses.ToArray();

            UpdateRecentBuffsList();
        }

        public void OpenBuffJson(string filePath)
        {
            var fileContents = File.ReadAllText(filePath);
            var json = JsonConvert.DeserializeObject<Buff>(fileContents, settings);
            buffInstance = json;
            fieldInfos = buffInstance.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            if (string.IsNullOrEmpty(buffInstance.spriteIconPath))
                return;
            var path = buffInstance.spriteIconPath.Split('+');
            var res = Resources.LoadAll<Sprite>($"Sprites/{path[0]}");
            res.ForEach(x =>
            {
                if (x.name == path[1])
                {
                    buffInstance.spriteIcon = x;
                    return;
                }
            });
            var spellTypeName = buffInstance.GetType().ToString().Replace("AgeOfHeroes.Spell.", string.Empty);
            List<string> list = new List<string>();
            foreach (var enumString in buffClassesEnumStrings) list.Add(enumString);
            selectedSpellType = list.IndexOf(spellTypeName);
        }

        private void UpdateRecentBuffsList()
        {
            recentBuffsList = new List<string>();
            var buffsDirInfo = new DirectoryInfo($"{Application.dataPath}/Resources/Buffs");
            var allBuffsFiles = buffsDirInfo.GetFiles("*.json", SearchOption.AllDirectories);
            recentBuffsList.AddRange(allBuffsFiles.Select(x => Path.GetFileNameWithoutExtension(x.Name)).ToList());
        }

        private void SaveToJSON()
        {
            if (string.IsNullOrEmpty(buffInstance.internalName))
            {
                EditorUtility.DisplayDialog("Ошибка", "Заполните internalName!", "ок");
                return;
            }

            var buffJson = JsonConvert.SerializeObject(buffInstance, settings);
            File.WriteAllText($"{Application.dataPath}/Resources/Buffs/{buffInstance.internalName}.json", buffJson);
            UpdateRecentBuffsList();
            AssetDatabase.Refresh();
        }

        public void DrawGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();

            selectedSpellType = EditorGUILayout.Popup("Spell Type", selectedSpellType, buffClassesEnumStrings);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create"))
            {
                var spellTypeString = buffFullNameClasses[selectedSpellType];
                var instance = Activator.CreateInstance("AgeOfHeroes", spellTypeString);
                buffInstance = instance.Unwrap() as Buff;
                fieldInfos = buffInstance.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            }

            if (GUILayout.Button("Open"))
            {
                var filePath = EditorUtility.OpenFilePanel("Select buff.json",
                    $"{Application.dataPath}/Resources/Buffs", "json");
                OpenBuffJson(filePath);
            }

            EditorGUILayout.EndHorizontal();

            if (buffInstance != null)
            {
                GlobalEditorHelper.DrawFields(fieldInfos, buffInstance, scrollViewValue, true);
                var icon = buffInstance.spriteIcon;
                if (icon != null)
                {
                    buffInstance.spriteIconPath =
                        $"{Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(icon))}+{icon.name}";
                }

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

            #region Recent Buffs

            EditorGUILayout.BeginVertical();
            using (var v = new EditorGUILayout.VerticalScope(GUI.skin.label))
            {
                recentBuffsScrollView = EditorGUILayout.BeginScrollView(recentBuffsScrollView);
                EditorGUILayout.LabelField("Recent:");
                EditorGUILayout.Space(10f);
                for (int i = 0; i < recentBuffsList.Count; i++)
                {
                    if (GUILayout.Button(recentBuffsList[i]))
                    {
                        if (buffInstance != null)
                            SaveToJSON();
                        var filePath = $"{Application.dataPath}/Resources/Buffs/{recentBuffsList[i]}.json";
                        OpenBuffJson(filePath);
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
            string description = buffInstance.description;
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

            string dispellableColorString = $"<color=#{ColorUtility.ToHtmlStringRGB(Colors.dispellableColor)}>";
            string undispellableColorString = $"<color=#{ColorUtility.ToHtmlStringRGB(Colors.undispellableColor)}>";
            string targetColorString = buffInstance.isDispellable ? dispellableColorString : undispellableColorString;
            string targetDispellableString = buffInstance.isDispellable ? "(Dispellable)" : "(Not Dispellable)";
            string dispellableString = $" {targetColorString}{targetDispellableString}</color>";
            description += dispellableString;

            buffInstance.description = description;
        }
    }
}