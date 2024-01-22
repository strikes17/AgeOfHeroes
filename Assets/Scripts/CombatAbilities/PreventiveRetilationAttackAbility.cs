using System.Collections;
using Redcode.Moroutines;
using UnityEngine;

namespace AgeOfHeroes
{
    public class PreventiveRetilationAttackAbility : SpecialAbility
    {
        public override IEnumerator Use(CombatData combatData)
        {
            var source = combatData.defensiveCharacter;
            var target = combatData.offensiveCharacter;
            yield return Moroutine.Run(source.IERetilationAttackProcess(target)).WaitForComplete();
        }
    }
}