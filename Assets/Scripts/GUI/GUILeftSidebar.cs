using System;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUILeftSidebar : MonoBehaviour
    {
        public Button hideButton;
        public GUICastleMenu GUICastleMenu;
        public GUICharacterShopMenu GUICharacterShopMenu;
        public GUISpecialBuildsMenu GUISpecialBuildsMenu;
        public GUICastleGarnisonPanel GUICastleGarnisonPanel;
        public GUICastleSiegePanel GUICastleSiegePanel;
        private event GUIManager.OnGUIElementGroupEventDelegate opened, closed;
        [SerializeField] private ScrollRect _scrollRect;


        public void OpenCharacterShopMenu()
        {
            GUISpecialBuildsMenu.Hide();
            GUICastleMenu.Hide();
            GUICharacterShopMenu.Show();
            _scrollRect.content = GUICharacterShopMenu.rectTransform;
        }

        public void OpenCastleMainMenu()
        {
            GUICastleMenu.Show();
            GUICharacterShopMenu.Hide();
            GUISpecialBuildsMenu.Hide();
            _scrollRect.content = GUICastleMenu.rectTransform;
        }

        public void OpenSpecialBuildingsMenu()
        {
            GUISpecialBuildsMenu.Show();
            GUICastleMenu.Hide();
            GUICharacterShopMenu.Hide();
            _scrollRect.content = GUISpecialBuildsMenu.rectTransform;
        }

        private void Awake()
        {
            GUICastleMenu.castleMenuButton.button.onClick.AddListener(OpenCastleMainMenu);
            GUICastleMenu.characterShopMenuButton.button.onClick.AddListener(OpenCharacterShopMenu);
            GUICastleMenu.specialBuildingsMenuButton.button.onClick.AddListener(OpenSpecialBuildingsMenu);
            GUICastleMenu.garnisonMenuButton.button.onClick.AddListener(GUICastleGarnisonPanel.Toggle);
            GUICastleGarnisonPanel.Init();
            GUICastleGarnisonPanel.Hide();
            GUICastleSiegePanel.Hide();
            GUICastleGarnisonPanel.Opened += () => { GUICastleSiegePanel.Hide(); };
            GUICastleSiegePanel.Opened += () => { GUICastleGarnisonPanel.Hide(); };
            Hide();
        }

        public event GUIManager.OnGUIElementGroupEventDelegate Opened
        {
            add => opened += value;
            remove => opened -= value;
        }

        public event GUIManager.OnGUIElementGroupEventDelegate Closed
        {
            add => closed += value;
            remove => closed -= value;
        }

        public void ShowForCastleInternal()
        {
            OpenCastleMainMenu();
            foreach (Transform c in transform)
            {
                c.gameObject.SetActive(true);
            }
            hideButton.onClick.RemoveAllListeners();
            hideButton.onClick.AddListener(OpenCastleMainMenu);
            opened?.Invoke();
        }

        public void Show()
        {
            hideButton.onClick.RemoveAllListeners();
            hideButton.onClick.AddListener(Hide);
            OpenCastleMainMenu();
            foreach (Transform c in transform)
            {
                c.gameObject.SetActive(true);
            }

            opened?.Invoke();
        }

        public void Hide()
        {
            foreach (Transform c in transform)
            {
                c.gameObject.SetActive(false);
            }
            
            closed?.Invoke();
        }
    }
}