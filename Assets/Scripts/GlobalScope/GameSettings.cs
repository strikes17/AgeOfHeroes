using UnityEngine;

namespace AgeOfHeroes
{
    [CreateAssetMenu(fileName ="New Game Settings", menuName = "Create Game Settings")]
    public class GameSettings : ScriptableObject
    {
        public bool spellBookEnabled = true;
        public bool heroesLevelEnabled = true;
        public bool noFogOfWar = false;
    }
}