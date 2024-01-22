using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes.Spell
{
    [CreateAssetMenu(fileName = "Sprite Animation Sequence Object", menuName = "New Sprite Animation Sequence Object", order = 0)]
    public class SpriteAnimationSequenceObject : ScriptableObject
    {
        public Vector2 Offset;
        public Color Color = Color.white;
        public float fps;
        public List<Sprite> Sprites;
    }
}