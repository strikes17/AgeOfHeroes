using System;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;

namespace AgeOfHeroes.Editor
{
    public class HeroSkillTreeEditorWindow : EditorWindow
    {
        [MenuItem("Ilya/Hero Skill Tree Editor")]
        private static void Init()
        {
            HeroSkillTreeEditorWindow window =
                (HeroSkillTreeEditorWindow)EditorWindow.GetWindow(typeof(HeroSkillTreeEditorWindow));
            window.titleContent.text = "Hero Skill Tree";
            window.titleContent.image = EditorResources.Load<Texture2D>("icons/ca_editor_icon.png");
            window.Show();
        }

        private int _modeSwitcher;
        private HeroSkillTreeEditor _skillTreeEditor;
        private HeroSkillEditor _heroSkillEditor;

        private void OnEnable()
        {
            Setup();
        }

        private void Setup()
        {
            _skillTreeEditor = new HeroSkillTreeEditor();
            _heroSkillEditor = new HeroSkillEditor();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            GUIStyle pressedButton = new GUIStyle(GUI.skin.button);
            pressedButton.normal.textColor = Color.yellow;
            pressedButton.focused.textColor = Color.yellow;
            pressedButton.hover.textColor = Color.yellow;
            pressedButton.fontStyle = FontStyle.Bold;
            GUIStyle defaultButton = new GUIStyle(GUI.skin.button);

            if (GUILayout.Button("Skill Tree Editor", _modeSwitcher == 0 ? pressedButton : defaultButton))
            {
                _modeSwitcher = 0;
            }

            if (GUILayout.Button("Skills Editor", _modeSwitcher == 1 ? pressedButton : defaultButton))
            {
                _modeSwitcher = 1;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(20f);
            switch (_modeSwitcher)
            {
                case 0:
                    _skillTreeEditor.DrawGUI();
                    break;
                case 1:
                    _heroSkillEditor.DrawGUI();
                    break;
            }

            EditorGUILayout.EndVertical();
        }
    }
}