using System.Collections;
using Redcode.Moroutines;
using UnityEngine;

namespace AgeOfHeroes
{
    public class RangedDoubleAttackAbility : SpecialAbility
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
            Vector3Int attackerPosition = ControllableCharacter.V3Int(source.transform.position);
            Vector3Int targetPosition = ControllableCharacter.V3Int(target.transform.position);
            var distance = Vector3Int.Distance(attackerPosition, targetPosition);
            if (Mathf.FloorToInt(distance) == 1)
                yield break;
            yield return Moroutine.Run(source.IECombatAttackProcess(combatData)).WaitForComplete();
        }
    }
}