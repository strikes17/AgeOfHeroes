using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUICollectArtifactDialogue : GUIDialogueWindow
    {
        [SerializeField] private GUIArtifactStatWidget _statWidgetPrefab;
        [SerializeField] private Transform _contentRoot;
        [SerializeField] private Button _okButton, _closeButtoon;
        [SerializeField] private Button _artifactFramingButton;
        [SerializeField] private Image _artifactImage;
        [SerializeField] private TMP_Text _titleText;
        private ArtifactObject _artifactObject;
        private List<GUIArtifactStatWidget> _widgets = new List<GUIArtifactStatWidget>();

        public Button okButton => _okButton;

        public void Set(ArtifactObject artifactObject)
        {
            Clear();
            _artifactObject = artifactObject;
            _titleText.text = artifactObject.Title;
            var color = GlobalVariables.ArtifactQualityColors[_artifactObject.Quality];
            _titleText.color = color;
            _artifactFramingButton.image.color = color;
            _artifactImage.sprite = _artifactObject.Icon;
            var modifiers = _artifactObject.Modifiers;
            for (int i = 0; i < modifiers.Count; i++)
            {
                var widget = GetOrCreateWidget(modifiers[i]);
                _widgets.Add(widget);
            }
        }

        public void HasFreeSlots()
        {
            _okButton.gameObject.SetActive(true);
            _okButton.onClick.RemoveAllListeners();
            _closeButtoon.gameObject.SetActive(false);
        }

        public void AlertOfMaxSlots()
        {
            _closeButtoon.gameObject.SetActive(true);
            _okButton.gameObject.SetActive(false);
            _closeButtoon.onClick.RemoveAllListeners();
            _closeButtoon.onClick.AddListener(Hide);
        }

        private void Clear()
        {
            for (int i = 0; i < _widgets.Count; i++)
            {
                Destroy(_widgets[i].gameObject);
            }

            _widgets.Clear();
        }

        private GUIArtifactStatWidget GetOrCreateWidget(ArtifactModifier modifier)
        {
            var widget = GameObject.Instantiate(_statWidgetPrefab, Vector3.zero, Quaternion.identity, _contentRoot);
            widget.Icon = ResourcesBase.GetSprite($"stats+{GlobalVariables.statIcons[modifier.StatType]}");
            var operationChar = modifier.value >= 0 ? GlobalVariables.operationNames[modifier.operation] : string.Empty;
            widget.Description = $"{GlobalVariables.statNames[modifier.StatType]} {operationChar}{modifier.value}";
            return widget;
        }
    }
}