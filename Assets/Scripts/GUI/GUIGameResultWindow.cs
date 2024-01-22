using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUIGameResultWindow : GUIDialogueWindow
    {
        [SerializeField] private Button _okButton;
        [SerializeField] private Image _bannerImage;
        [SerializeField] private TMP_Text _titleText;
        private PlayerColor _targetPlayer;

        public event PlayerGameStateEventDelegate PlayerWon
        {
            add => playerWon += value;
            remove => playerWon -= value;
        }
        
        public event PlayerGameStateEventDelegate PlayerLost
        {
            add => playerLost += value;
            remove => playerLost -= value;
        }

        private event PlayerGameStateEventDelegate playerWon, playerLost;
        public delegate void PlayerGameStateEventDelegate(PlayerColor playerColor);

        private void SetVisuals(PlayerColor targetPlayer)
        {
            _targetPlayer = targetPlayer;
            _bannerImage.sprite = ResourcesBase.GetPlayerBanner(GlobalVariables.playerBanners[_targetPlayer]);
            Show();
        }

        public void SetPlayerLoseState(PlayerColor targetPlayer)
        {
            SetVisuals(targetPlayer);
            _titleText.text = $"player {_targetPlayer} lost";
            _okButton.onClick.RemoveAllListeners();
            _okButton.onClick.AddListener(Hide);
            OnPlayerLost(targetPlayer);
        }

        public void SetPlayerWinState(PlayerColor targetPlayer)
        {
            SetVisuals(targetPlayer);
            _titleText.text = $"player {_targetPlayer} won";
            _okButton.onClick.RemoveAllListeners();
            _okButton.onClick.AddListener(Hide);
            _okButton.onClick.AddListener(Menu);
            OnPlayerWon(targetPlayer);
        }

        private void Menu()
        {
            GameManager.Instance.GUIManager.PauseMenuWindow.LoadMainMenu();
        }

        protected virtual void OnPlayerWon(PlayerColor playercolor)
        {
            playerWon?.Invoke(playercolor);
        }

        protected virtual void OnPlayerLost(PlayerColor playercolor)
        {
            playerLost?.Invoke(playercolor);
        }
    }
}