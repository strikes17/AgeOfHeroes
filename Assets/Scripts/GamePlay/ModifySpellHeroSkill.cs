using System;
using System.Collections.Generic;
using AgeOfHeroes.Spell;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AgeOfHeroes
{
    public class ModifySpellHeroSkill : ModifierHeroSkill
    {
        public float manaCostMultiplier, cooldownMultiplier, castRangeMultiplier;
        public List<string> magicSpellsNames = new();
        protected MagicSpell _magicSpell;

        public override void OnLearnt(LearntHeroSkillData learntHeroSkillData)
        {
            base.OnLearnt(learntHeroSkillData);
            var spellBook = learntHeroSkillData.Hero.spellBook;
            spellBook.SpellLearnt += (magicSpell) =>
            {
                CheckMagicSpell(magicSpell, learntHeroSkillData);
            };

            var magicSpells = spellBook.LearntMagicSpells;
            foreach (var magic in magicSpells)
            {
                if (magicSpellsNames.Contains(magic.Key))
                {
                    CheckMagicSpell(magic.Value, learntHeroSkillData);
                }
            }
        }

        private void CheckMagicSpell(MagicSpell magicSpell, LearntHeroSkillData learntHeroSkillData)
        {
            if (magicSpellsNames.Contains(magicSpell.internalName))
            {
                _magicSpell = magicSpell;
                Modify(learntHeroSkillData);
            }
        }

        public override void Modify(LearntHeroSkillData learntHeroSkillData)
        {
            _magicSpell.ManaCost = Mathf.RoundToInt(_magicSpell.BaseManaCost * manaCostMultiplier);
            _magicSpell.BaseCooldown = Mathf.RoundToInt(_magicSpell.BaseCooldown * cooldownMultiplier);
            _magicSpell.castRange = Mathf.RoundToInt(_magicSpell.BaseCastRange * castRangeMultiplier);
            Debug.Log($"Modified {_magicSpell.internalName}");
        }
    }
}