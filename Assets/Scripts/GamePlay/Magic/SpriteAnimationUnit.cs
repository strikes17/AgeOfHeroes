using System;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes.Spell
{
    public class SpriteAnimationUnit : MonoBehaviour
    {
        private SpriteAnimationSequenceObject _spriteAnimationSequence;
        private SpriteRenderer _spriteRenderer;
        private float _updateTime, _fps;
        private int _indexer;
        private int _spritesCount;
        private int _cycles, _cyclesIndexer;
        private bool _expired = true;
        private Color _color;
        private Vector2 _offset;

        public delegate void OnSpriteAnimationEventDelegate();

        public event OnSpriteAnimationEventDelegate Expired
        {
            add => expired += value;
            remove => expired -= value;
        }

        private event OnSpriteAnimationEventDelegate expired;

        public bool Looped => _cycles == -1;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void FlipOffset(bool x)
        {
            Vector3 localPosition = transform.localPosition;
            if (x)
                localPosition.x = -_offset.x;
            else
                localPosition.x = _offset.x;
            transform.localPosition = localPosition;
        }

        public void FlipSprite(bool x, bool y)
        {
            _spriteRenderer.flipX = x && !_spriteRenderer.flipX;
            _spriteRenderer.flipY = y && !_spriteRenderer.flipY;
        }

        public void Play(SpriteAnimationSequenceObject spriteAnimationSequenceObject, int cycles = -1,
            float xOffset = 0f, float yOffset = 0f)
        {
            _spriteAnimationSequence = spriteAnimationSequenceObject;
            _spritesCount = _spriteAnimationSequence.Sprites.Count;
            _cycles = cycles;
            _expired = false;
            _updateTime = 1000f;
            _color = spriteAnimationSequenceObject.Color;
            _fps = spriteAnimationSequenceObject.fps;
            _spriteRenderer.color = _color;
            _offset = new Vector2(xOffset, yOffset);
            _spriteRenderer.transform.Translate(_offset);
        }

        public void Stop()
        {
            _expired = true;
            gameObject.SetActive(false);
            expired?.Invoke();
            Destroy(gameObject);
        }

        public void Update()
        {
            if (_expired)
                return;
            _updateTime += Time.deltaTime;
            if (_updateTime < _fps)
                return;
            _updateTime = 0f;
            _spriteRenderer.sprite = _spriteAnimationSequence.Sprites[_indexer];
            _indexer++;
            if (_indexer >= _spritesCount)
            {
                _cyclesIndexer++;
                if (_cycles != -1 && _cyclesIndexer >= _cycles)
                    Stop();
            }

            _indexer = _indexer >= _spritesCount ? 0 : _indexer;
        }
    }
}