using UnityEngine;

namespace AgeOfHeroes
{
    public class DropArtifactActionCell : AbstractActionCell
    {
        public DropArtifactActionCell()
        {
            _color = new Color(0.83f, 0.21f, 0f);
            _sprite = ResourcesBase.GetSprite("action_cell_move");
        }

        public override void OnActionCellActivated(Player player, ActionCell actionCell)
        {
            var artifact = GameManager.Instance.GUIManager.HeroPortraitWidget.ClearSelectedArtifactSlot();
            var position = new Vector3Int(actionCell.Position.x, actionCell.Position.y, 0);
            var artifactBehaviour = GameManager.Instance.SpawnManager.SpawnArtifact(artifact.ArtifactObject,
                position, player.ActiveTerrain);
            player.selectedCharacter.DeselectAndRemoveControll(player.Color);
        }
    }
}