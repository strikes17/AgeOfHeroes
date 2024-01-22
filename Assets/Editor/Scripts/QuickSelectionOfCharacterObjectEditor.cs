using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AgeOfHeroes;
using Ludiq.Peek;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class QuickSelectionOfCharacterObjectEditor : EditorWindow
{
    [MenuItem("Ilya/Toggle Unity UI")]
    public static void ToggleUnityUI()
    {
        EditorPrefs.SetBool("ui_hidden", !EditorPrefs.GetBool("ui_hidden", true));
        int layer = LayerMask.NameToLayer("UI");
        var gameObjects = GameObject.FindObjectsOfType<GameObject>();
        if (EditorPrefs.GetBool("ui_hidden"))
            SceneVisibilityManager.instance.Show(gameObjects.Where(x => x.layer == layer).ToArray(), true);
        else
            SceneVisibilityManager.instance.Hide(gameObjects.Where(x => x.layer == layer).ToArray(), true);
    }

    [MenuItem("Ilya/Select Character Object")]
    public static void SelectCharacterObject()
    {
        var selection = Selection.activeGameObject;
        if (selection == null) return;
        var charComponent = selection.GetComponent<Character>();
        if (charComponent != null)
        {
            Selection.activeObject = charComponent.CharacterObject;
            return;
        }

        var heroComponent = selection.GetComponent<Hero>();
        if (heroComponent != null)
        {
            Selection.activeObject = heroComponent.HeroObject;
        }
    }
}