using System;
using System.Collections.Generic;
using AgeOfHeroes.Spell;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUICharacterInfoWindow : GUIDialogueWindow
    {
        [SerializeField] private Image characterBgImage, characterImage, quantityBgImage;
        [SerializeField] private Button _closeButton;
        [SerializeField] private GUIPanWindowButton _panWindowButton;
        [SerializeField] private GUICharacterStatsInfo _characterStatsInfo;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private GameObject combatAbilityPrefab;
        [SerializeField] private GUIBuffInfoWidget _buffInfoWidgetPrefab;
        [SerializeField] private Transform buffsBarTransform;
        [SerializeField] private Button _descriptionTab, _specialsTab, _buffsTab;
        [SerializeField] private RectTransform _descriptionContent, _specialsContent, _buffsContent;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private GUISpecialDescriptionWidget _specialDescriptionWidgetPrefab;
        [SerializeField] private GUIBuffInfoPopup _buffInfoPopup;
        [SerializeField] private GUIHeroExperienceWidget _experienceWidget;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private GameObject _levelGameObject;

        public bool temporaryInstance;

        private Dictionary<int, GUISpecialDescriptionWidget> _instantiatedWidgets =
            new Dictionary<int, GUISpecialDescriptionWidget>();

        private ControllableCharacter _controllableCharacter;
        private CharacterObject _characterObject;
        private List<GUIBuffInfoWidget> _buffInfoWidgets = new List<GUIBuffInfoWidget>();
        private List<GUICombatAbilitiyInfoWidget> _combatAbilitiyInfoWidgets = new List<GUICombatAbilitiyInfoWidget>();

        private void Awake()
        {
            _closeButton.onClick.AddListener(Hide);
            _descriptionTab.onClick.AddListener(ShowDescriptionView);
            _specialsTab.onClick.AddListener(ShowSpecialsView);
            _buffsTab.onClick.AddListener(ShowBuffsView);
            ShowDescriptionView();
        }

        public override void Show()
        {
            base.Show();
            _buffInfoPopup.Hide();
        }

        public void ShowDescriptionView()
        {
            _specialsContent.gameObject.SetActive(false);
            _buffsContent.gameObject.SetActive(false);
            _descriptionContent.gameObject.SetActive(true);
            _scrollRect.content = _descriptionContent;
        }

        public void ShowSpecialsView()
        {
            _specialsContent.gameObject.SetActive(true);
            _buffsContent.gameObject.SetActive(false);
            _descriptionContent.gameObject.SetActive(false);
            _scrollRect.content = _specialsContent;
        }

        public void ShowBuffsView()
        {
            _specialsContent.gameObject.SetActive(false);
            _buffsContent.gameObject.SetActive(true);
            _descriptionContent.gameObject.SetActive(false);
            _scrollRect.content = _buffsContent;
        }

        public override void Hide()
        {
            base.Hide();
            if (temporaryInstance)
            {
                Destroy(gameObject);
            }
        }

        private void Clear()
        {
            for (int i = 0; i < _buffInfoWidgets.Count; i++)
            {
                Destroy(_buffInfoWidgets[i].gameObject);
            }

            for (int i = 0; i < _combatAbilitiyInfoWidgets.Count; i++)
            {
                Destroy(_combatAbilitiyInfoWidgets[i].gameObject);
            }

            var keys = _instantiatedWidgets.Keys;
            foreach (var key in keys)
            {
                Destroy(_instantiatedWidgets[key].gameObject);
            }

            _buffInfoWidgets.Clear();
            _instantiatedWidgets.Clear();
            _combatAbilitiyInfoWidgets.Clear();
        }

        public void SetCharacterInfoForWorld(ControllableCharacter controllableCharacter)
        {
            if (controllableCharacter == null)
                return;
            Clear();
            if (controllableCharacter is Hero)
            {
                var hero = controllableCharacter as Hero;
                _experienceWidget.FillValue = hero.LeftExperienceRatio;
                _experienceWidget.Show();
                _levelText.text = hero.CurrentLevel.ToString();
                _levelGameObject.SetActive(true);
            }
            else
            {
                _experienceWidget.Hide();
                _levelGameObject.SetActive(false);
            }
            buffsBarTransform.gameObject.SetActive(true);
            characterImage.sprite = controllableCharacter.GetMainSprite();
            _titleText.text = controllableCharacter.title;
            _characterStatsInfo.SetGUIInfo(controllableCharacter);
            quantityBgImage.color = GlobalVariables.playerColors[controllableCharacter.playerOwnerColor];
            var appliedBuffs = controllableCharacter.AllAppliedBuffs;
            List<Buff> stackedBuffs = new List<Buff>();
            for (int i = 0; i < appliedBuffs.Count; i++)
            {
                var buff = appliedBuffs[i];
                if (buff.stackingEnabled)
                {
                    stackedBuffs.Add(buff);
                    continue;
                }

                CreateBuffInfoWidget(buff);
            }

            if (stackedBuffs.Count > 0)
                CreateBuffInfoWidget(stackedBuffs[0]);


            var combatAbilities = controllableCharacter.specialAbilities;
            int abilityIndex = 0;
            foreach (var ability in combatAbilities)
            {
                abilityIndex++;
                var abilityInstance = GameObject.Instantiate(combatAbilityPrefab, Vector3.zero, Quaternion.identity,
                    _specialsContent);
                abilityInstance.name = $"{ability.title}_{abilityIndex}";
                var abilityInfoWidget = abilityInstance.GetComponent<GUICombatAbilitiyInfoWidget>();
                abilityInfoWidget.AbilityIcon = ability.spriteIcon;
                abilityInfoWidget.AbilityName = ability.title;
                abilityInfoWidget.clickableIcon.onClick.AddListener(() =>
                    ToggleSpecialView(abilityInfoWidget, ability.description));
                _combatAbilitiyInfoWidgets.Add(abilityInfoWidget);
            }
        }

        private void CreateBuffInfoWidget(Buff buff)
        {
            var buffInfoWidget = GameObject.Instantiate(_buffInfoWidgetPrefab, Vector3.zero, Quaternion.identity,
                buffsBarTransform);
            buffInfoWidget.name = $"{buff.internalName}_{buff.GetHashCode()}";
            buffInfoWidget.BuffIcon = buff.spriteIcon;
            buffInfoWidget.RoundsLeft = buff.durationLeft;
            buffInfoWidget.IsNotDebuff = buff.IsNotDebuff();
            buffInfoWidget.button.onClick.AddListener(() =>
                ShowBuffInfoPopup(buff, buffInfoWidget.transform.position));
            _buffInfoWidgets.Add(buffInfoWidget);
        }

        private void ShowBuffInfoPopup(Buff buff, Vector2 position)
        {
            _buffInfoPopup.TItle = buff.title;
            _buffInfoPopup.Description = buff.description;
            _buffInfoPopup.transform.position = position;
            _buffInfoPopup.Show();
        }

        private void HideBuffInfoPopup()
        {
            _buffInfoPopup.TItle = string.Empty;
            _buffInfoPopup.Description = string.Empty;
            _buffInfoPopup.Hide();
        }

        private void ToggleSpecialView(GUICombatAbilitiyInfoWidget widget, string description)
        {
            GUISpecialDescriptionWidget descriptionWidget = null;
            int widgetHashCode = widget.GetHashCode();
            if (_instantiatedWidgets.TryGetValue(widgetHashCode, out descriptionWidget))
            {
                descriptionWidget.Description = description;
                descriptionWidget.Toggle();
                return;
            }

            int siblingIndex = widget.transform.GetSiblingIndex();
            descriptionWidget = GameObject.Instantiate(_specialDescriptionWidgetPrefab, Vector3.zero,
                Quaternion.identity, _specialsContent.transform);
            descriptionWidget.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            descriptionWidget.transform.SetSiblingIndex(siblingIndex + 1);
            _instantiatedWidgets.TryAdd(widgetHashCode, descriptionWidget);
            descriptionWidget.Description = description;
            descriptionWidget.Show();
        }

        public void SetCharacterInfoForCastle(CharacterObject characterObject, CastleTierIncomeValues incomeValues)
        {
            Clear();
            buffsBarTransform.gameObject.SetActive(false);
            _characterObject = characterObject;
            if (_characterObject == null)
                return;
            characterImage.sprite = _characterObject.mainSprite;
            _titleText.text = _characterObject.title;
            _characterStatsInfo.SetGUIInfo(_characterObject, incomeValues);
            quantityBgImage.color = Colors.unavailableElementDarkGray;
            _experienceWidget.Hide();
            _levelGameObject.SetActive(false);

            var combatAbilityNames = characterObject._combatAbilityNames;
            int abilityIndex = 0;
            foreach (var combatAbilityName in combatAbilityNames)
            {
                var ability = ResourcesBase.GetCombatAbility(combatAbilityName);
                if (ability == null) continue;
                abilityIndex++;
                var abilityInstance = GameObject.Instantiate(combatAbilityPrefab, Vector3.zero, Quaternion.identity,
                    _specialsContent);
                abilityInstance.name = $"{ability.title}_{abilityIndex}";
                var abilityInfoWidget = abilityInstance.GetComponent<GUICombatAbilitiyInfoWidget>();
                abilityInfoWidget.AbilityIcon = ability.spriteIcon;
                abilityInfoWidget.AbilityName = ability.title;
                abilityInfoWidget.clickableIcon.onClick.AddListener(() =>
                    ToggleSpecialView(abilityInfoWidget, ability.description));
                _combatAbilitiyInfoWidgets.Add(abilityInfoWidget);
            }
        }
    }
}