using System;
using System.Collections.Generic;
using Redcode.Moroutines;
using UnityEngine;

namespace AgeOfHeroes.Spell
{
    public abstract class MagicSpell
    {
        public string internalName, title, description;
        public long allowedTarget;
        public int castRange;
        public bool selfCast;
        protected int _baseCooldown, _baseChargesCount, _baseManaCost, _baseCastRange;
        protected readonly List<string> affectsOnlyTheseCharacters = new List<string>();

        public int BaseCastRange
        {
            get { return _baseCastRange; }
            set { _baseCastRange = value; }
        }


        public int BaseCooldown
        {
            get { return _baseCooldown; }
            set { _baseCooldown = value; }
        }

        public int BaseCharges
        {
            get { return _baseChargesCount; }
        }

        public int BaseManaCost
        {
            get { return _baseManaCost; }
            set { _baseManaCost = value; }
        }

        public int Cooldown
        {
            get => _cooldown;
            set => _cooldown = value;
        }

        public int Charges => _charges;

        public int ManaCost
        {
            get => _manaCost;
            set => _manaCost = value;
        }

        public List<string> AffectsOnlyTheseCharacters => affectsOnlyTheseCharacters;

        protected int _cooldown, _charges, _manaCost;
        public Sprite Icon;
        public List<Buff> appliedPositiveBuffsOnCaster;
        public List<Buff> appliedNegativeBuffsOnCaster;

        public List<Buff> appliedPositiveBuffsOnTarget;
        public List<Buff> appliedNegativeBuffsOnTarget;

        public delegate void OnMagicSpellDelegate(MagicSpell magicSpell);

        public MagicSpell(MagicSpellObject magicSpellObject)
        {
            Icon = ResourcesBase.GetSprite(magicSpellObject.iconName);
            selfCast = magicSpellObject.selfCast;
            allowedTarget = magicSpellObject.allowedTarget;
            internalName = magicSpellObject.internalName;
            title = magicSpellObject.title;
            description = magicSpellObject.description;
            castRange = magicSpellObject.castRange;
            _baseCastRange = magicSpellObject.castRange;
            _baseCooldown = magicSpellObject.baseCooldown;
            _baseChargesCount = magicSpellObject.baseChargesCount;
            _baseManaCost = magicSpellObject.baseManaCost;
            _cooldown = 0;
            _charges = _baseChargesCount;
            _manaCost = _baseManaCost;

            appliedPositiveBuffsOnCaster = new List<Buff>();
            magicSpellObject.appliedPositiveBuffsOnCaster.ForEach(x =>
            {
                var buffTemplate = ResourcesBase.GetBuff(x);
                var buff = buffTemplate.Clone() as Buff;
                appliedPositiveBuffsOnCaster.Add(buff);
            });

            appliedNegativeBuffsOnTarget = new List<Buff>();
            magicSpellObject.appliedNegativeBuffsOnTarget.ForEach(x =>
            {
                var buffTemplate = ResourcesBase.GetBuff(x);
                var buff = buffTemplate.Clone() as Buff;
                appliedNegativeBuffsOnTarget.Add(buff);
            });

            appliedNegativeBuffsOnCaster = new List<Buff>();
            magicSpellObject.appliedNegativeBuffsOnCaster.ForEach(x =>
            {
                var buffTemplate = ResourcesBase.GetBuff(x);
                var buff = buffTemplate.Clone() as Buff;
                appliedNegativeBuffsOnCaster.Add(buff);
            });

            appliedPositiveBuffsOnTarget = new List<Buff>();
            magicSpellObject.appliedPositiveBuffsOnTarget.ForEach(x =>
            {
                var buffTemplate = ResourcesBase.GetBuff(x);
                var buff = buffTemplate.Clone() as Buff;
                appliedPositiveBuffsOnTarget.Add(buff);
            });
            affectsOnlyTheseCharacters.AddRange(magicSpellObject.affectsOnlyTheseCharacters);
        }

        public virtual bool TryCastSpell(MagicSpellCombatData _magicSpellCombatData)
        {
            return false;
        }

        public virtual void SetOnCooldown()
        {
            _cooldown = BaseCooldown;
        }

        public abstract void CreatePrepareVisuals();
        public abstract Moroutine CheckCastConditions(MagicSpellCombatData _magicSpellCombat);
        public abstract void DestroyPrepareVisuals();
    }
}