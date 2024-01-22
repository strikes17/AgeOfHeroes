using System;
using UnityEngine;

namespace AgeOfHeroes
{
    public class SpriteGUIQuickBuyManager : MonoBehaviour
    {
        private GameObject shopCellPrefab;

        private void Awake()
        {
            shopCellPrefab = ResourcesBase.GetPrefab("Character Shop Cell");
            
        }
    }
}