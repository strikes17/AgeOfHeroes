using UnityEditor;
using UnityEditor.SceneManagement;

namespace AgeOfHeroes.Editor
{
    public static class EditorSceneLoader
    {
        [MenuItem("Ilya/Open Scene/main_menu")]
        public static void OpenMainMenuScene()
        {
            var scenePath = "Assets/Scenes/main_menu.unity";
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }

        [MenuItem("Ilya/Open Scene/map_editor")]
        public static void OpenMapEditorScene()
        {
            var scenePath = "Assets/Scenes/map_editor.unity";
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }

        [MenuItem("Ilya/Open Scene/game")]
        public static void OpenGameScene()
        {
            var scenePath = "Assets/Scenes/game.unity";
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }
    }
}