using System.Collections;
using System.Net;
using UnityEditor.Experimental;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AgeOfHeroes.Editor
{
    public class MagicSpellsEditorWindow : EditorWindow
    {
        [MenuItem("Ilya/Spells Editor")]
        private static void Init()
        {
            MagicSpellsEditorWindow window = (MagicSpellsEditorWindow) EditorWindow.GetWindow(typeof(MagicSpellsEditorWindow));
            window.titleContent.text = "Buffs & Magics";
            window.titleContent.image = EditorResources.Load<Texture2D>("icons/magic_editor_icon.png");
            window.Show();
        }

        private BuffsEditor _buffsEditor;
        private MagicSpellsEditor _magicSpellsEditor;
        private SpellBookEditor _spellBookEditor;

        private int _modeSwitcher = 0;

        private void OnEnable()
        {
            _buffsEditor = new BuffsEditor();
            _magicSpellsEditor = new MagicSpellsEditor();
            _spellBookEditor = new SpellBookEditor();
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

            if (GUILayout.Button("Buffs Editor", _modeSwitcher == 0 ? pressedButton : defaultButton))
            {
                _modeSwitcher = 0;
            }

            if (GUILayout.Button("Spells Editor", _modeSwitcher == 1 ? pressedButton : defaultButton))
            {
                _modeSwitcher = 1;
            }

            if (GUILayout.Button("Books Editor", _modeSwitcher == 2 ? pressedButton : defaultButton))
            {
                _modeSwitcher = 2;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(20f);
            switch (_modeSwitcher)
            {
                case 0:
                    _buffsEditor.DrawGUI();
                    break;
                case 1:
                    _magicSpellsEditor.DrawGUI();
                    break;
                case 2:
                    _spellBookEditor.DrawGUI();
                    break;
            }

            EditorGUILayout.EndVertical();
        }
    }
}