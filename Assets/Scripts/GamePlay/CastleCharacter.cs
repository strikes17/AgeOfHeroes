using System.Collections;
using UnityEngine;

namespace AgeOfHeroes
{

    public class CastleCharacter : MonoBehaviour
    {
        public void OnClicked(Castle castle, ControllableCharacter controllableCharacter)
        {
            castle.AddGarnisonCharacterStack(controllableCharacter);
        }
    }
}