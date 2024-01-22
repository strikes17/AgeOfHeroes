using UnityEngine;

namespace AgeOfHeroes
{
    [CreateAssetMenu(fileName = "Treasure Object", menuName = "Create New Treasure Object")]
    public class TreasureObject : ScriptableObject
    {
        public Sprite Icon;
        public int goldValue, gemsValue, experienceValue;
    }
}