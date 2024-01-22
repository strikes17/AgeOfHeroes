using System;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUICharacterInfoButton : MonoBehaviour
    {
        [SerializeField] private Button showInfoButton;
        [SerializeField] private Image characterBackground;
        private ControllableCharacter _controllableCharacter;
        private CharacterObject _characterObject;
        private CastleTierIncomeValues _incomeValues;
        public bool newWindow;

        private void Awake()
        {
            showInfoButton.onClick.AddListener(() => ShowCharacterInfoWindow(newWindow));
        }

        public void SetGUI(ControllableCharacter controllableCharacter)
        {
            _controllableCharacter = controllableCharacter;
            if (_controllableCharacter == null)
            {
                characterBackground.sprite = ResourcesBase.GetDefaultCharacterSprite();
                gameObject.SetActive(false);
            }
            else
            {
                characterBackground.sprite = _controllableCharacter.GetMainSprite();
                gameObject.SetActive(true);
            }
        }

        public void SetGUI(CharacterObject characterObject, CastleTierIncomeValues incomeValues)
        {
            _incomeValues = incomeValues;
            _characterObject = characterObject;
            characterBackground.sprite = _characterObject.mainSprite;
        }

        public void Test()
        {
            showInfoButton.onClick.RemoveAllListeners();
            showInfoButton.onClick.AddListener(TestShit);
        }

        public void TestShit()
        {
            GameManager.Instance.GUIManager.CharacterInfoWindow.SetCharacterInfoForWorld(_controllableCharacter);
            GameManager.Instance.GUIManager.CharacterInfoWindow.Show();
        }
        

        public void ShowCharacterInfoWindow(bool newWindowInstance = false)
        {
            if (_controllableCharacter == null && _characterObject == null)
                return;
            if (newWindowInstance)
            {
                var infoWindowInstance = GameManager.Instance.GUIManager.CreateInfoWindowInstance(_characterObject, _incomeValues);
                infoWindowInstance.temporaryInstance = true;
                infoWindowInstance.Show();
                return;
            }

            GameManager.Instance.GUIManager.CharacterInfoWindow.SetCharacterInfoForWorld(_controllableCharacter);
            GameManager.Instance.GUIManager.CharacterInfoWindow.Show();
        }
    }
}