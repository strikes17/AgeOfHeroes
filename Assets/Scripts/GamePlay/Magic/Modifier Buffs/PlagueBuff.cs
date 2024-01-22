using System.Linq;
using UnityEngine;

namespace AgeOfHeroes.Spell
{
    public class PlagueBuff : ModifierBuff
    {
        public float baseAttackReduce, baseDamageReduce, baseMovementReduce;
        public float attackRatio, damageRatio, movementRatio;
        public int baseDamage;
        public int damageDecrease;
        private FloatModifier _moveModifier, _attackModifier, _damageModifier;
        private int _moveInstanceId, _attackInstanceId, _damageInstanceId;
        private DamageBuff _damageBuff;

        public override bool IsNotDebuff()
        {
            return false;
        }

        public override void Init(MagicSpellCombatData magicSpellCombatData)
        {
            base.Init(magicSpellCombatData);
            _damageBuff = new DamageBuff();
            _damageBuff.Init(magicSpellCombatData);
            _damageBuff.value = baseDamage;
            _damageBuff.baseDuration = baseDuration;
            _moveModifier = new FloatModifier(baseMovementReduce);
            _attackModifier = new FloatModifier(baseAttackReduce);
            _damageModifier = new FloatModifier(baseDamageReduce);
            SetModifier(_moveModifier, ref _moveInstanceId);
            SetModifier(_attackModifier, ref _attackInstanceId);
            SetModifier(_damageModifier, ref _damageInstanceId);
            _controllableCharacter.moveSpeedModifiers.Add(_moveModifier);
            _controllableCharacter.attackModifiers.Add(_attackModifier);
            _controllableCharacter.damageModifiers.Add(_damageModifier);
        }

        private void SetModifier(FloatModifier modifier, ref int instanceId)
        {
            modifier.Operation = Operation;
            instanceId = modifier.id;
        }

        public override void UpdateState()
        {
            if (!active)
                return;
            if (baseDuration < 0)
                return;

            if (durationLeft < baseDuration)
            {
                _moveModifier.value += movementRatio;
                _damageModifier.value += damageRatio;
                _attackModifier.value += attackRatio;
                _damageBuff.value -= damageDecrease;
            }
            _damageBuff.UpdateState();
            durationLeft--;
            if (_specialEffectObject != null)
                UseSpriteAnimationEffect(_specialEffectObject.onBuffUpdateSpriteSequenceObject, 1);
            if (durationLeft <= 0)
            {
                OnExpired(this);
            }
        }

        public override void OnExpired(Buff buff)
        {
            var modifier = _controllableCharacter.moveSpeedModifiers.Where(x => x.id == _moveInstanceId)
                .FirstOrDefault();
            _controllableCharacter.moveSpeedModifiers.Remove(modifier);
            modifier = _controllableCharacter.damageModifiers.Where(x => x.id == _damageInstanceId).FirstOrDefault();
            _controllableCharacter.damageModifiers.Remove(modifier);
            modifier = _controllableCharacter.attackModifiers.Where(x => x.id == _attackInstanceId).FirstOrDefault();
            _controllableCharacter.attackModifiers.Remove(modifier);
            base.OnExpired(buff);
        }
    }
}