using System;
using System.Collections;
using System.Collections.Generic;
using AgeOfHeroes.Spell;
using Redcode.Moroutines;
using TMPro;
using UnityEditor;
// using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace AgeOfHeroes
{
    [IconAttribute("Assets/Editor/character.png")]
    [RequireComponent(typeof(CharacterEditorAddon))]
    public class Character : ControllableCharacter
    {
        public TMP_Text _tmpTextQuantity;
        public SpriteRenderer _quantityBackgroundSprite;

        public CharacterObject CharacterObject
        {
            get { return (CharacterObject)_controllableCharacterObject; }
            set { _controllableCharacterObject = value; }
        }

        public override void Init()
        {
            base.Init();
            StackDemolished += (target, source) => ClearUp(this);
            StackBanished += (target, source) => ClearUp(this);
            growthPerDay = CharacterObject.baseGrowthPerDay;
            _spriteRenderer.sortingOrder = CharacterObject.isHuge
                ? GlobalVariables.HugeCharacterRenderOrder
                : GlobalVariables.SmallCharacterRenderOrder;
            _spawnedCount = _count;
        }

        protected void Update()
        {
            base.Update();
            _quantityBackgroundSprite.color = GlobalVariables.playerColors[playerOwnerColor];
            _tmpTextQuantity.text = Count.ToString();
        }

        public override int GetTier()
        {
            return CharacterObject.tier;
        }

        public override Sprite GetMainSprite()
        {
            return CharacterObject.mainSprite;
        }

        public override void VisuallyHide()
        {
            base.VisuallyHide();
            _tmpTextQuantity.gameObject.SetActive(false);
            _quantityBackgroundSprite.gameObject.SetActive(false);
        }

        public override void VisuallyShow()
        {
            base.VisuallyShow();
            _tmpTextQuantity.gameObject.SetActive(true);
            _quantityBackgroundSprite.gameObject.SetActive(true);
        }

        public override void OnStackDemolished(ControllableCharacter demolishedCharacter,
            ControllableCharacter characterSource = null)
        {
            base.OnStackDemolished(demolishedCharacter, characterSource);
            gameObject.SetActive(false);
            isExistingInWorld = false;
            Player.fogOfWarController.UpdateVisionForCharacterOnDeath(this);
            _gameManager.SpawnManager.SpawnCorpse(this);
            MassiveCheer();
            Moroutine.Run(IEDestroyBody());
        }
        
        public override void ChangeHealthValue(int value, ControllableCharacter characterSource = null)
        {
            if (this == null)
                return;
            int deathCount = 0;
            if (_count == _spawnedCount && value > 0)
            {
                HealthLeft += value;
            }
            else if (_count > 1)
            {
                int totalHealth = HealthValue * (_count - 1) + HealthLeft;
                int health = totalHealth + value;
                float ratio = (float)health / HealthValue;
                int ostatok = health % HealthValue;
                int countLeft = Mathf.CeilToInt(ratio);
                deathCount = countLeft - Mathf.CeilToInt((float)totalHealth / (float)HealthValue);
                _count = Mathf.Clamp(countLeft, 0, _spawnedCount);
                _healthLeft = ostatok;
                if (HealthLeft == 0)
                {
                    _healthLeft = HealthValue;
                }
            }
            else if (_count == 1)
            {
                _healthLeft += value;
                if (_healthLeft <= 0)
                    _count--;
                if (_healthLeft > HealthValue)
                    _healthLeft = HealthValue;
            }

            var fsc = CharacterObject.fullStackCount;
            if (_count > fsc)
                _count = fsc;

            OnHealthChanged(value);

            if (_count <= 0)
            {
                OnStackDemolished(this);
            }

            var valueColor = value >= 0
                ? GlobalVariables.healthPositiveTextColor
                : GlobalVariables.healthNegativeTextColor;
            var countColor = deathCount >= 0
                ? GlobalVariables.healthPositiveTextColor
                : GlobalVariables.healthNegativeTextColor;
            FloatingText.Create(Position, 0.2f, 0.2f)
                .MakeFloatDelayed(countColor, deathCount, 0.4f);
            FloatingText.Create(Position, 0.2f, 0.2f).MakeFloat(valueColor, value);

            OnQuantityChanged(-deathCount);
            if (characterSource == null) return;
            bool isHero = characterSource is Hero;

            if (isHero)
            {
                characterSource.GainExperience(experienceValue * -deathCount);
                return;
            }

            var allEnemyHeroesAround =
                _activeTerrain.TerrainNavigator.NavigationMap.GetAllObjectsOfType<Hero>(Position,
                    GlobalVariables.heroGainExperienceRange, LayerMask.NameToLayer("Character"));

            foreach (var hero in allEnemyHeroesAround)
            {
                if (IsAnAllyTo(hero)) continue;
                float expValue = (float)(experienceValue * -deathCount) * GlobalVariables.heroGainExperiencePenalty;
                hero.GainExperience(Mathf.RoundToInt(expValue));
            }
        }

        public override void OnNewTurnBegin()
        {
            if (!gameObject.activeSelf)
                return;
            AttacksLeft = AttacksCountValue;
            _retilationsLeft = CharacterObject.retilationsCount;
            countOfPerformedAttacks = 0;
            base.OnNewTurnBegin();
        }

        public override void OnEndOfTurn()
        {
        }
    }
}