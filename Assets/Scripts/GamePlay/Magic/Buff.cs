using System;
using UnityEngine;

namespace AgeOfHeroes.Spell
{
    public enum BuffUpdateMoment
    {
        OnTurnStarted = 1,
        OnTurnEnded = 2,
        OnEnemyTurnStarted = 4,
        OnEnemyTurnEnded = 8,
        Instant = 16
    }

    public abstract class Buff : ICloneable
    {
        public string title;
        public string internalName;
        public string description;
        public string specialEffectName;
        public int baseDuration;
        public bool Active => active;
        public bool isDispellable = true;
        public bool stackingEnabled = false;
        public bool savingState = true;

        [NonSerialized] public int durationLeft;

        public abstract bool IsNotDebuff();

        public int buffUpdateMoment;
        public string spriteIconPath;
        [NonSerialized] public Sprite spriteIcon;
        [NonSerialized] protected bool active = true;
        [NonSerialized] protected AbstractMagicSpellTarget _target;
        [NonSerialized] protected AbstractMagicSpellTarget _caster;
        [NonSerialized] protected BuffSpecialEffectObject _specialEffectObject;

        protected SpriteAnimationPlayer _spriteAnimationPlayer = new SpriteAnimationPlayer();

        public delegate void OnBuffEventDelegate(Buff buff);

        public virtual void Init(MagicSpellCombatData magicSpellCombatData)
        {
            active = true;
            _target = magicSpellCombatData.target;
            _caster = magicSpellCombatData.source;
            spriteIcon = ResourcesBase.GetSprite(spriteIconPath);
            // durationLeft = baseDuration + 1;
            durationLeft = baseDuration;
        }

        public virtual void OnExpired(Buff buff)
        {
            if (_target != null)
                GameManager.Instance.SoundManager.Play3DAudioClip(_specialEffectObject?.onBuffExpiredAudioClip,
                    _target.transform.position, false);
            // Debug.Log($"{internalName} Buff Expired from {_target.name}");
            active = false;
            expired?.Invoke(buff);
        }

        protected SpriteAnimationUnit UseSpriteAnimationEffect(
            SpriteAnimationSequenceObject spriteAnimationSequenceObject, int cycles = -1)
        {
            if (spriteAnimationSequenceObject == null || _target == null) return null;
            var animation = _spriteAnimationPlayer.PlayAnimation(_target.transform.position,
                spriteAnimationSequenceObject, cycles);
            // animation.transform.SetParent(_target.transform);
            return animation;
        }

        public event OnBuffEventDelegate Expired
        {
            add => expired += value;
            remove => expired -= value;
        }

        private event OnBuffEventDelegate expired;

        public virtual void UpdateState()
        {
            if (!active)
                return;
            if (baseDuration < 0)
                return;
            durationLeft--;
            if (durationLeft <= 0)
            {
                OnExpired(this);
            }
        }

        public virtual void Apply()
        {
            _specialEffectObject = ResourcesBase.GetBuffSpecialEffect(specialEffectName);
            if (_specialEffectObject != null)
            {
                var animation = UseSpriteAnimationEffect(_specialEffectObject.onBuffAppliedSpriteSequnce,
                    _specialEffectObject.onBuffAppliedEffectCycles);
                GameManager.Instance.SoundManager.Play3DAudioClip(_specialEffectObject?.onBuffAppliedAudioClip,
                    _target.transform.position, false);
                Expired += buff =>
                {
                    if (animation.Looped)
                        _spriteAnimationPlayer.StopAnimation(animation.GetHashCode());
                };
            }

            UpdateState();
        }

        public virtual bool CheckConditions(MagicSpellCombatData magicSpellCombatData)
        {
            return true;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}