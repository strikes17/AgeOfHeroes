using AgeOfHeroes.Spell;
using UnityEngine;

namespace AgeOfHeroes
{
    public class EditCharactersPositionActionCell : AbstractActionCell
    {
        public EditCharactersPositionActionCell()
        {
            _color = new Color(0.4f, 0.21f, 0.7f);
            _sprite = ResourcesBase.GetSprite("action_cell_move");
        }

        public override void OnActionCellActivated(Player player, ActionCell actionCell)
        {
            int iPosX = actionCell.Position.x;
            int iPosY = actionCell.Position.y;
            var character = player.ActiveTerrain.TerrainNavigator.NavigationMap.GetCharacterAtPosition(
                new Vector2Int(iPosX, iPosY));
            if (character == null)
            {
                PlaceCharacterForPlayer(player, new Vector2Int(iPosX, iPosY));
            }
            else
            {
                bool isAnAlly = character.playerOwnerColor == player.Color;
                if (isAnAlly)
                    RemoveCharacterForPlayer(player, character);
            }
        }

        private void RemoveCharacterForPlayer(Player player, ControllableCharacter controllableCharacter)
        {
            var castleCharacter = controllableCharacter.GetComponent<CastleCharacter>();

            if (controllableCharacter.CurrentState == ControllableCharacter.State.Combat)
                return;
            player.LastSelectedCastle.AddGarnisonCharacterStack(controllableCharacter);

        }

        private void PlaceCharacterForPlayer(Player player, Vector2Int position)
        {
            var garnison = GameManager.Instance.GUIManager.LeftSidebar.GUICastleGarnisonPanel;
            var character = garnison.SelectedGarnisonCharacter;
            if (character != null)
            {
                garnison.RemoveCharacterSlot(character);
                garnison.SelectedGarnisonCharacter = null;
                character.PlaceOnTerrain(player.ActiveTerrain);
                character.Position = position;
                character.gameObject.SetActive(true);
                player.LastSelectedCastle.RemoveFromGarnisonCharacterStack(character);

            }
        }
    }
}