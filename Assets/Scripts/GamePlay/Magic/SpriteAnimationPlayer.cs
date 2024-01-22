using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes.Spell
{
    public class SpriteAnimationPlayer
    {
        private Dictionary<int, SpriteAnimationUnit> _animations = new Dictionary<int, SpriteAnimationUnit>();
        private SpriteAnimationUnit _animationPrefab;

        public static SpriteAnimationPlayer Instance =>
            _instance = _instance == null ? new SpriteAnimationPlayer() : _instance;

        private static SpriteAnimationPlayer _instance;

        public SpriteAnimationPlayer()
        {
            _animationPrefab = ResourcesBase.GetPrefab("animation").GetComponent<SpriteAnimationUnit>();
        }

        public SpriteAnimationUnit PlayAnimation(Vector3 position,
            SpriteAnimationSequenceObject animationSequenceObject, int cycles = -1, float offsetX = 0f, float offsetY = 0f)
        {
            var animationInstance = GameObject.Instantiate(_animationPrefab, position, Quaternion.identity);
            int key = animationInstance.GetHashCode();
            _animations.TryAdd(key, animationInstance);
            animationInstance.Expired += () => { _animations.Remove(key); };
            animationInstance.Play(animationSequenceObject, cycles, offsetX, offsetY);
            return animationInstance;
        }

        public void StopAnimation(int key)
        {
            SpriteAnimationUnit animationUnit = null;
            if (_animations.TryGetValue(key, out animationUnit))
            {
                animationUnit.Stop();
            }
        }
    }
}