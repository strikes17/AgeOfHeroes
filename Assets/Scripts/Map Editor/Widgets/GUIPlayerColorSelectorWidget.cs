using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes.MapEditor
{
    public class GUIPlayerColorSelectorWidget : GUIMapEditorWidget
    {
        private PlayerColor _playerColor;
        private Image _playerColorButtonImage;

        public void Set(PlayerColor playerColor, Image playerColorButtonImage)
        {
            _playerColorButtonImage = playerColorButtonImage;
            _button.onClick.RemoveAllListeners();
            _playerColor = playerColor;
            var playerBanner = ResourcesBase.GetPlayerBanner(GlobalVariables.playerBanners[_playerColor]);
            _image.sprite = playerBanner;
            _button.onClick.AddListener(SelectCharacter);
        }

        private void SelectCharacter()
        {
            _playerColorButtonImage.sprite = ResourcesBase.GetPlayerBanner(GlobalVariables.playerBanners[_playerColor]);
            MapEditorManager.Instance.SelectedPlayerColor = _playerColor;
        }
    }
}