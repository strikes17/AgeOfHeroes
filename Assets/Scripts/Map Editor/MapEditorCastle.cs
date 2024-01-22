using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace AgeOfHeroes.MapEditor
{
    public class MapEditorCastle : MapEditorEntity
    {
        [SerializeField] private new SpriteRenderer _castleSpriteRenderer;

        public PlayerColor PlayerOwnerColor
        {
            set
            {
                _playerOwnerColor = value;
                _coloreds.ForEach(x => x.PlayerOwnerColor = _playerOwnerColor);
            }
            get { return _playerOwnerColor; }
        }
        
        public Fraction Fraction;

        protected override void Awake()
        {
            base.Awake();
            UniqueId = GetInstanceID();
        }

        public int UniqueId
        {
            set => _uniqueId =_uniqueId == 0 ? value : _uniqueId;
            get => _uniqueId;
        }

        private int _uniqueId;
        [SerializeField] private CastleObject _castleObject;
        [SerializeField] private List<Colored> _coloreds;
        private CastleInfo _castleInfo;
        private PlayerColor _playerOwnerColor;
        private int _tier;

        public override void OnClicked()
        {
            base.OnClicked();
        }

        public CastleObject castleObject
        {
            get => _castleObject;
            set
            {
                _castleObject = value;
                Fraction = _castleObject.fraction;
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.TypeNameHandling = TypeNameHandling.All;
                settings.Formatting = Formatting.Indented;
                var asset = _castleObject.castleInfoAsset;
                var castleJson = JsonConvert.DeserializeObject<CastleInfo>(asset.text, settings);
                _castleInfo = castleJson;
            }
        }

        public int Tier
        {
            get => _tier;
            set
            {
                _tier = value;
                if (_tier > _castleObject.maxTiers)
                    _tier = _castleObject.maxTiers;
                if (_tier < 0)
                    _tier = 0;
                _castleSpriteRenderer.sprite = _castleObject.tierSprite[_tier];
            }
        }

        public CastleInfo castleInfo => _castleInfo;
    }
}