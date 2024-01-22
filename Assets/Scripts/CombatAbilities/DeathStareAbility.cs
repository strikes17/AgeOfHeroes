using System.Collections;
using UnityEngine;

namespace AgeOfHeroes
{
    public class DeathStareAbility : SpecialAbility
    {
        // public float chance = 0.5f;
        private SoundManager _soundManager;
        [SerializeField] private string soundPath = "Magic/DeathStare";
        private string soundName;

        public override SpecialAbility Init(ControllableCharacter _gridCharacter)
        {
            _soundManager = GameManager.Instance.SoundManager;
            _soundManager.PreloadAudioClip(soundPath, (s => soundName = s));
            return base.Init(_gridCharacter);
        }

        public override IEnumerator Use(CombatData combatData)
        {
            var source = combatData.offensiveCharacter;
            var target = combatData.defensiveCharacter;
            _soundManager.Play3DAudioClip(soundName, target.transform.position, false);
            yield return new WaitForSeconds(2f);
            var damage = target.baseHealth;
            target.ChangeHealthValue(-damage);
        }
    }
}