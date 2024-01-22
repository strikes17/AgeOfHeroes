using System.Collections;
using UnityEngine;

namespace AgeOfHeroes
{
    public class StatueRecoveryAbility : SpecialAbility
    {
        public override IEnumerator Use(CombatData combat)
        {
            var target = combat.offensiveCharacter;
            Debug.Log($"mp: {target.MovementPointsLeft} | tm: {target.MaxMovementValue}");
            if (target.MovementPointsLeft == target.MaxMovementValue)
            {
                var healValue = Mathf.RoundToInt(target.HealthValue * 0.25f);
                target.ChangeHealthValue(healValue);
            }

            yield break;
        }
    }
}