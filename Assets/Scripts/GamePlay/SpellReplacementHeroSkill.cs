using AgeOfHeroes.Spell;
using Newtonsoft.Json;
using UnityEngine;

namespace AgeOfHeroes
{
    public class SpellReplacementHeroSkill : SpellLearnHeroSkill
    {
        public string spellToReplace;
        protected MagicSpell magicSpell;

        protected override void OnSpellLearnAccepted(LearntHeroSkillData learntHeroSkillData)
        {
            base.OnSpellLearnAccepted(learntHeroSkillData);
            learntHeroSkillData.Hero.spellBook.RemoveSpell(spellToReplace);
            GameManager.Instance.GUISpellBook.UpdateHotbar(learntHeroSkillData.Hero.spellBook.HotbarMagicSpells);
        }
    }
}