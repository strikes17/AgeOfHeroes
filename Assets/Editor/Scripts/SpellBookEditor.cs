using System.Collections.Generic;
using System.IO;
using System.Linq;
using AgeOfHeroes.Spell;
using Ludiq.PeekCore;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace AgeOfHeroes.Editor
{
    public class SpellBookEditor
    {
        public SpellBookObject _spellBookObject;
        private Vector2 scrollViewValue;
        private List<string> availableSpells;
        private string[] availableSpellsNames;
        private List<int> selectedSpellIndexes;
        private JsonSerializerSettings _jsonSerializerSettings;

        public SpellBookEditor()
        {
            _jsonSerializerSettings = new JsonSerializerSettings();
            _jsonSerializerSettings.TypeNameHandling = TypeNameHandling.All;
            _jsonSerializerSettings.Formatting = Formatting.Indented;
        }

        public void UpdateSpells()
        {
            availableSpells = new List<string>();
            string spellsPath = $"{Application.dataPath}/Resources/Magic Spells";
            DirectoryInfo spellsDirectoryInfo = new DirectoryInfo(spellsPath);
            var allSpellsFiles = spellsDirectoryInfo.GetFiles("*.json", SearchOption.AllDirectories);
            availableSpells.AddRange(allSpellsFiles.Select(x => Path.GetFileNameWithoutExtension(x.Name)).ToList());
            availableSpellsNames = availableSpells.ToArray();
        }

        public void DrawGUI()
        {
            scrollViewValue = EditorGUILayout.BeginScrollView(scrollViewValue);
            EditorGUILayout.Space(10f);

            if (GUILayout.Button("Update spells"))
            {
                UpdateSpells();
            }

            EditorGUILayout.Space(5f);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Create New"))
            {
                _spellBookObject = new SpellBookObject();
                selectedSpellIndexes = new List<int>();
                UpdateSpells();
            }

            if (GUILayout.Button("Open"))
            {
                UpdateSpells();
                var jsonPath = EditorUtility.OpenFilePanel("Open Spell Book Json", $"{Application.dataPath}/Resources/Spell Books", "json");
                var jsonContents = File.ReadAllText(jsonPath);
                var jsonSpellbook = JsonConvert.DeserializeObject<SpellBookObject>(jsonContents);
                _spellBookObject = jsonSpellbook;
                selectedSpellIndexes = new List<int>();
                _spellBookObject.spells.ForEach(x => selectedSpellIndexes.Add(availableSpells.IndexOf(x)));
            }

            EditorGUILayout.EndHorizontal();

            GUIStyle helpBtnStyle = new GUIStyle(GUI.skin.button);
            helpBtnStyle.normal.textColor = Color.green;
            helpBtnStyle.fontStyle = FontStyle.Bold;

            if (_spellBookObject != null)
            {
                EditorGUILayout.LabelField("Internal Name");
                _spellBookObject.internalName = EditorGUILayout.TextField(_spellBookObject.internalName);
                EditorGUILayout.LabelField("Spells:");
                for (int i = 0; i < _spellBookObject.spells.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"{i.ToString()}.", GUILayout.MaxWidth(16f));
                    selectedSpellIndexes[i] = EditorGUILayout.Popup(selectedSpellIndexes[i], availableSpellsNames);
                    _spellBookObject.spells[i] = availableSpellsNames[selectedSpellIndexes[i]];
                    if (GUILayout.Button("-"))
                    {
                        selectedSpellIndexes.RemoveAt(i);
                        _spellBookObject.spells.RemoveAt(i);
                    }

                    if (GUILayout.Button("?", helpBtnStyle))
                    {
                        var spellFile = File.ReadAllText($"{Application.dataPath}/Resources/Magic Spells/{_spellBookObject.spells[i]}.json");
                        var magicSpell = JsonConvert.DeserializeObject<MagicSpellObject>(spellFile, _jsonSerializerSettings);
                        EditorUtility.DisplayDialog(magicSpell.title, magicSpell.description, "ok");
                    }

                    EditorGUILayout.EndHorizontal();
                }

                if (GUILayout.Button("+"))
                {
                    selectedSpellIndexes.Add(0);
                    _spellBookObject.spells.Add(string.Empty);
                }

                EditorGUILayout.Space(8f);

                if (GUILayout.Button("Save"))
                {
                    if (string.IsNullOrEmpty(_spellBookObject.internalName))
                    {
                        EditorUtility.DisplayDialog("Ошибка", "Заполните internalName!", "ок");
                        return;
                    }
                    var json = JsonConvert.SerializeObject(_spellBookObject, _jsonSerializerSettings);
                    File.WriteAllText($"{Application.dataPath}/Resources/Spell Books/{_spellBookObject.internalName}.json", json);
                    AssetDatabase.Refresh();
                }
            }

            EditorGUILayout.EndScrollView();
        }
    }
}