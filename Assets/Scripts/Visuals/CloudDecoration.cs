using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AgeOfHeroes
{
    public class CloudDecoration : MonoBehaviour
    {
        [SerializeField] private List<Sprite> _spriteVariants;
        private SpriteRenderer _spriteRenderer;
        private float _speed;
        private Vector2 _direction, _boundary, _initialPosition;
        private bool _isFloating;
        private Color _cloudColor;
        private Color _fadedColor;
        private Color _currentColor;
        private bool _isFading;

        public static CloudDecoration Create(Vector2 offset)
        {
            var prefab = ResourcesBase.GetPrefab("_CLOUD").GetComponent<CloudDecoration>();
            var instance = GameObject.Instantiate(prefab, new Vector3(offset.x, offset.y, 0f),
                Quaternion.identity);
            instance._initialPosition = instance.transform.position;
            instance._spriteRenderer = instance.GetComponent<SpriteRenderer>();
            instance._cloudColor = instance._spriteRenderer.color;
            instance._fadedColor = instance._cloudColor;
            instance._fadedColor.a = 0f;
            instance.gameObject.SetActive(false);
            return instance;
        }


        public void Float(Vector2 direction, Vector2 boundary, float speed)
        {
            _spriteRenderer.sprite = _spriteVariants[Random.Range(0, _spriteVariants.Count)];
            _cloudColor.a = Random.Range(0.3f, 0.5f);
            _isFading = false;
            _spriteRenderer.color = _fadedColor;
            transform.position = _initialPosition;
            _isFloating = true;
            _direction = direction;
            _boundary = boundary;
            _speed = speed;
            gameObject.SetActive(true);
        }

        public void Stop()
        {
            _isFloating = false;
        }

        private void Update()
        {
            if (!_isFloating)
                return;
            var v = _direction * _speed * Time.deltaTime;

            transform.Translate(v, Space.World);
            if (transform.position.x > _boundary.x || transform.position.y > _boundary.y)
            {
                _isFading = true;
            }

            if (_isFading)
            {
                if (_currentColor.a > _fadedColor.a)
                {
                    _currentColor.a = Mathf.MoveTowards(_currentColor.a, _fadedColor.a, Time.deltaTime);
                    _spriteRenderer.color = _currentColor;
                }
                else
                {
                    Float(_direction, _boundary, _speed);
                }
            }
            else
            {
                if (_currentColor.a < _cloudColor.a)
                {
                    _currentColor.a = Mathf.MoveTowards(_currentColor.a, _cloudColor.a, Time.deltaTime);
                    _spriteRenderer.color = _currentColor;
                }
            }
        }
    }
}