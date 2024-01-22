using System.Collections.Generic;
using AgeOfHeroes.Spell;
using UnityEngine;

namespace AgeOfHeroes
{
    public class ControllableCharacterObject : ScriptableObject
    {
        public Fraction Fraction;
        public string title, description;
        public AudioClip[] voiceAttackAudioClips;
        public AudioClip[] weaponAttackAudioClips;
        public AudioClip[] idleAudioClips;
        public AudioClip[] painAudioClips;
        public AudioClip[] deathAudioClips;
        public AudioClip[] cheerAudioClips;
        public SpriteAnimationSequenceObject IdleSpriteAnimation;
        public SpriteAnimationSequenceObject ShootAnimation;
        public List<string> _combatAbilityNames;
        public string spellBookInternalName;
        public List<Sprite> idleAnimation, attackAnimation, moveAnimation, specialAnimation, magicAnimation;
        public Sprite mainSprite, corpseSprite;
        public int tier = 1;
        public int startingMovementPoints;
        public int startingHealth, startingMana;
        public int damageValue, attackValue, defenseValue, attackRange;
        public bool isFlying, canCrossWater;
        public bool isShooter, isWizard;
        public int attacksCount, retilationsCount, rangedRetilationsCount;
        public List<MagicSpellAllowedTarget> persona;
        public int shoots;
        public bool isHuge;
        public int visionRadius;
        public int experienceValue;
    }
}