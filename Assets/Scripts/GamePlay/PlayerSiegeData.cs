using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes
{
    public class PlayerSiegeData
    {
        public Player Player;
        public PlayableTerrain SiegedCastleTerrain;
        public List<ControllableCharacter> allCharacters = new List<ControllableCharacter>();
        public List<ControllableCharacter> appliedCharacters = new List<ControllableCharacter>();
        public Dictionary<ControllableCharacterObject, int> defeatedCharactersCount = new Dictionary<ControllableCharacterObject, int>();
        // public Dictionary<HeroObject, int> defeatedCharactersCount = new Dictionary<CharacterObject, int>();

        public int AddDefeatedCharacter(ControllableCharacterObject characterObject, int quantity)
        {
            if (defeatedCharactersCount.TryGetValue(characterObject, out int k))
            {
                defeatedCharactersCount[characterObject] += quantity;
            }
            else
            {
                defeatedCharactersCount.Add(characterObject, quantity);
            }
            return defeatedCharactersCount[characterObject];
        }

        public Dictionary<ControllableCharacter, Vector2Int> charsPositionBeforeSiege =
            new Dictionary<ControllableCharacter, Vector2Int>();
    }
}