using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUIHeroPortraitWidget : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Button _hideButton;
        [SerializeField] private GUIHeroPortraitWidgetMini _portraitWidgetMini;
        [SerializeField] private Transform _artifactsContent;
        [SerializeField] private List<GUIArtifactQuickSlotWidget> _artifactQuickSlotWidgets;
        private Hero _referencedHero;
        private int _selectedArtifactIndex;
        public bool Locked;

        public Artifact ClearSelectedArtifactSlot()
        {
            var widget = _artifactQuickSlotWidgets[_selectedArtifactIndex];
            _referencedHero.inventoryManager.RemoveArtifact(widget.Artifact);
            var artifact = widget.Artifact;
            widget.Artifact = null;
            widget.artifactFramingButton.onClick.RemoveAllListeners();
            return artifact;
        }

        private void Awake()
        {
            _hideButton.onClick.AddListener(Hide);
            _hideButton.onClick.AddListener((() => Locked = true));
            Hide();
        }

        public void Show()
        {
            if (Locked)
            {
                _portraitWidgetMini.gameObject.SetActive(true);
                return;
            }
            gameObject.SetActive(true);
            _portraitWidgetMini.gameObject.SetActive(false);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            _portraitWidgetMini.gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        private void SetQuickSlotArtifact(Artifact artifact)
        {
            for (int i = 0; i < _artifactQuickSlotWidgets.Count; i++)
            {
                var widget = _artifactQuickSlotWidgets[i];
                if (widget.Artifact == null)
                {
                    widget.Artifact = artifact;
                    widget.artifactFramingButton.onClick.AddListener(() =>
                    {
                        _selectedArtifactIndex = i;
                        Debug.Log(_selectedArtifactIndex);
                        _referencedHero?.CreateDropArtifactGrid();
                    });
                    break;
                }
            }
        }

        private void UpdateUI()
        {
            if (_referencedHero == null) return;
            foreach (var widget in _artifactQuickSlotWidgets)
            {
                widget.Artifact = null;
                widget.artifactFramingButton.onClick.RemoveAllListeners();
            }

            var artifacts = _referencedHero.inventoryManager.equipedArtifacts;
            foreach (var artifact in artifacts)
            {
                SetQuickSlotArtifact(artifact);
            }
        }

        public Hero ReferencedHero
        {
            set
            {
                _referencedHero = value;
                if (_referencedHero == null)
                    return;
                _referencedHero.inventoryManager.ArtifactAdded -= SetQuickSlotArtifact;
                UpdateUI();
                _referencedHero.inventoryManager.ArtifactAdded += SetQuickSlotArtifact;
                _image.sprite = _portraitWidgetMini.PortraitIcon = _referencedHero.HeroObject.portraitIcon;
            }
            get => _referencedHero;
        }

        public GUIHeroPortraitWidgetMini portraitWidgetMini => _portraitWidgetMini;
    }
}