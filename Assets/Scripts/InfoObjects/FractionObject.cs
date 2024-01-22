using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes.MapEditor
{
    [CreateAssetMenu(fileName = "New Fraction Object", menuName = "Create New Fraction Object")]
    public class FractionObject : ScriptableObject
    {
        public Sprite Icon;
        public Fraction Fraction;
        public string DefaultCastleObject;
        public List<string> Heroes;
    }
}