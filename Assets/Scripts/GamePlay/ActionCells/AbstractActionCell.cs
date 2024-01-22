using UnityEngine;

namespace AgeOfHeroes
{
    public abstract class AbstractActionCell
    {
        public Color Color => _color;
        public Sprite Sprite => _sprite;
        protected Color _color;
        protected Sprite _sprite;
        public abstract void OnActionCellActivated(Player player, ActionCell actionCell);
    }
}