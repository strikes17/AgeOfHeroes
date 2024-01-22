using System;
using System.Reflection;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;

namespace AgeOfHeroes.Editor
{
    public class CastlesAndBuildingsEditorWindow : EditorWindow
    {
        [MenuItem("Ilya/Catles And Buildings Editor")]
        private static void Init()
        {
            CastlesAndBuildingsEditorWindow window = (CastlesAndBuildingsEditorWindow) EditorWindow.GetWindow(typeof(CastlesAndBuildingsEditorWindow));
            window.titleContent.text = "Buildings & Castles";
            window.titleContent.image = EditorResources.Load<Texture2D>("icons/castle_editor_icon.png");
            window.Show();
        }

        private int _modeSwitcher = 0;
        private CastleEditor _castleEditor;
        private BuildingEditor _buildingEditor;
        
        private void OnEnable()
        {
            _castleEditor = new CastleEditor();
            _buildingEditor = new BuildingEditor();
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

            if (GUILayout.Button("Castle Editor", _modeSwitcher == 0 ? pressedButton : defaultButton))
            {
                _modeSwitcher = 0;
            }

            if (GUILayout.Button("Building Editor", _modeSwitcher == 1 ? pressedButton : defaultButton))
            {
                _modeSwitcher = 1;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(20f);
            switch (_modeSwitcher)
            {
                case 0:
                    _castleEditor.DrawGUI();
                    break;
                case 1:
                    _buildingEditor.DrawGUI();
                    break;
            }

            EditorGUILayout.EndVertical();
        }
    }
}