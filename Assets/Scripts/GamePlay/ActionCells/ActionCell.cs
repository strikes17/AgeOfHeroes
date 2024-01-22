using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class ActionCell : MonoBehaviour, IClickTarget
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        private AbstractActionCell _variant;

        public Vector2Int Position
        {
            get => new Vector2Int(Mathf.RoundToInt(transform.position.x),Mathf.RoundToInt(transform.position.y));
        }

        public bool Active
        {
            set { gameObject.SetActive(value); }
            get => gameObject.activeSelf;
        }
        

        public SpriteRenderer spriteRenderer => _spriteRenderer;

        public AbstractActionCell Variant
        {
            get => _variant;
            set
            {
                _variant = value;
                _spriteRenderer.color = _variant.Color;
                _spriteRenderer.sprite = _variant.Sprite;
            }
        }

        public IEnumerator OnClicked(Player player)
        {
            yield return new WaitForEndOfFrame();
            _variant.OnActionCellActivated(player, this);
        }
    }
}