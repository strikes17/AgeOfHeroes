using System;
using System.Collections;
using Redcode.Moroutines;
using UnityEngine;

namespace AgeOfHeroes
{
    public class DoubleAttackAbility : SpecialAbility
    {
        public override SpecialAbility Init(ControllableCharacter _gridCharacter)
        {
            _gridCharacter.maxCountOfPerformedAttacks = 2;
            return base.Init(_gridCharacter);
        }

        public override IEnumerator Use(CombatData combatData)
        {
            var source = combatData.offensiveCharacter;
            var target = combatData.defensiveCharacter;
            if (target != null && source != null)
                yield return Moroutine.Run(source.IECombatAttackProcess(combatData)).WaitForComplete();
        }
    }
}