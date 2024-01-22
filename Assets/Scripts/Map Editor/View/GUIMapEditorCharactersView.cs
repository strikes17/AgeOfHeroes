using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AgeOfHeroes.MapEditor
{
    public class GUIMapEditorCharactersView : MonoBehaviour
    {
        [SerializeField] private Transform _contentTransform;
        [SerializeField] private GUIMapEditorCharacterWidget _characterWidgetPrefab;

        private List<GUIMapEditorCharacterWidget> _charactersWidgets = new List<GUIMapEditorCharacterWidget>();

        private void Clear()
        {
            for (int i = 0; i < _charactersWidgets.Count; i++)
            {
                Destroy(_charactersWidgets[i].gameObject);
            }

            _charactersWidgets.Clear();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            Fraction lastSelectedFraction = MapEditorManager.Instance.SelectedFraction == Fraction.None ? Fraction.Human : MapEditorManager.Instance.SelectedFraction;
            UpdateGUI(lastSelectedFraction);
        }

        public void UpdateGUI(Fraction fraction)
        {
            Clear();
            var allCharacters = MapEditorDatabase.Instance.GetAllCharactersFromFraction(fraction);
            foreach (var character in allCharacters)
            {
                var characterWidgetInstance = GameObject.Instantiate(_characterWidgetPrefab, Vector3.zero, Quaternion.identity, _contentTransform);
                characterWidgetInstance.Set(character);
                _charactersWidgets.Add(characterWidgetInstance);
            }
        }

        public void Hide()
        {
            Clear();
            gameObject.SetActive(false);
        }
    }
}