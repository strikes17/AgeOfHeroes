using System.Collections.Generic;
using AgeOfHeroes.MapEditor;
using UnityEngine;

namespace AgeOfHeroes
{
    public static class GUIDialogueFactory
    {
        public static Transform canvasRoot;

        private static Dictionary<int, GUIDialogueWindow> _dialogues = new Dictionary<int, GUIDialogueWindow>();

        static GUIDialogueFactory()
        {
            canvasRoot = GameObject.FindWithTag("CanvasMain").transform;
        }

        public static GUIMapEditorMarkerInfoWindow CreateMapEditorMarkerInfoWindow(int id)
        {
            if (_dialogues.ContainsKey(id)) return null;
            canvasRoot = GameObject.FindWithTag("CanvasMain").transform;
            var mapEditorMarkerInfoWindow = ResourcesBase.GetPrefab("Map Editor Prefabs/Map Editor Marker Info Window")
                .GetComponent<GUIMapEditorMarkerInfoWindow>();
            var instance = GameObject.Instantiate(mapEditorMarkerInfoWindow);
            instance.transform.SetParent(canvasRoot);
            instance.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            instance.Closed += () => _dialogues.Remove(id);
            _dialogues.Add(id, instance);
            return instance;
        }

        public static GUINecromanceryRaiseDialogueWindow CreateNecromanceryDialogueWindow()
        {
             var necromanceryRaiseDialogueWindow =
                ResourcesBase.GetPrefab("GUI/Necromancery Raise Dialogue").GetComponent<GUINecromanceryRaiseDialogueWindow>();
            var instance = GameObject.Instantiate(necromanceryRaiseDialogueWindow);
            instance.transform.SetParent(canvasRoot);
            instance.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            instance.Closed += () => _dialogues.Remove(instance.GetHashCode());
            _dialogues.Add(instance.GetHashCode(), instance);
            return instance;
        }

        public static GUICommonDialogueWindow CreateCommonDialogueWindow()
        {
            GUICommonDialogueWindow commonDialogueWindow =
                ResourcesBase.GetPrefab("GUI/Common Dialogue").GetComponent<GUICommonDialogueWindow>();
            var instance = GameObject.Instantiate(commonDialogueWindow);
            instance.transform.SetParent(canvasRoot);
            instance.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            instance.Closed += () => _dialogues.Remove(instance.GetHashCode());
            _dialogues.Add(instance.GetHashCode(), instance);
            return instance;
        }

        public static GUILearnRandomSpellDialogueWindow CreateLearnRandomSpellDialogue()
        {
            GUILearnRandomSpellDialogueWindow guiLearnRandomSpellDialogueWindow = ResourcesBase
                .GetPrefab("GUI/Learn Random Spell Widget").GetComponent<GUILearnRandomSpellDialogueWindow>();
            var instance = GameObject.Instantiate(guiLearnRandomSpellDialogueWindow);
            instance.transform.SetParent(canvasRoot);
            instance.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            instance.Closed += () => _dialogues.Remove(instance.GetHashCode());
            _dialogues.Add(instance.GetHashCode(), instance);
            return instance;
        }

        public static GUISpecialBuildingInfoWindow CreateSpecialBuildingInfoWindow(int instanceId)
        {
            GUIDialogueWindow dialogue = null;
            if (_dialogues.TryGetValue(instanceId, out dialogue))
            {
                var specialBuildingDialogue = (GUISpecialBuildingInfoWindow)dialogue;
                return specialBuildingDialogue;
            }

            var specialBuildingDialogue2 = ResourcesBase.GetPrefab("GUI/Special Building Info Window")
                .GetComponent<GUISpecialBuildingInfoWindow>();
            var instance = GameObject.Instantiate(specialBuildingDialogue2);
            instance.transform.SetParent(canvasRoot);
            var rectTransform = instance.GetComponent<RectTransform>();
            rectTransform.anchoredPosition =
                new Vector2(-rectTransform.rect.width / 2f, rectTransform.rect.height / 2f);
            _dialogues.TryAdd(instanceId, instance);
            return specialBuildingDialogue2;
        }

