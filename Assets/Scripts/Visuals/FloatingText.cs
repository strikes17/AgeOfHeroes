using System;
using System.Collections;
using Redcode.Moroutines;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class FloatingText : MonoBehaviour
    {
        public static FloatingText Create(Vector2 position, float xOffset, float yOffset)
        {
            var prefab = ResourcesBase.GetPrefab("_FT").GetComponent<FloatingText>();
            var instance = GameObject.Instantiate(prefab, new Vector3(position.x + xOffset, position.y + yOffset, 0f),
                Quaternion.identity);
            instance.gameObject.SetActive(false);
            return instance;
        }

        [SerializeField] private TMP_Text _tmpText;
        [SerializeField] private SpriteRenderer _floatingIcon;
        [SerializeField] private float _displayTimeBeforeVanish, _vanishTime, _floatSpeed;
        private Color _color;
        private bool _floating;
        private float updateTime;

        private void Awake()
        {
            _tmpText.gameObject.SetActive(false);
            _floatingIcon.gameObject.SetActive(false);
        }

        public void MakeFloat(Color color, int value, Sprite icon = null)
        {
            if (value == 0)
                Stop();
            _color = color;
            _tmpText.color = _color;
            string strValue = value.ToString();
            _tmpText.text = value >= 0 ? $"+{strValue}" : $"{strValue}";
            _tmpText.gameObject.SetActive(true);
            _floating = true;
            if (icon != null)
            {
                _floatingIcon.sprite = icon;
                _floatingIcon.gameObject.SetActive(true);
            }
        }

        public void MakeFloatDelayed(Color color, int value, float delay)
        {
            Moroutine.Run(IEDelay(color, value, delay));
        }

        private IEnumerator IEDelay(Color color, int value, float delay)
        {
            yield return new WaitForSeconds(delay);
            MakeFloat(color, value);
        }

        private void Update()
        {
            if (!_floating) return;
            updateTime += Time.deltaTime;
            _tmpText.transform.Translate(Vector3.up * _floatSpeed * Time.deltaTime, Space.World);
            if (_floatingIcon.sprite != null)
                _floatingIcon.transform.position =
                    new Vector3(_tmpText.transform.position.x + 1f, _tmpText.transform.position.y, 0f);
            if (updateTime < _displayTimeBeforeVanish)
                return;
            _color.a = Mathf.MoveTowards(_color.a, 0f, _vanishTime);
            _tmpText.color = _color;
            if (_floatingIcon.sprite != null)
                _floatingIcon.color = _color;
            if (_color.a <= 0f)
            {
                Stop();
            }
        }

        private void Stop()
        {
            _floating = false;
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}