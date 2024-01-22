using System;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes.Spell
{
    public abstract class ModifierBuff : Buff
    {
        public float value;
        protected float _baseValue;
        protected FloatModifier _floatModifier;

        public override bool IsNotDebuff()
        {
            if (value >= 0)
                return true;
            return false;
        }

        public int IntegerValue
        {
            get => Mathf.RoundToInt(value);
        }

        public float BaseValue => _baseValue;

        public FloatModifier Modifier => _floatModifier;

        public int Operation;
        protected int modifierInstanceId;
        [NonSerialized] protected ControllableCharacter _controllableCharacter;

        public override void Init(MagicSpellCombatData magicSpellCombatData)
        {
            base.Init(magicSpellCombatData);
            _baseValue = value;
            _floatModifier = new FloatModifier(_baseValue);
            _controllableCharacter = (ControllableCharacter)this._target;
        }
    }
}