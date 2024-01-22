using System;
using UnityEngine;

namespace AgeOfHeroes.Spell
{
    [Serializable]
    public class DamageBuff : Buff
    {
        public int value = 0;
        public bool isLethal = true;
        private ControllableCharacter _controllableCharacter;

        public override bool IsNotDebuff()
        {
            return false;
        }

        public override void Init(MagicSpellCombatData magicSpellCombatData)
        {
            base.Init(magicSpellCombatData);
            _controllableCharacter = (ControllableCharacter)this._target;
        }
        
        public override void OnExpired(Buff buff)
        {
            base.OnExpired(buff);
            if (_specialEffectObject == null)
                return;
            var animation = UseSpriteAnimationEffect(_specialEffectObject.onBuffExpiredSpriteSequenceObject, 1);
        }

        public override void UpdateState()
        {
            if (!isLethal)
                value = Mathf.Clamp(value, 0, _controllableCharacter.HealthLeft - 1);
            
            _controllableCharacter.ChangeHealthValue(-value, _caster.Character);
            GameManager.Instance.SoundManager.Play3DAudioClip(_specialEffectObject?.onBuffUpdateAudioClip,
                _target.transform.position, false);
            base.UpdateState();
        }
    }
}