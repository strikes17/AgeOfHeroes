using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AgeOfHeroes.Spell;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using AgeOfHeroes.Spell;
using Ludiq.OdinSerializer.Utilities;
using Ludiq.PeekCore;
using UnityEditor.Experimental;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace AgeOfHeroes.Editor
{
    public class MagicSpellsEditor
    {
        private List<string> spellClasses, spellFullNameClasses;
        private string[] _spellClassesEnumStrings;
        private int _selectedSpellType = 0;
        private MagicSpellObject _magicSpellObjectInstance;
        private FieldInfo[] _fieldInfos;
        JsonSerializerSettings settings = new JsonSerializerSettings();
        public List<string> appliedNegativeBuffsOnCaster = new List<string>();
        public List<string> appliedPositiveBuffsOnTarget = new List<string>();
        public List<string> appliedPositiveBuffsOnCaster = new List<string>();
        public List<string> appliedNegativeBuffsOnTarget = new List<string>();
        public List<int> selectedAppliedPositiveBuffsOnCaster;
        public List<int> selectedAppliedNegativeBuffsOnCaster;
        public List<int> selectedAppliedPositiveBuffsOnTarget;
        public List<int> selectedAppliedNegativeBuffsOnTarget;
        public List<string> availableBuffs;
        public List<MagicSpellAllowedTarget> spellAllowedTargets;
        private Vector2 scrollViewValue, recentSpellsScrollView, scrollViewValue2;
        private List<string> recentSpellsList;

        public MagicSpellsEditor()
        {
            settings.TypeNameHandling = TypeNameHandling.All;
            settings.Formatting = Formatting.Indented;
            spellClasses = new List<string>();
            spellFullNameClasses = new List<string>();
            var targetAssembly = Assembly.GetAssembly(typeof(MagicSpellObject));
            var allTypesInAssembly = targetAssembly.GetTypes();
            var allBuffsTypes = allTypesInAssembly.Where(x => x.IsSubclassOf(typeof(MagicSpellObject)) && !x.IsAbstract)
                .ToList();
            allBuffsTypes.ForEach(x =>
            {
                spellClasses.Add(x.Name);
                spellFullNameClasses.Add(x.FullName);
            });

            _spellClassesEnumStrings = spellClasses.ToArray();
            UpdateRecentSpellsList();
        }

        private void UpdateRecentSpellsList()
        {
            recentSpellsList = new List<string>();
            var spellsDirInfo = new DirectoryInfo($"{Application.dataPath}/Resources/Magic Spells");
            var allSpellsFiles = spellsDirInfo.GetFiles("*.json", SearchOption.AllDirectories);
            recentSpellsList.AddRange(allSpellsFiles.Select(x => Path.GetFileNameWithoutExtension(x.Name)).ToList());
        }

        public void OpenSpellJson(string filePath)
        {
            var fileContents = File.ReadAllText(filePath);
            var json = JsonConvert.DeserializeObject<MagicSpellObject>(fileContents, settings);
            _magicSpellObjectInstance = json;
            appliedNegativeBuffsOnCaster = _magicSpellObjectInstance.appliedNegativeBuffsOnCaster;
            appliedPositiveBuffsOnTarget = _magicSpellObjectInstance.appliedPositiveBuffsOnTarget;
            appliedPositiveBuffsOnCaster = _magicSpellObjectInstance.appliedPositiveBuffsOnCaster;
            appliedNegativeBuffsOnTarget = _magicSpellObjectInstance.appliedNegativeBuffsOnTarget;
            spellAllowedTargets = new List<MagicSpellAllowedTarget>();
            _fieldInfos = _magicSpellObjectInstance.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            UpdateBuffs();
            appliedNegativeBuffsOnCaster.ForEach(x =>
                selectedAppliedNegativeBuffsOnCaster.Add(availableBuffs.IndexOf(x)));
            appliedPositiveBuffsOnTarget.ForEach(x =>
                selectedAppliedPositiveBuffsOnTarget.Add(availableBuffs.IndexOf(x)));
            appliedPositiveBuffsOnCaster.ForEach(x =>
                selectedAppliedPositiveBuffsOnCaster.Add(availableBuffs.IndexOf(x)));
            appliedNegativeBuffsOnTarget.ForEach(x =>
                selectedAppliedNegativeBuffsOnTarget.Add(availableBuffs.IndexOf(x)));
            List<long> allowedTargetsFullList = new List<long>();
            var allowedTargetsNames = (List<MagicSpellAllowedTarget>)Enum.GetValues(typeof(MagicSpellAllowedTarget))
                .ConvertTo(typeof(List<MagicSpellAllowedTarget>));
            foreach (var value in allowedTargetsNames)
            {
                long longValue = (long)value;
                if ((longValue & (long)_magicSpellObjectInstance.allowedTarget) != 0)
                {
                    spellAllowedTargets.Add(value);
                }
            }

            long allowedTargetsMask = _magicSpellObjectInstance.allowedTarget;

            var path = _magicSpellObjectInstance.iconName.Split('+');
            var res = Resources.LoadAll<Sprite>($"Sprites/{path[0]}").ToList();
            res.ForEach(x =>
            {
                if (x.name == path[1])
                {
                    _magicSpellObjectInstance.Icon = x;
                    return;
                }
            });
        }

        public void UpdateBuffs()
        {
            availableBuffs = new List<string>();
            selectedAppliedPositiveBuffsOnCaster = new List<int>();
            selectedAppliedNegativeBuffsOnTarget = new List<int>();
            selectedAppliedNegativeBuffsOnCaster = new List<int>();
            selectedAppliedPositiveBuffsOnTarget = new List<int>();
            var buffsDirInfo = new DirectoryInfo($"{Application.dataPath}/Resources/Buffs");
            var allBuffsFiles = buffsDirInfo.GetFiles("*.json", SearchOption.AllDirectories);
            availableBuffs.AddRange(allBuffsFiles.Select(x => Path.GetFileNameWithoutExtension(x.Name)));
        }

        public void GUIBuffsList(string label, ref List<int> buffsIndexer, ref List<string> buffsList)
        {
            GUIStyle helpBtnStyle = new GUIStyle(GUI.skin.button);
            helpBtnStyle.normal.textColor = Color.green;
            helpBtnStyle.fontStyle = FontStyle.Bold;

            EditorGUILayout.LabelField(label);
            EditorGUILayout.Space(2f);
            for (int i = 0; i < buffsList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"{i.ToString()}.", GUILayout.MaxWidth(16f));

                buffsIndexer[i] = EditorGUILayout.Popup(buffsIndexer[i], availableBuffs.ToArray());
                buffsList[i] = availableBuffs[buffsIndexer[i]];
                if (GUILayout.Button("-"))
                {
                    buffsIndexer.RemoveAt(i);
                    buffsList.RemoveAt(i);
                }

                if (GUILayout.Button("?", helpBtnStyle))
                {
                    var buffFile = File.ReadAllText($"{Application.dataPath}/Resources/Buffs/{buffsList[i]}.json");
                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.TypeNameHandling = TypeNameHandling.All;
                    var buff = JsonConvert.DeserializeObject<Buff>(buffFile, settings);
                    EditorUtility.DisplayDialog(buff.title, buff.description, "ok");
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+"))
            {
                buffsIndexer.Add(0);
                buffsList.Add(string.Empty);
            }

            EditorGUILayout.Space(10f);
        }

        private void SaveToJSON()
        {
            if (string.IsNullOrEmpty(_magicSpellObjectInstance.internalName))
            {
                EditorUtility.DisplayDialog("Ошибка", "Заполните internalName!", "ок");
                return;
            }

            _magicSpellObjectInstance.appliedNegativeBuffsOnCaster = appliedNegativeBuffsOnCaster;
            _magicSpellObjectInstance.appliedPositiveBuffsOnTarget = appliedPositiveBuffsOnTarget;
            _magicSpellObjectInstance.appliedPositiveBuffsOnCaster = appliedPositiveBuffsOnCaster;
            _magicSpellObjectInstance.appliedNegativeBuffsOnTarget = appliedNegativeBuffsOnTarget;
            long mask = 0;
            foreach (var allowedTarget in spellAllowedTargets)
            {
                mask |= (long)allowedTarget;
            }

            _magicSpellObjectInstance.allowedTarget = mask;
            var magicSpellJson = JsonConvert.SerializeObject(_magicSpellObjectInstance, settings);
            File.WriteAllText(
                $"{Application.dataPath}/Resources/Magic Spells/{_magicSpellObjectInstance.internalName}.json",
                magicSpellJson);
            UpdateRecentSpellsList();
            AssetDatabase.Refresh();
        }

        public void DrawGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();

            scrollViewValue = EditorGUILayout.BeginScrollView(scrollViewValue);
            _selectedSpellType = EditorGUILayout.Popup("Spell Type", _selectedSpellType, _spellClassesEnumStrings);
            EditorGUILayout.Space(8f);

            string spellType = _magicSpellObjectInstance == null
                ? _spellClassesEnumStrings[_selectedSpellType]
                : _magicSpellObjectInstance.GetType().ToString();
            EditorGUILayout.LabelField($"Spell Type: {spellType}");
            EditorGUILayout.Space(2f);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create"))
            {
                var spellTypeString = spellFullNameClasses[_selectedSpellType];
                var instance = Activator.CreateInstance("AgeOfHeroes", spellTypeString);
                _magicSpellObjectInstance = instance.Unwrap() as MagicSpellObject;
                appliedPositiveBuffsOnCaster = new List<string>();
                appliedNegativeBuffsOnTarget = new List<string>();
                appliedNegativeBuffsOnCaster = new List<string>();
                appliedPositiveBuffsOnTarget = new List<string>();
                spellAllowedTargets = new List<MagicSpellAllowedTarget>();
                _fieldInfos = _magicSpellObjectInstance.GetType()
                    .GetFields(BindingFlags.Public | BindingFlags.Instance);
                UpdateBuffs();
            }

            if (GUILayout.Button("Open"))
            {
                var filePath = EditorUtility.OpenFilePanel("Select MagicSpell.json",
                    $"{Application.dataPath}/Resources/Magic Spells", "json");
                OpenSpellJson(filePath);
            }

            EditorGUILayout.EndHorizontal();

            if (_magicSpellObjectInstance != null)
            {
                GlobalEditorHelper.DrawFields(_fieldInfos, _magicSpellObjectInstance, scrollViewValue2, true);

                EditorGUILayout.BeginVertical();
                for (int i = 0; i < spellAllowedTargets.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    spellAllowedTargets[i] =
                        (MagicSpellAllowedTarget)EditorGUILayout.EnumPopup(
                            (MagicSpellAllowedTarget)spellAllowedTargets[i]);
                    if (GUILayout.Button("-"))
                    {
                        spellAllowedTargets.RemoveAt(i);
                    }

                    EditorGUILayout.EndHorizontal();
                }

                if (GUILayout.Button("+"))
                {
                    spellAllowedTargets.Add(MagicSpellAllowedTarget.Alive);
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.Space(15f);
                var redBtnStyle = new GUIStyle(GUI.skin.button);
                redBtnStyle.normal.textColor = Color.red;
                if (GUILayout.Button("Update Buffs", redBtnStyle))
                {
                    UpdateBuffs();
                }

                var icon = _magicSpellObjectInstance.Icon;
                if (icon != null)
                {
                    _magicSpellObjectInstance.iconName =
                        $"{Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(icon))}+{icon.name}";
                }

                if (availableBuffs.Count > 0)
                {
                    EditorGUILayout.Space(4f);

                    GUIBuffsList("Caster Positive Buffs:", ref selectedAppliedPositiveBuffsOnCaster,
                        ref appliedPositiveBuffsOnCaster);
                    GUIBuffsList("Caster Negative Buffs:", ref selectedAppliedNegativeBuffsOnCaster,
                        ref appliedNegativeBuffsOnCaster);
                    GUIBuffsList("Target Positive Buffs:", ref selectedAppliedPositiveBuffsOnTarget,
                        ref appliedPositiveBuffsOnTarget);
                    GUIBuffsList("Target Negative Buffs:", ref selectedAppliedNegativeBuffsOnTarget,
                        ref appliedNegativeBuffsOnTarget);

                    if (GUILayout.Button("Save To JSON", redBtnStyle))
                    {
                        SaveToJSON();
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("No buffs were found! Please create some at buffs editor");
                }
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            #region Recent Spells

            EditorGUILayout.BeginVertical();
            using (var v = new EditorGUILayout.VerticalScope(GUI.skin.label))
            {
                recentSpellsScrollView = EditorGUILayout.BeginScrollView(recentSpellsScrollView);
                EditorGUILayout.LabelField("Recent:");
                EditorGUILayout.Space(10f);
                for (int i = 0; i < recentSpellsList.Count; i++)
                {
                    if (GUILayout.Button(recentSpellsList[i]))
                    {
                        if (_magicSpellObjectInstance != null)
                        {
                            if (string.IsNullOrEmpty(_magicSpellObjectInstance.internalName))
                                _magicSpellObjectInstance.internalName = $"new_spell_{Random.Range(0, 2000)}";
                            SaveToJSON();
                        }

                        var filePath = $"{Application.dataPath}/Resources/Magic Spells/{recentSpellsList[i]}.json";
                        OpenSpellJson(filePath);
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