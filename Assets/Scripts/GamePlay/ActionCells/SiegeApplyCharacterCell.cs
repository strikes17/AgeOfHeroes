using UnityEngine;

namespace AgeOfHeroes
{
    public class SiegeApplyCharacterCell : AbstractActionCell
    {
        public SiegeApplyCharacterCell()
        {
            _color = new Color(0.63f, 0.4f, 0.05f);
            _sprite = ResourcesBase.GetSprite("action_cell_attack");
        }

        public override void OnActionCellActivated(Player player, ActionCell actionCell)
        {
            var siegePanel = GameManager.Instance.GUIManager.SiegePanel;
            var character =
                player.ActiveTerrain.TerrainNavigator.NavigationMap.GetCharacterAtPosition(actionCell.Position);
            var slot = siegePanel.AddCharacterSlot(character);
            actionCell.gameObject.SetActive(false);
            // player.SiegeData.attackingPlayerData.charsPositionBeforeSiege.TryAdd(character, character.Position);

            slot.button.onClick.AddListener((() =>
            {
                siegePanel.RemoveCharacterSlot(character);
                actionCell.gameObject.SetActive(true);
                // player.SiegeData.attackingPlayerData.charsPositionBeforeSiege.Remove(character);
            }));
        }
    }
}