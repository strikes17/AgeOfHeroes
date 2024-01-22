using System;
using UnityEngine;

namespace AgeOfHeroes.Spell
{
    public class DestroyGameObjectBuff : Buff
    {
        [NonSerialized] private GameObject _gameObjectToDestroy;
        public override bool IsNotDebuff()
        {
            return false;
        }
        public override void Init(MagicSpellCombatData magicSpellCombatData)
        {
            base.Init(magicSpellCombatData);
            _gameObjectToDestroy = magicSpellCombatData.target.gameObject;
        }

        public override void Apply()
        {
            OnExpired(this);
            var controllableCharacter = _gameObjectToDestroy.GetComponent<ControllableCharacter>();
            if (controllableCharacter != null)
            {
                var corpse = _gameObjectToDestroy.GetComponent<Corpse>();
                if(corpse != null)
                {
                    corpse.Banish();
                    return;
                }
                controllableCharacter.DestroyStack();
            }
        }
    }
}