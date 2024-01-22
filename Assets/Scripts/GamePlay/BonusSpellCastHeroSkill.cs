using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes
{
    public class BonusSpellCastHeroSkill : HeroSkill
    {
        public float chance;
        public int healthValue, manaValue, cooldownReduce;
        public bool anySpell;
        public List<string> spellsNames = new();
        
        public override void OnLearnt(LearntHeroSkillData learntHeroSkillData)
        {
            base.OnLearnt(learntHeroSkillData);
            var hero = learntHeroSkillData.Hero;
            learntHeroSkillData.Hero.spellBook.SpellLearnt += spell =>
            {
                if (spellsNames.Contains(spell.internalName) || anySpell)
                {
                    learntHeroSkillData.Hero.SpellCasted += data =>
                    {
                        if(spell != data.magicSpell)return;
                        var randomFloat = Random.Range(0f, 1f);
                        Debug.Log(randomFloat);
                        if (chance > randomFloat)
                        {
                            hero.ManaLeft += 10;
                            hero.HealthLeft += 25;
                        }
                    };
                }
            };

            var allSpells = hero.spellBook.LearntMagicSpellsList;
            foreach (var spell in allSpells)
            {
                if (spellsNames.Contains(spell.internalName) || anySpell)
                {
                    hero.SpellCasted += data =>
                    {
                        if(spell != data.magicSpell)return;
                        var randomFloat = Random.Range(0f, 1f);
                        Debug.Log(randomFloat);
                        if (chance > randomFloat)
                        {
                            hero.ManaLeft += 10;
                            hero.HealthLeft += 50;
                            Debug.Log("Recover!");
                        }
                    };
                }
            }
        }
    }
}