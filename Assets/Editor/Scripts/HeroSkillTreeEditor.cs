using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEditor;
using UnityEngine;

namespace AgeOfHeroes.Editor
{
    public class HeroSkillTreeEditor
    {
        private HeroSkillTreeObject _heroSkillTreeObject;
        private Vector2 _scrollPosition;
        private int _indexer;

        public void DrawGUI()
        {
            GUIStyle removeBtnStyle = new GUIStyle(GUI.skin.button);
            removeBtnStyle.normal.textColor = Color.red;
            GUIStyle addSkillBtnStyle = new GUIStyle(GUI.skin.button);
            addSkillBtnStyle.normal.textColor = Color.green;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create"))
            {
                _indexer = 0;
                _heroSkillTreeObject = new HeroSkillTreeObject();
            }

            if (GUILayout.Button("Load"))
            {
                var filePath = EditorUtility.OpenFilePanel("Open Skill Tree",
                    $"{Application.dataPath}/Resources/SkillTrees", "json");
                var fileContents = File.ReadAllText(filePath);
                var jsonSettings = GlobalVariables.GetDefaultSerializationSettings();
                jsonSettings.TypeNameHandling = TypeNameHandling.All;
                var json = JsonConvert.DeserializeObject<HeroSkillTreeObject>(fileContents, jsonSettings);
                _heroSkillTreeObject = json;
                _indexer = _heroSkillTreeObject.HeroSkills.Keys.Count;
            }

            GUILayout.EndHorizontal();

            if (_heroSkillTreeObject == null)
                return;
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            _heroSkillTreeObject.internalName =
                EditorGUILayout.TextField("Internal Name", _heroSkillTreeObject.internalName, GUILayout.MaxWidth(300f));
            var keys = _heroSkillTreeObject.HeroSkills.Keys;
            foreach (var key in keys)
            {
                EditorGUILayout.LabelField($"Tier: {key}");
                var c = _heroSkillTreeObject.HeroSkills[key].Count;
                for (int i = 0; i < c; i++)
                {
                    GUILayout.BeginHorizontal();
                    _heroSkillTreeObject.HeroSkills[key][i] =
                        EditorGUILayout.TextField($"{i + 1}) Skill ", _heroSkillTreeObject.HeroSkills[key][i], GUILayout.MaxWidth(300f));
                    if (GUILayout.Button("-", removeBtnStyle, GUILayout.MaxWidth(64f)))
                    {
                        _heroSkillTreeObject.HeroSkills[key].RemoveAt(i);
                    }

                    GUILayout.EndHorizontal();
                }

                if (GUILayout.Button("+", addSkillBtnStyle, GUILayout.MaxWidth(64f)))
                {
                    _heroSkillTreeObject.HeroSkills[key].Add(string.Empty);
                }


                GUILayout.Space(5f);
            }

            GUILayout.Space(15f);

            if (GUILayout.Button("+"))
            {
                _indexer++;
                _heroSkillTreeObject.HeroSkills.TryAdd(_indexer, new List<string>());
            }

            if (GUILayout.Button("-", removeBtnStyle))
            {
                _heroSkillTreeObject.HeroSkills.Remove(_indexer--);
            }

            if (GUILayout.Button("Save"))
            {
                Save();
            }

            GUILayout.EndScrollView();
        }

        private void Save()
        {
            var jsonSettings = GlobalVariables.GetDefaultSerializationSettings();
            jsonSettings.TypeNameHandling = TypeNameHandling.All;
            var json = JsonConvert.SerializeObject(_heroSkillTreeObject, jsonSettings);
            File.WriteAllText($"{Application.dataPath}/Resources/SkillTrees/{_heroSkillTreeObject.internalName}.json",
                json);
            AssetDatabase.Refresh();
        }
    }
}