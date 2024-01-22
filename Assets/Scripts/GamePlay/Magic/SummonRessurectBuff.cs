using System;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes.Spell
{
    public class SummonRessurectBuff : RessurectBuff
    {
        public List<string> summonVariants = new List<string>();
        public int selectedVariantOrRandom = -1;

        protected void ShowSelectionGUI()
        {
            var dialogue = GUIDialogueFactory.CreateNecromanceryDialogueWindow();
            dialogue.Title = "Select Summon";
            dialogue.SetVariants(summonVariants);
            dialogue.Applied = () =>
            {
                var selectedWidget = dialogue.SelectedWidget;
                _characterObject = selectedWidget.CharacterObject;
                int maxHealth = _characterObject.startingHealth;
                int count = healthValue / maxHealth + 1;
                int healthLeft = healthValue % maxHealth;
                var character = GameManager.Instance.SpawnManager.SpawnCharacter(_characterObject,
                    _characterObject.Fraction, playerSummoner.Color, position,
                    playerSummoner.ActiveTerrain, count);
                character.ChangeHealthValue(-(maxHealth - healthLeft));
                OnExpired(this);
                if (_specialEffectObject != null)
                    UseSpriteAnimationEffect(_specialEffectObject.onBuffAppliedSpriteSequnce, 1);
                _target?.Character?.CreateBuff(ResourcesBase.GetBuff("destroy_corpse1")).Apply();
            };
        }

        public override void UpdateState()
        {
            bool isTargetCorpse = _target?.GetComponent<Corpse>() != null;
            if (isTargetCorpse)
            {
                ShowSelectionGUI();
            }
            else
            {
                _target?.Character.ChangeHealthValue(healthValue);
                durationLeft = 0;
                OnExpired(this);
            }
        }
    }
}