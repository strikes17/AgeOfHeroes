using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes
{
    [IconAttribute("Assets/Editor/flag.png")]
    [ExecuteAlways]
    [RequireComponent(typeof(BaseGridSnapResolver))]
    public class Colored : MonoBehaviour
    {
        public PlayerColor PlayerOwnerColor
        {
            set
            {
                _playerColor = value;
                _spriteRenderer.color = GlobalVariables.playerColors[_playerColor];
            }
            get { return _playerColor; }
        }

        private PlayerColor _playerColor;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = _spriteRenderer == null ? GetComponent<SpriteRenderer>() : _spriteRenderer;
            _spriteRenderer.sortingOrder = GlobalVariables.DecorativeRenderOrder;
        }
    }
}