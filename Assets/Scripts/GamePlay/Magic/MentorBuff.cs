using System;
using UnityEngine;
using System.Collections.Generic;

namespace AgeOfHeroes.Spell
{
    public class MentorBuff : Buff
    {
        public List<string> _fromCharacters = new List<string>();
        public List<string> _toCharacters = new List<string>();

        public override bool CheckConditions(MagicSpellCombatData magicSpellCombatData)
        {
            var target = magicSpellCombatData.target;
            var baseVersionCharacter = target.Character;
            var eliteFormObject = (baseVersionCharacter.BaseCharacterObject as CharacterObject).eliteFormObject;
            var indexOfEliteConvertToNextTier = _fromCharacters.IndexOf(baseVersionCharacter.BaseCharacterObject.name);
            if (eliteFormObject == null && indexOfEliteConvertToNextTier < 0)
            {
                var dialogueWindow = GUIDialogueFactory.CreateCommonDialogueWindow();
                dialogueWindow.Title = "Failed!";
                dialogueWindow.Description = "You can not mentor this unit anymore!";
                return false;
            }
            return true;
        }

        public override bool IsNotDebuff()
        {
            return true;
        }

        public override void UpdateState()
        {
            var destroyBuff = ResourcesBase.GetBuff("destroy_corpse1");
            var baseVersionCharacter = _target.Character;
            var eliteFormObject = (baseVersionCharacter.BaseCharacterObject as CharacterObject).eliteFormObject;
            var indexOfEliteConvertToNextTier = _fromCharacters.IndexOf(baseVersionCharacter.BaseCharacterObject.name);

            if (eliteFormObject == null)
            {
                var nextTierCharacterObject = ResourcesBase.GetCharacterObject(_toCharacters[indexOfEliteConvertToNextTier]);
                int count = Math.Clamp(baseVersionCharacter.Count, 1, nextTierCharacterObject.fullStackCount);
                var nextTierCharacterSpawned = GameManager.Instance.SpawnManager.SpawnCharacter(nextTierCharacterObject,
                    nextTierCharacterObject.Fraction,
                    baseVersionCharacter.playerOwnerColor, baseVersionCharacter.Position,
                    baseVersionCharacter.PlayableTerrain, count);
            }
            else
            {
                int count = Math.Clamp(baseVersionCharacter.Count, 1, eliteFormObject.fullStackCount);
                var eliteVersionCharacter = GameManager.Instance.SpawnManager.SpawnCharacter(eliteFormObject,
                    eliteFormObject.Fraction,
                    baseVersionCharacter.playerOwnerColor, baseVersionCharacter.Position,
                    baseVersionCharacter.PlayableTerrain, baseVersionCharacter.Count);
            }
            _target.Character.CreateBuff(destroyBuff).Apply();
            base.UpdateState();
        }
    }
}