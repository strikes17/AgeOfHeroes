using System.Collections;
using UnityEngine;

namespace AgeOfHeroes
{
    public class SpriteGUIQucikInspectCharacterButton : MonoBehaviour, IClickTarget
    {
        public IEnumerator OnClicked(Player player)
        {
            Debug.Log($"{name}");
            yield return null;
        }
    }
}