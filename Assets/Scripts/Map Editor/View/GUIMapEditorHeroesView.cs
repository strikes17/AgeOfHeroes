using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes.MapEditor
{
    public class GUIMapEditorHeroesView : MonoBehaviour
    {
        [SerializeField] private Transform _contentTransform;
        [SerializeField] private GUIMapEditorHeroWidget _heroWidgetPrefab;

        private List<GUIMapEditorHeroWidget> _heroesWidgets = new List<GUIMapEditorHeroWidget>();

        private void Clear()
        {
            for (int i = 0; i < _heroesWidgets.Count; i++)
            {
                Destroy(_heroesWidgets[i].gameObject);
            }

            _heroesWidgets.Clear();
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
            var allHeroes = MapEditorDatabase.Instance.GetAllHeroesFromFraction(fraction);
            foreach (var hero in allHeroes)
            {
                var heroWidgetInstance = GameObject.Instantiate(_heroWidgetPrefab, Vector3.zero, Quaternion.identity, _contentTransform);
                heroWidgetInstance.Set(hero);
                _heroesWidgets.Add(heroWidgetInstance);
            }
        }

        public void Hide()
        {
            Clear();
            gameObject.SetActive(false);
        }
    }
}