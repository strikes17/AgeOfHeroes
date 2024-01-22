using UnityEngine;

namespace AgeOfHeroes.MapEditor
{
    public class MapEditorTreasure : MapEditorEntity
    {
        private SpriteRenderer _spriteRenderer;
        private TreasureObject _treasureObject;
        public TreasureObject treasureObject => _treasureObject;

        public void Init(TreasureObject treasureObject)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _treasureObject = treasureObject;
            _spriteRenderer.sprite = _treasureObject.Icon;
        }
        
        public virtual void OnClicked()
        {
            MapEditorManager.Instance.SelectedTreasure = this;
        }
    }
}