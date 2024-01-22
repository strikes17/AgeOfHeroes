using UnityEngine;

namespace AgeOfHeroes.Spell
{
    [CreateAssetMenu(fileName = "Buff Special Effect Object", menuName = "New Buff Special Effect Object", order = 0)]
    public class BuffSpecialEffectObject : ScriptableObject
    {
        public AudioClip onBuffAppliedAudioClip, onBuffUpdateAudioClip, onBuffExpiredAudioClip;
        public SpriteAnimationSequenceObject onBuffAppliedSpriteSequnce, onBuffUpdateSpriteSequenceObject, onBuffExpiredSpriteSequenceObject;
        public int onBuffAppliedEffectCycles = -1, onBuffUpdateEffectCycles = -1, onBuffExpiredEffectCycles = -1;
    }
}