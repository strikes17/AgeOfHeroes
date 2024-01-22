using System.Collections.Generic;
using AgeOfHeroes.MapEditor;
using UnityEngine;

namespace AgeOfHeroes
{
    public abstract class DwellBuilding : SpecialBuilding
    {
        public List<string> coloredsNames = new List<string>();
        protected int positionX, positionY;

        public Vector2Int Position
        {
            get => new Vector2Int(positionX, positionY);
            set
            {
                positionX = value.x;
                positionY = value.y;
            }
        }
        public virtual void OnOwnerChanged(Player newOwner)
        {
            
        }
    }
}