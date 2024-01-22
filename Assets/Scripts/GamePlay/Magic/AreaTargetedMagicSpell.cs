using System;
using System.Collections.Generic;
using Redcode.Moroutines;

namespace AgeOfHeroes.Spell
{
    public class AreaTargetedMagicSpell : MagicSpell
    {
        [NonSerialized] public List<AbstractMagicSpellTarget> _spellTargets;
        public int maxRange;
        public int rangeSlices;
        public int powerReducePerSlice;
        public bool powerDependsOnDistance;

        public AreaTargetedMagicSpell(AreaTargetedMagicSpellObject magicSpellObject) : base(magicSpellObject)
        {
            maxRange = magicSpellObject.maxRange;
            rangeSlices = magicSpellObject.rangeSlices;
            powerDependsOnDistance = magicSpellObject.powerDependsOnDistance;
            powerReducePerSlice = magicSpellObject.powerReducePerSlice;
        }

        public override void CreatePrepareVisuals()
        {
        }

        public override void DestroyPrepareVisuals()
        {
            
        }

        public override bool TryCastSpell(MagicSpellCombatData _magicSpellCombatData)
        {
            return base.TryCastSpell(_magicSpellCombatData);
        }

        public override Moroutine CheckCastConditions(MagicSpellCombatData _magicSpellCombatData)
        {
            return null;
        }
    }
}