using AgeOfHeroes.MapEditor;
using UnityEngine;

namespace AgeOfHeroes
{
    public class AbstractTreasure : AbstractCollectable
    {
        protected TreasureObject _treasureObject;
        protected int _goldValue, _gemsValue, _experienceValue;
        protected TreasureType _chosenTreasureType;

        public int GoldValue => _goldValue;

        public int GemsValue => _gemsValue;

        public int ExperienceValue => _experienceValue;

        public TreasureObject TreasureObject => _treasureObject;

        public override void ShowDialogue(Hero heroCollector)
        {
        }

        public virtual void Set(TreasureObject treasureObject)
        {
            _treasureObject = treasureObject;
        }

        public void LoadFromSerializable(SerializableTreasure serializableTreasure)
        {
            _goldValue = serializableTreasure.Gold;
            _gemsValue = serializableTreasure.Gems;
            _experienceValue = serializableTreasure.Exp;
            Position = new Vector2Int(serializableTreasure.positionX, serializableTreasure.positionY);
        }

        public override void Init()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = _treasureObject.Icon;
            _spriteRenderer.sortingOrder = GlobalVariables.TreasureRenderOrder;
            overallValue = _treasureObject.goldValue + _treasureObject.experienceValue * 2 + _treasureObject.gemsValue * 500;
        }
    }
}