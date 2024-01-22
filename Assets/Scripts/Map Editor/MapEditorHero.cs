using TMPro;
using UnityEngine;

namespace AgeOfHeroes.MapEditor
{
    public class MapEditorHero : MapEditorEntity
    {
        [SerializeField] private SpriteRenderer _healthSpriteBg;
        [SerializeField] private TMP_Text _healthText;

        public HeroObject HeroObject => _heroObject;
        public Fraction fraction => _fraction;

        private int _startingLevel;
        private HeroObject _heroObject;
        private PlayerColor _playerColor;
        private Fraction _fraction;
        private int _health;

        public PlayerColor playerColor
        {
            set
            {
                _playerColor = value;
                _healthSpriteBg.color = GlobalVariables.playerColors[_playerColor];
            }
            get { return _playerColor; }
        }

        public int health
        {
            get => _health;
            set
            {
                _health = value;
                _healthText.text = _health.ToString();
            }
        }


        public void Init(HeroObject heroObject)
        {
            _heroObject = heroObject;
            _spriteRenderer.sprite = _heroObject.mainSprite;
            health = _heroObject.startingHealth;
            _fraction = heroObject.Fraction;
        }

        public override void OnClicked()
        {
            MapEditorManager.Instance.SelectedMapHero = this;
            // if (_infoWindow == null)
            //     _infoWindow = MapEditorManager.Instance.GUIMapEditorManager.CreateMapCharacterInfoWindowInstance(this);
            // _infoWindow.Closed = () => { _infoWindow = null; };
        }
    }
}