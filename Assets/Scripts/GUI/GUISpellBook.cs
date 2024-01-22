using System;
using System.Collections;
using System.Collections.Generic;
using Redcode.Moroutines;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes.Spell
{
    public class GUISpellBook : MonoBehaviour
    {
        public float animationFps;
        public Transform spellBookRoot;
        public int maxSpellsOnPage = 2;
        private int maxSpellsOnPairedPage;
        public GUISpellPage leftPage, rightPage;
        public GUITurnPageButton leftTurnPageButton, rightTurnPageButton;
        public SpellBook spellBook;
        public Image bgImage;
        public bool isInteractionLocked = false;
        [SerializeField] private TMP_Text _manaInfo;
        [SerializeField] private List<GUISpellSlot> _hotbarSpellSlots;
        [SerializeField] private Transform _hotbarRoot;
        private Material bgMaterial;
        private GUISpriteAnimation _guiSpriteAnimation;
        private int pairedPagesIndex = 0;
        private int totalPairedPages;
        private GUISpriteAnimation _leftPageAnimation, _rightPageAnimation;

        public delegate void OnSpellBookGUIDelegate();

        public event OnSpellBookGUIDelegate Opened
        {
            add => onOpened += value;
            remove => onOpened -= value;
        }

        public event OnSpellBookGUIDelegate Opening
        {
            add => onOpening += value;
            remove => onOpening -= value;
        }

        public event OnSpellBookGUIDelegate Closed
        {
            add => onClosed += value;
            remove => onClosed -= value;
        }

        public event OnSpellBookGUIDelegate Closing
        {
            add => onClosing += value;
            remove => onClosing -= value;
        }

        private event OnSpellBookGUIDelegate onOpening;
        private event OnSpellBookGUIDelegate onOpened;
        private event OnSpellBookGUIDelegate onClosing;
        private event OnSpellBookGUIDelegate onClosed;

        public void OnSpellSelected(MagicSpell magicSpell)
        {
            if (magicSpell == null) return;
            var mapScenarioHandler = GameManager.Instance.MapScenarioHandler;
            var activePlayer = mapScenarioHandler.players[mapScenarioHandler.TurnOfPlayerId];
            activePlayer.PrepareSpell(magicSpell);
            activePlayer.SetActiveContext(PlayerContext.UsingSpell);
        }

        public void UpdateHotbar(Dictionary<int, MagicSpell> hotbarMagicSpells)
        {
            for (int i = 0; i < _hotbarSpellSlots.Count; i++)
            {
                MagicSpell magicSpell = null;
                var hotbarSpellSlot = _hotbarSpellSlots[i];
                if (hotbarMagicSpells.TryGetValue(i, out magicSpell))
                {
                    hotbarSpellSlot.MagicSpell = magicSpell;
                    hotbarSpellSlot.button.interactable = true;
                    continue;
                }

                hotbarSpellSlot.button.interactable = false;
                hotbarSpellSlot.MagicSpell = null;
            }
        }

        public void ShowHotbar()
        {
            _hotbarRoot.gameObject.SetActive(true);
        }

        public void HideHotbar()
        {
            _hotbarRoot.gameObject.SetActive(false);
        }

        public void Init()
        {
            _leftPageAnimation = leftTurnPageButton.GetComponent<GUISpriteAnimation>();
            _rightPageAnimation = rightTurnPageButton.GetComponent<GUISpriteAnimation>();
            maxSpellsOnPairedPage = maxSpellsOnPage * 2;
            bgMaterial = bgImage.material;
            _guiSpriteAnimation = bgImage.GetComponent<GUISpriteAnimation>();
            spellBookRoot.gameObject.SetActive(false);
            rightPage.gameObject.SetActive(false);
            leftPage.gameObject.SetActive(false);
            leftTurnPageButton.AddOnClickListener(() =>
            {
                rightTurnPageButton.gameObject.SetActive(true);

                pairedPagesIndex = pairedPagesIndex == 0 ? 0 : pairedPagesIndex - 1;
                if (pairedPagesIndex == 0)
                    leftTurnPageButton.gameObject.SetActive(false);

                FillPages();
            });
            rightTurnPageButton.AddOnClickListener(() =>
            {
                leftTurnPageButton.gameObject.SetActive(true);

                pairedPagesIndex = pairedPagesIndex == totalPairedPages - 1 ? pairedPagesIndex : pairedPagesIndex + 1;
                if (pairedPagesIndex == totalPairedPages - 1)
                    rightTurnPageButton.gameObject.SetActive(false);

                FillPages();
            });
            foreach (var hotbarSpellSlot in _hotbarSpellSlots)
            {
                hotbarSpellSlot.button.onClick.AddListener(() => { OnSpellSelected(hotbarSpellSlot.MagicSpell); });
            }
        }

        public void SetSpellBook(SpellBook spellBook)
        {
            this.spellBook = spellBook;
            pairedPagesIndex = 0;
            int totalSpellsCount = spellBook.LearntMagicSpells.Count;
            float totalPagesFloat = (float)totalSpellsCount / maxSpellsOnPairedPage;
            totalPairedPages = Mathf.CeilToInt(totalPagesFloat);
        }

        private void ShowTurnPagesButtons()
        {
            if (totalPairedPages == 1)
            {
                leftTurnPageButton.gameObject.SetActive(false);
                rightTurnPageButton.gameObject.SetActive(false);
            }
            else if (totalPairedPages > 1)
            {
                leftTurnPageButton.gameObject.SetActive(true);
                rightTurnPageButton.gameObject.SetActive(true);

                if (pairedPagesIndex == 0)
                    leftTurnPageButton.gameObject.SetActive(false);
                else if (pairedPagesIndex == totalPairedPages - 1)
                    rightTurnPageButton.gameObject.SetActive(false);
            }
        }

        public void FillPages()
        {
            leftPage.Clear();
            rightPage.Clear();
            var spells = spellBook.LearntMagicSpellsList;
            int c = 0;
            for (int i = pairedPagesIndex * maxSpellsOnPairedPage;
                 i < (pairedPagesIndex + 1) * maxSpellsOnPairedPage;
                 i++)
            {
                if (i > spells.Count - 1)
                    break;
                if (c >= maxSpellsOnPairedPage)
                    c = 0;

                if (c < maxSpellsOnPage)
                {
                    leftPage.SpellSelected += OnSpellSelected;
                    leftPage.PlaceSpell(spells[i]);
                }

                if (c >= maxSpellsOnPage)
                {
                    rightPage.SpellSelected += OnSpellSelected;
                    rightPage.PlaceSpell(spells[i]);
                }

                c++;
            }
        }

        public void Open(ControllableCharacter sourceCharacter)
        {
            if(gameObject.activeSelf)
                return;
            if (isInteractionLocked) return;
            var playerLastSelectedCharacter = sourceCharacter;
            _manaInfo.text =
                $"{playerLastSelectedCharacter.ManaLeft.ToString()}/{playerLastSelectedCharacter.startingMana.ToString()}";
            onOpening?.Invoke();
            bgMaterial.SetColor("_OutlineColor", spellBook.Player.realColor);
            spellBookRoot.gameObject.SetActive(true);
            isInteractionLocked = true;
            _guiSpriteAnimation.PlayOneShot(animationFps, () =>
            {
                onOpened?.Invoke();
                rightPage.gameObject.SetActive(true);
                leftPage.gameObject.SetActive(true);
                ShowTurnPagesButtons();
                FillPages();
                isInteractionLocked = false;
                _leftPageAnimation.PlayLooped(20f);
                _rightPageAnimation.PlayLooped(20f);
            });
        }

        public void Close(bool skipAnimation = false)
        {
            if(!gameObject.activeSelf)
                return;
            if (isInteractionLocked) return;
            onClosing?.Invoke();
            isInteractionLocked = true;
            leftPage.gameObject.SetActive(false);
            rightPage.gameObject.SetActive(false);
            leftTurnPageButton.gameObject.SetActive(false);
            rightTurnPageButton.gameObject.SetActive(false);
            _leftPageAnimation.Stop();
            _rightPageAnimation.Stop();
            if (!skipAnimation)
                _guiSpriteAnimation.PlayOneShotReversed(animationFps, () =>
                {
                    onClosed?.Invoke();
                    isInteractionLocked = false;
                    spellBookRoot.gameObject.SetActive(false);
                });
            else
            {
                onClosed?.Invoke();
                isInteractionLocked = false;
                spellBookRoot.gameObject.SetActive(false);
            }
        }

        public void SwitchState(ControllableCharacter controllableCharacter)
        {
            if (!spellBookRoot.gameObject.activeSelf)
            {
                Open(controllableCharacter);
            }
            else
            {
                Close();
            }
        }
    }
}