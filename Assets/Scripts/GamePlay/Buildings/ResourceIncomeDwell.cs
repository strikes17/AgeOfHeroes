using AgeOfHeroes.MapEditor;
using UnityEngine;

namespace AgeOfHeroes
{
    public class ResourceIncomeDwell : DwellBuilding
    {
        public int goldValue, gemsValue;
        protected Player _owner;

        public override void OnOwnerChanged(Player newOwner)
        {
            base.OnOwnerChanged(newOwner);
            if (_owner != null)
                _owner.TurnRecieved -= OnRoundUpdate;
            _owner = newOwner;
            newOwner.TurnRecieved += OnRoundUpdate;
        }

        protected void OnRoundUpdate(Player player)
        {
            if (player.Color == PlayerColor.Neutral) return;
            Sprite goldSprite = ResourcesBase.GetSprite("ResourceGold");
            FloatingText.Create(Position, 0.05f, 0.05f).MakeFloat(new Color(0.765f, 0.65f, 0f), goldValue, goldSprite);
            player.Gold += goldValue;
            player.Gems += gemsValue;
        }
    }
}