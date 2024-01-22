using UnityEngine;

namespace AgeOfHeroes
{
    public class SiegeActionCell : AbstractActionCell
    {
        public SiegeActionCell()
        {
            _color = new Color(0.83f, 0.4f, 0.05f);
            _sprite = ResourcesBase.GetSprite("action_cell_attack");
        }

        public override void OnActionCellActivated(Player player, ActionCell actionCell)
        {
            var siegePanel = GameManager.Instance.GUIManager.SiegePanel;
            var siegedCastle =
                player.ActiveTerrain.TerrainNavigator.NavigationMap.GetObjectOfType<Castle>(actionCell.Position,
                    LayerMask.NameToLayer("Building"));
            CastleSiegeData castleSiegeData = new CastleSiegeData();
            castleSiegeData.attackingPlayerData.Player = player;
            castleSiegeData.siegedCastle = siegedCastle;
            castleSiegeData.defendingPlayerData.Player = siegedCastle.Player;
            castleSiegeData.SiegedCastleTerrain = siegedCastle.CastleTerrain;
            siegePanel.SetupSiegeData(castleSiegeData);
            siegePanel.Show();
            player.SetActiveContext(PlayerContext.PlanningSiege);
            var nearbyCharacters = player.ActiveTerrain.TerrainNavigator.NavigationMap
                .GetAllObjectsOfType<ControllableCharacter>(actionCell.Position, 6, LayerMask.NameToLayer("Character"));
            for (int i = 0; i < nearbyCharacters.Count; i++)
            {
                var character = nearbyCharacters[i];
                if (character.playerOwnerColor != player.Color) continue;
                player.actionCellPool.CreateActionCellAtPosition(character.Position,
                    GlobalVariables.SiegeApplyCharacterCell);
            }
        }
    }
}