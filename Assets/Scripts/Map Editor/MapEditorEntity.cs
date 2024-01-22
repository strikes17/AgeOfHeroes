using System;
using UnityEngine;

namespace AgeOfHeroes.MapEditor
{
    public class MapEditorEntity : MonoBehaviour
    {
        protected SpriteRenderer _spriteRenderer;
        public Vector2Int Position
        {
            get => new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
            set => transform.position = new Vector3(value.x, value.y, 0);
        }

        protected virtual void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public SpriteRenderer SpriteRenderer => _spriteRenderer;

        public virtual void OnClicked()
        {
            
        }
    }
}