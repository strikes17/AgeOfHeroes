using System;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUIManager : MonoBehaviour
    {
        public delegate void OnGUIElementGroupEventDelegate();

        [SerializeField] private Transform _mainCanvas;
        [SerializeField] private SkipTurnButton _skipTurnButton;
        [SerializeField] private GUITurnOfPlayerIndicatorWidget _turnOfPlayerIndicatorWidget;
        [SerializeField] private GUIHeroSkillTreeButton _heroSkillTreeButton;
        [SerializeField] private GUIHeroPortraitWidget _heroPortraitWidget;
        [SerializeField] private GUIRecentCharactersWidget _recentCharactersWidget;
        [SerializeField] private GUICharacterInfoWindow _characterInfoWindow;
        [SerializeField] private GUICharacterInfoButton _characterInfoButton;
        [SerializeField] private GUICombatInfoDialogue _guiCombatInfoDialogue;
        [SerializeField] private GUIPauseMenuWindow _pauseMenuWindow;
        [SerializeField] private Button _menuButton;
        [SerializeField] private GUICancelSpellButtonWidget _cancelSpellButtonWidget;
        [SerializeField] private GUIGameResultWindow _gameResultWindow;
        [SerializeField] private GUISiegeResultDialogue _siegeResultDialogue;
        [SerializeField] private GUISwitchAIButton _guiSwitchAIButton;
        [SerializeField] private GUIEscapeSiegeButton _guiEscapeSiegeButton;
        [SerializeField] private GUICastleQuickButton _guiCastleQuickButton;
        [SerializeField] private Image _bgLockImage;
        public GUILeftSidebar LeftSidebar;

        private GUICollectArtifactDialogue _collectArtifactDialogue;

        public GUIHeroPortraitWidget HeroPortraitWidget => _heroPortraitWidget;

        public GUIRecentCharactersWidget RecentCharactersWidget => _recentCharactersWidget;

        public GUICharacterInfoWindow CharacterInfoWindow => _characterInfoWindow;

        public GUICharacterInfoButton CharacterInfoButton => _characterInfoButton;

        public GUICombatInfoDialogue CombatInfoDialogue => _guiCombatInfoDialogue;

        public SkipTurnButton skipTurnButton => _skipTurnButton;

        public Transform mainCanvas => _mainCanvas;

        public GUICollectArtifactDialogue collectArtifactDialogue => _collectArtifactDialogue;

        public GUICancelSpellButtonWidget CancelSpellButtonWidget => _cancelSpellButtonWidget;

        public GUICastleSiegePanel SiegePanel => LeftSidebar.GUICastleSiegePanel;

        public GUIHeroSkillTreeButton HeroSkillTreeButton => _heroSkillTreeButton;

        public GUIGameResultWindow GameResultWindow { get => _gameResultWindow; }
        public GUIPauseMenuWindow PauseMenuWindow { get => _pauseMenuWindow; }

        public GUISiegeResultDialogue SiegeResultDialogue => _siegeResultDialogue;

        public Image BgLockImage => _bgLockImage;

        public GUISwitchAIButton SwitchAIButton => _guiSwitchAIButton;

        public GUIEscapeSiegeButton EscapeSiegeButton => _guiEscapeSiegeButton;

        public GUICastleQuickButton CastleQuickButton => _guiCastleQuickButton;


        private void Awake()
        {
            _menuButton.onClick.AddListener(_pauseMenuWindow.Show);
            _pauseMenuWindow.Hide();
            _siegeResultDialogue.Hide();
            _guiCastleQuickButton.Hide();
            _guiEscapeSiegeButton.Hide();
            _guiCastleQuickButton.AddListener(() =>
            {
                LeftSidebar.Show();
                LeftSidebar.OpenCastleMainMenu();
            });
            // _guiSiegeEscapeButton.AddListener(() =>
            // {
            //     LeftSidebar.Show();
            //     LeftSidebar.OpenSiegeMenu();
            // });
            // LeftSidebar.Opened += () =>
            // {
            //     HideQuickButton();
            // };
            // LeftSidebar.Closed += () =>
            // {
            //     ShowQuickButton();
            // };
            BgLockImage.gameObject.SetActive(false);
        }

        private void ShowQuickButton()
        {
            _guiCastleQuickButton.Show();
        }

        private void HideQuickButton()
        {
            _guiCastleQuickButton.Hide();
        }

        public void OnSwitchTurnUpdateGUI(PlayerColor playerColor)
        {
            var player = GameManager.Instance.MapScenarioHandler.players[playerColor];
            var color = GlobalVariables.playerColors[playerColor];
            _turnOfPlayerIndicatorWidget.ImageColor = color;
            player.selectedCharacter = player.previousSelectedCharacter = null;
            _recentCharactersWidget.TopTierCharacter = player.topTierCharacter;
            _recentCharactersWidget.SetGUI( player.selectedCharacter, player.previousSelectedCharacter);
            UpdateInfoWindowState(player.selectedCharacter);
        }

        public void UpdateInfoWindowState(ControllableCharacter controllableCharacter)
        {
            _characterInfoButton.SetGUI(controllableCharacter);
            _characterInfoWindow.SetCharacterInfoForWorld(controllableCharacter);
        }

        public GUICharacterInfoWindow CreateInfoWindowInstance(CharacterObject characterObject, CastleTierIncomeValues incomeValues)
        {
            var infoWindowPrefab = ResourcesBase.GetPrefab("GUI/Character Info Window");
            var infoWindowInstance = GameObject.Instantiate(infoWindowPrefab, Vector3.zero, Quaternion.identity, _mainCanvas);
            var infoWindowComponent = infoWindowInstance.GetComponent<GUICharacterInfoWindow>();
            var rectTransform = infoWindowInstance.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(-rectTransform.rect.width / 2f, rectTransform.rect.height / 2f);
            infoWindowComponent.SetCharacterInfoForCastle(characterObject, incomeValues);
            infoWindowComponent.Show();
            return infoWindowComponent;
        }

        public GUICharacterInfoWindow CreateInfoWindowInstance(ControllableCharacter controllableCharacter)
        {
            var infoWindowPrefab = ResourcesBase.GetPrefab("GUI/Character Info Window");
            var infoWindowInstance = GameObject.Instantiate(infoWindowPrefab, Vector3.zero, Quaternion.identity, _mainCanvas);
            var infoWindowComponent = infoWindowInstance.GetComponent<GUICharacterInfoWindow>();
            var rectTransform = infoWindowInstance.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(-rectTransform.rect.width / 2f, rectTransform.rect.height / 2f);
            infoWindowComponent.SetCharacterInfoForWorld(controllableCharacter);
            infoWindowComponent.Show();
            return infoWindowComponent;
        }

    }
}