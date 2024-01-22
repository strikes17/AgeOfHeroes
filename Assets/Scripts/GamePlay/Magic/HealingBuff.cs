using System;
using UnityEngine;

namespace AgeOfHeroes.Spell
{
    [Serializable]
    public class HealingBuff : Buff
    {
        public int value = 0;
        public bool ressurects;
        public bool dispellsNegativeBuffs;
        private ControllableCharacter _controllableCharacter;
        public override bool IsNotDebuff()
        {
            if (value >= 0)
                return true;
            return false;
        }
        public override void Init(MagicSpellCombatData magicSpellCombatData)
        {
            base.Init(magicSpellCombatData);
            _controllableCharacter = (ControllableCharacter) this._target;
        }

        public HealingBuff SetHealthValue(int value)
        {
            this.value = value;
            return this;
        }

        public HealingBuff CanRessurect(bool ressurects)
        {
            this.ressurects = ressurects;
            return this;
        }

        public HealingBuff SetDuration(int roundsDuration)
        {
            this.baseDuration = roundsDuration;
            return this;
        }

        public override void UpdateState()
        {
            if (!ressurects)
                value = Mathf.Clamp(value, 0, _controllableCharacter.baseHealth - _controllableCharacter.HealthLeft);
            _controllableCharacter.ChangeHealthValue(value);
            base.UpdateState();
        }
    }
}