namespace AgeOfHeroes.Spell
{
    public class SplashAttackBuff : CombatEventBuff
    {
        protected override void OnAttackPerformed(CombatData combatData)
        {
            if (_specialEffectObject == null)
                return;
            var animation = UseSpriteAnimationEffect(_specialEffectObject.onBuffUpdateSpriteSequenceObject,
                _specialEffectObject.onBuffUpdateEffectCycles);
            var neighbours = _buffTarget.GetNeighboursInSplashAttackRange(combatData.defensiveCharacter);
            combatData.defensiveCharacter.CreateBuff(_onAttackPerformedBuff).Apply();
            foreach (var n in neighbours)
            {
                n.CreateBuff(_onAttackPerformedBuff).Apply();
            }
        }
    }
}