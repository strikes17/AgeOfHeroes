using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AgeOfHeroes.Spell;
using Ludiq.OdinSerializer.Utilities;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace AgeOfHeroes.Editor
{
    public class HeroSkillEditor
    {
        private List<string> heroSkillClasses, heroSkillFullNameClasses;
        private string[] heroSkillClassesEnumStrings;
        private int selectedSkillType = 0;
        private HeroSkill heroSkillInstance;
        private FieldInfo[] fieldInfos;
        private JsonSerializerSettings settings = new JsonSerializerSettings();
        private Vector2 scrollViewValue, recentHeroSkillsScrollView;
        private List<string> recentHeroSkillsList;

        public HeroSkillEditor()
        {
            settings.TypeNameHandling = TypeNameHandling.All;
            settings.Formatting = Formatting.Indented;
            heroSkillClasses = new List<string>();
            heroSkillFullNameClasses = new List<string>();
            var targetAssembly = Assembly.GetAssembly(typeof(HeroSkill));
            var allTypesInAssembly = targetAssembly.GetTypes();
            var allHeroSkillsTypes = allTypesInAssembly.Where(x =>
            {
                return x.IsSubclassOf(typeof(HeroSkill)) && !x.IsAbstract;
            }).ToList();
            allHeroSkillsTypes.ForEach(x =>
            {
                heroSkillClasses.Add(x.Name);
                heroSkillFullNameClasses.Add(x.FullName);
            });

            heroSkillClassesEnumStrings = heroSkillClasses.ToArray();

            UpdateRecentHeroSkillsList();
        }

        public void OpenHeroSkillJson(string filePath)
        {
            var fileContents = File.ReadAllText(filePath);
            var json = JsonConvert.DeserializeObject<HeroSkill>(fileContents, settings);
            heroSkillInstance = json;
            fieldInfos = heroSkillInstance.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            if (string.IsNullOrEmpty(heroSkillInstance.spriteIconPath))
                return;
            var path = heroSkillInstance.spriteIconPath.Split('+');
            var res = Resources.LoadAll<Sprite>($"Sprites/{path[0]}");
            res.ForEach(x =>
            {
                if (x.name == path[1])
                {
                    heroSkillInstance.spriteIcon = x;
                    return;
                }
            });
            var skillTypeName = heroSkillInstance.GetType().ToString().Replace("AgeOfHeroes.", string.Empty);
            List<string> list = new List<string>();
            foreach (var enumString in heroSkillClassesEnumStrings) list.Add(enumString);
            selectedSkillType = list.IndexOf(skillTypeName);
        }

        private void UpdateRecentHeroSkillsList()
        {
            recentHeroSkillsList = new List<string>();
            var heroSkillsDirInfo = new DirectoryInfo($"{Application.dataPath}/Resources/SkillTrees/Skills");
            var allHeroSkillsFiles = heroSkillsDirInfo.GetFiles("*.json", SearchOption.AllDirectories);
            recentHeroSkillsList.AddRange(allHeroSkillsFiles.Select(x => Path.GetFileNameWithoutExtension(x.Name))
                .ToList());
        }

        private void SaveToJSON()
        {
            if (string.IsNullOrEmpty(heroSkillInstance.internalName))
            {
                EditorUtility.DisplayDialog("Ошибка", "Заполните internalName!", "ок");
                return;
            }

            var heroSkillJson = JsonConvert.SerializeObject(heroSkillInstance, settings);
            File.WriteAllText(
                $"{Application.dataPath}/Resources/SkillTrees/Skills/{heroSkillInstance.internalName}.json",
                heroSkillJson);
            UpdateRecentHeroSkillsList();
            AssetDatabase.Refresh();
        }

        public void DrawGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();

            selectedSkillType = EditorGUILayout.Popup("Skill Type", selectedSkillType, heroSkillClassesEnumStrings);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create"))
            {
                var spellTypeString = heroSkillFullNameClasses[selectedSkillType];
                var instance = Activator.CreateInstance("AgeOfHeroes", spellTypeString);
                heroSkillInstance = instance.Unwrap() as HeroSkill;
                fieldInfos = heroSkillInstance.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            }

            if (GUILayout.Button("Open"))
            {
                var filePath = EditorUtility.OpenFilePanel("Select heroSkill.json",
                    $"{Application.dataPath}/Resources/HeroSkills", "json");
                OpenHeroSkillJson(filePath);
            }

            EditorGUILayout.EndHorizontal();

            if (heroSkillInstance != null)
            {
                GlobalEditorHelper.DrawFields(fieldInfos, heroSkillInstance, scrollViewValue, true);

                var icon = heroSkillInstance.spriteIcon;
                if (icon != null)
                {
                    heroSkillInstance.spriteIconPath =
                        $"{Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(icon))}+{icon.name}";
                }

                if (GUILayout.Button("Save To JSON"))
                {
                    SaveToJSON();
                }
            }

            EditorGUILayout.EndVertical();

            #region Recent HeroSkills

            EditorGUILayout.BeginVertical();
            using (var v = new EditorGUILayout.VerticalScope(GUI.skin.label))
            {
                recentHeroSkillsScrollView = EditorGUILayout.BeginScrollView(recentHeroSkillsScrollView);
                EditorGUILayout.LabelField("Recent:");
                EditorGUILayout.Space(10f);
                for (int i = 0; i < recentHeroSkillsList.Count; i++)
                {
                    if (GUILayout.Button(recentHeroSkillsList[i]))
                    {
                        if (heroSkillInstance != null)
                            SaveToJSON();
                        var filePath =
                            $"{Application.dataPath}/Resources/SkillTrees/Skills/{recentHeroSkillsList[i]}.json";
                        OpenHeroSkillJson(filePath);
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