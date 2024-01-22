using System;
using System.Collections;
using System.Collections.Generic;
using AgeOfHeroes.Spell;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUISpellSlot : MonoBehaviour
{
    public Button button;
    [SerializeField] protected Image _cooldownOverlay, _iconImage, _framingImage;
    [SerializeField] protected TMP_Text _cooldownCounter, _cooldownInfo, _manaCostText, _titleText;
    protected MagicSpell _magicSpell;

    public MagicSpell MagicSpell
    {
        get => _magicSpell;
        set
        {
            _magicSpell = value;
            if (_magicSpell == null)
            {
                _iconImage.color = Color.clear;
                _manaCostText.gameObject.SetActive(false);
                _cooldownCounter.gameObject.SetActive(false);
                _cooldownInfo.gameObject.SetActive(false);
                _cooldownOverlay.gameObject.SetActive(false);
                return;
            }
            _iconImage.color = Color.white;
            _manaCostText.gameObject.SetActive(true);
            _cooldownCounter.gameObject.SetActive(true);
            _cooldownInfo.gameObject.SetActive(true);
            
            name = _magicSpell.title;
            int cooldown = _magicSpell.Cooldown;
            if (cooldown > 0)
            {
                _cooldownOverlay.gameObject.SetActive(true);
                _cooldownOverlay.fillAmount = (float)cooldown / (float)_magicSpell.BaseCooldown;
            }
            else
            {
                _cooldownOverlay.gameObject.SetActive(false);
            }

            if (_titleText != null)
                _titleText.text = _magicSpell.title;
            _iconImage.sprite = _magicSpell.Icon;
            _cooldownCounter.text = _magicSpell.Cooldown.ToString();
            _cooldownInfo.text = _magicSpell.BaseCooldown.ToString();
            _manaCostText.text = _magicSpell.ManaCost.ToString();
        }
    }
}