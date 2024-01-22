using UnityEngine;

namespace AgeOfHeroes.MapEditor
{
    public class MapEditorDwelling : MapEditorEntity
    {
        public PlayerColor PlayerColor
        {
            set
            {
                _playerColor = value;
                foreach (Transform child in transform)
                {
                    var colored = child.GetComponent<Colored>();
                    if (colored != null) colored.PlayerOwnerColor = _playerColor;
                }
            }
            get { return _playerColor; }
        }

        private PlayerColor _playerColor;
        private DwellBuilding _dwellBuilding;

        public DwellBuilding DwellBuilding
        {
            set
            {
                _dwellBuilding = value;
            }
            get => _dwellBuilding;
        }

        public override void OnClicked()
        {
            base.OnClicked();
            ShowContextMenu();
        }

        public void ShowContextMenu()
        {
            if (DwellBuilding is ResourceIncomeDwell)
            {
            }
            else if (DwellBuilding is CharacterIncomeDwell)
            {
            }
        }
    }
}