using System;
using System.Collections;
using AgeOfHeroes.Spell;
using UnityEngine;

namespace AgeOfHeroes
{
    public class Corpse : ControllableCharacter
    {
        public void Init(long personaMask)
        {
            persona = personaMask;
            _spriteRenderer.sprite = _controllableCharacterObject.corpseSprite;
            _spriteRenderer.sortingOrder = GlobalVariables.CorpseRenderOrder;
            bool wasAlive = (persona & (long)MagicSpellAllowedTarget.Alive) != 0;
            bool wasUndead = (persona & (long)MagicSpellAllowedTarget.Undead) != 0;
            if (wasAlive)
                persona |= (long)MagicSpellAllowedTarget.AliveCorpse;
            else if(wasUndead)
                persona |= (long)MagicSpellAllowedTarget.UndeadCorpse;
            // Destroy(gameObject, 60f);
        }

        private void OnDestroy()
        {
            _activeTerrain.SpawnedCorpses.Remove(this);
        }

        public IEnumerator OnClicked(PlayerColor playerColor)
        {
            yield return null;
        }
        
        public override bool IsAnAllyTo(ControllableCharacter character)
        {
            Debug.Log("Corpse!");
            return true;
        }
    }
}