        public static GUISpecialBuildingDialogue CreateSpecialBuildingDialogue(int instanceId)
        {
            GUIDialogueWindow dialogue = null;
            if (_dialogues.TryGetValue(instanceId, out dialogue))
            {
                var specialBuildingDialogue = (GUISpecialBuildingDialogue)dialogue;
                return specialBuildingDialogue;
            }

            var specialBuildingDialogue2 = ResourcesBase.GetPrefab("GUI/Special Building Dialogue")
                .GetComponent<GUISpecialBuildingDialogue>();
            var instance = GameObject.Instantiate(specialBuildingDialogue2);
            instance.transform.SetParent(canvasRoot);
            var rectTransform = instance.GetComponent<RectTransform>();
            rectTransform.anchoredPosition =
                new Vector2(-rectTransform.rect.width / 2f, rectTransform.rect.height / 2f);
            _dialogues.TryAdd(instanceId, instance);
            return specialBuildingDialogue2;
        }

        public static GUIBuildFailedDialogue CreateBuildFailedDialogue()
        {
            GUIBuildFailedDialogue guiBuildFailedDialogue =
                ResourcesBase.GetPrefab("GUI/Build Failed").GetComponent<GUIBuildFailedDialogue>();
            var instance = GameObject.Instantiate(guiBuildFailedDialogue);
            instance.transform.SetParent(canvasRoot);
            var rectTransform = instance.GetComponent<RectTransform>();
            rectTransform.anchoredPosition =
                new Vector2(-rectTransform.rect.width / 2f, rectTransform.rect.height / 2f);
            // _dialogues.Add(instance.GetHashCode(), instance);
            return instance;
        }

        public static GUICollectArtifactDialogue CreateCollectArtifactDialogue()
        {
            GUICollectArtifactDialogue guiCollectArtifactDialogue = ResourcesBase.GetPrefab("GUI/Artifact Dialogue")
                .GetComponent<GUICollectArtifactDialogue>();
            var instance = GameObject.Instantiate(guiCollectArtifactDialogue);
            instance.transform.SetParent(canvasRoot);
            instance.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            // _dialogues.Add(instance.GetHashCode(), instance);
            return instance;
        }

        public static GUIChestTreasureDialogue CreateBasicChestTreasureDialogue()
        {
            GUIChestTreasureDialogue guiChestTreasureDialogue = ResourcesBase.GetPrefab("GUI/Chest Treasure Dialogue")
                .GetComponent<GUIChestTreasureDialogue>();
            var instance = GameObject.Instantiate(guiChestTreasureDialogue);
            instance.transform.SetParent(canvasRoot);
            instance.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            // _dialogues.Add(instance.GetHashCode(), instance);
            return instance;
        }

        public static GUITreasureDialog CreateTreasureDialogue()
        {
            GUITreasureDialog treasureDialogue =
                ResourcesBase.GetPrefab("GUI/Treasure Dialogue").GetComponent<GUITreasureDialog>();
            var instance = GameObject.Instantiate(treasureDialogue);
            instance.transform.SetParent(canvasRoot);
            instance.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            // _dialogues.Add(instance.GetHashCode(), instance);
            return instance;
        }

        public static GUIBuyCharacterDialogue CreateBuyCharacterDialogue()
        {
            GUIBuyCharacterDialogue treasureDialogue = ResourcesBase.GetPrefab("GUI/Buy Character Dialogue")
                .GetComponent<GUIBuyCharacterDialogue>();
            var instance = GameObject.Instantiate(treasureDialogue);
            instance.transform.SetParent(canvasRoot);
            instance.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            // _dialogues.Add(instance.GetHashCode(), instance);
            return instance;
        }
    }
}