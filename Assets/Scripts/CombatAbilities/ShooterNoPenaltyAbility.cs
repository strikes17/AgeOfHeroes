using System.Collections;

namespace AgeOfHeroes
{
    public class ShooterNoPenaltyAbility : SpecialAbility
    {
        public override IEnumerator Use(CombatData combatData)
        {
            yield return null;
        }
    }
}