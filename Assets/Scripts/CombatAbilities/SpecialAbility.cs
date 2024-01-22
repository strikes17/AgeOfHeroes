using System;
using System.Collections;
using UnityEngine;

namespace AgeOfHeroes
{
    public abstract class SpecialAbility : ICloneable
    {
        [NonSerialized] protected ControllableCharacter _gridCharacter;
        [NonSerialized] public Sprite spriteIcon;

        public bool performedOnRetilation = true;
        public string spriteIconPath;
        public CombatAbilityType combatAbilityType;
        public CombatAbilityOrder combatAbilityOrder;
        public string description, title, internalName;
        public bool executeAlways;

        public virtual SpecialAbility Init(ControllableCharacter _gridCharacter)
        {
            spriteIcon = ResourcesBase.GetSprite(spriteIconPath);
            this._gridCharacter = _gridCharacter;
            return this;
        }

        public abstract IEnumerator Use(CombatData combat);

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}