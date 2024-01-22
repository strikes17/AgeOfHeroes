using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes
{
    public class GUINecromanceryRaiseDialogueWindow : GUICommonDialogueWindow
    {
        [SerializeField] private Transform _contentRoot;
        private List<GUINecroRaiseWidget> _widgets = new List<GUINecroRaiseWidget>();
        private GUINecroRaiseWidget _selectedWidget;

        public GUINecroRaiseWidget SelectedWidget => _selectedWidget;

        protected void Clear()
        {
            for (int i = 0; i < _widgets.Count; i++)
            {
                Destroy(_widgets[i]);
            }
            _widgets.Clear();
        }
        
        public void SetVariants(List<string> variants)
        {
            _okButton.gameObject.SetActive(false);
            Clear();
            var prefab = ResourcesBase.GetPrefab("GUI/Necro Raise Widget").GetComponent<GUINecroRaiseWidget>();
            foreach (var variant in variants)
            {
                var characterObject = ResourcesBase.GetCharacterObject(variant);
                var widget = Instantiate(prefab, Vector3.zero, Quaternion.identity, _contentRoot);
                widget.CharacterObject = characterObject;
                widget.SelectionButton.onClick.AddListener((() =>
                {
                    _selectedWidget = widget;
                    for (var i = 0; i < _widgets.Count; i++)
                    {
                        _widgets[i].Deselect();
                    }
                    _selectedWidget.Select();
                    _okButton.gameObject.SetActive(true);
                }));
                _widgets.Add(widget);
            }
        }
    }
}