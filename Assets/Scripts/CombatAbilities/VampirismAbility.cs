using System;
using System.Collections;
using AgeOfHeroes.Spell;
using UnityEngine;

namespace AgeOfHeroes
{
    public class VampirismAbility : SpecialAbility
    {
        public float effectMultiplier = 0.9f;

        public override IEnumerator Use(CombatData combatData)
        {
            var source = combatData.offensiveCharacter;
            var target = combatData.defensiveCharacter;
            if ((target.persona & (long) MagicSpellAllowedTarget.Alive) == 0)
                yield break;

            var damage = combatData.totalDamage;
            float dmg = (float)damage * effectMultiplier;
            damage = Mathf.RoundToInt(dmg);
            source.ChangeHealthValue((int) damage);
            Debug.Log($"from {source.title}: {damage}");
        }
    }
}