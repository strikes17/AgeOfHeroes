using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace AgeOfHeroes.MapEditor
{
    public class MapEditorCharacter : MapEditorEntity
    {
        public CharacterObject CharacterObject => _characterObject;
        [SerializeField] private SpriteRenderer _quantitySpriteBg;
        [SerializeField] private TMP_Text _quantityText;
        private int _countInStack;
        private PlayerColor _playerOwnerColor;
        private CharacterObject _characterObject;
        private Fraction _fraction;
        private GUIMapEditorCharacterInfoWindow _infoWindow;

        public PlayerColor PlayerOwnerColor
        {
            set
            {
                _playerOwnerColor = value;
                _quantitySpriteBg.color = GlobalVariables.playerColors[_playerOwnerColor];
            }
            get { return _playerOwnerColor; }
        }
        
        public int countInStack
        {
            get => _countInStack;
            set
            {
                _countInStack = value;
                _quantityText.text = _countInStack.ToString();
            }
        }

        public Fraction fraction => _fraction;

        public void Init(CharacterObject characterObject)
        {
            _characterObject = characterObject;
            _spriteRenderer.sprite = _characterObject.mainSprite;
            countInStack = _characterObject.fullStackCount;
            _fraction = characterObject.Fraction;
        }

        public override void OnClicked()
        {
            MapEditorManager.Instance.SelectedMapCharacter = this;
            if (_infoWindow == null)
                _infoWindow = MapEditorManager.Instance.GUIMapEditorManager.CreateMapCharacterInfoWindowInstance(this);
            _infoWindow.Closed = () => { _infoWindow = null; };
        }
    }
}