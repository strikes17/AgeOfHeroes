using System;
using System.Collections.Generic;
using AgeOfHeroes.Spell;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AgeOfHeroes
{
    public class SpellLearnHeroSkill : ModifierHeroSkill
    {
        public List<string> magicSpellVariants;
        public int totalVariantsShown;
        protected string _selectedSpellName;
        protected List<string> _selectedSpellVariants;

        public SpellLearnHeroSkill()
        {
            magicSpellVariants = new List<string>();
        }

        public override void OnLearnt(LearntHeroSkillData learntHeroSkillData)
        {
            base.OnLearnt(learntHeroSkillData);
            if (learntHeroSkillData.SilentMode) return;
            if (totalVariantsShown > 1)
                SelectRandomVariants(learntHeroSkillData);
            else if (totalVariantsShown == 1)
            {
                _selectedSpellName = magicSpellVariants[0];
                OnSpellLearnAccepted(learntHeroSkillData);
            }
        }

        private void SelectRandomVariants(LearntHeroSkillData learntHeroSkillData)
        {
            _selectedSpellVariants = new List<string>();
            int totalSpellsCount = magicSpellVariants.Count;
            List<int> randoms = new List<int>();
            for (int i = 0; i < totalSpellsCount; i++)
            {
                randoms.Add(i);
            }

            for (int i = 0; i < totalVariantsShown; i++)
            {
                int rand = Random.Range(0, randoms.Count);
                int num = randoms[rand];
                randoms.RemoveAt(rand);
                _selectedSpellVariants.Add(magicSpellVariants[num]);
            }

            var dialogue = GUIDialogueFactory.CreateLearnRandomSpellDialogue();
            dialogue.SetupRandomSpells(_selectedSpellVariants, OnMagicSpellButtonClicked);
            dialogue.Closed += () => OnSpellLearnAccepted(learntHeroSkillData);
            dialogue.Show();
        }

        protected void OnMagicSpellButtonClicked(string internalName)
        {
            _selectedSpellName = internalName;
        }

        protected virtual void OnSpellLearnAccepted(LearntHeroSkillData learntHeroSkillData)
        {
            base.OnLearnt(learntHeroSkillData);
            learntHeroSkillData.Hero.spellBook.AddSpell(_selectedSpellName);
        }
    }
}