using System;
using System.Collections.Generic;
using AgeOfHeroes.Spell;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUILearnRandomSpellDialogueWindow : GUIDialogueWindow
    {
        [SerializeField] private GUILearnSpellWidgetButton _widgetPrefab;
        [SerializeField] private GameObject _spellDescriptionObject;
        [SerializeField] private TMP_Text _spellDescriptionText;
        [SerializeField] private Transform _viewRoot;
        [SerializeField] private Button _okButton;

        private List<GUILearnSpellWidgetButton> _widgets = new List<GUILearnSpellWidgetButton>();

        private void Awake()
        {
            _okButton.onClick.AddListener(CloseAndDestroy);
        }

        public void SetupRandomSpells(List<string> magicSpellObjectNames, Action<string> magicSpellButtonClicked)
        {
            foreach (var magicSpellName in magicSpellObjectNames)
            {
                var widget = GameObject.Instantiate(_widgetPrefab, Vector3.zero, Quaternion.identity, _viewRoot);
                var magicSpell = ResourcesBase.GetMagicSpell(magicSpellName);
                widget.Button.onClick.AddListener(() =>
                {
                    _spellDescriptionObject.gameObject.SetActive(true);
                    magicSpellButtonClicked?.Invoke(magicSpellName);
                    foreach (var w in _widgets)
                    {
                        w.RemoveSelectionEffect();
                    }
                    widget.ApplySelectionEffect();
                    _spellDescriptionText.text = magicSpell.description;
                });
                _widgets.Add(widget);
                widget.Image.sprite = magicSpell.SpriteIcon;
                widget.magicSpellName = magicSpell.internalName;
                Debug.Log($"Select any from {magicSpellName}");
            }
        }
    }
}