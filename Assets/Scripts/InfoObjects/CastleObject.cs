using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes
{
    [CreateAssetMenu(fileName = "New Castle", menuName = "Create New Castle")]
    public class CastleObject : ScriptableObject
    {
        public string internalName;
        public TextAsset castleInfoAsset;
        public Fraction fraction;
        public int defaultStartingTier, maxTiers;
        public List<Sprite> tierSprite;
    }
}