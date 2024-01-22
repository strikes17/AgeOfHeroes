using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUIPanWindowButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        private bool isLocked = true;
        private Vector3 inputPosition;
        private Vector3 prevMousePosition, currentMousePosition;
        private Vector3 mouseVector;
        [SerializeField] private RectTransform _rectTransform;

        public void OnPointerDown(PointerEventData eventData)
        {
            isLocked = false;
            inputPosition = Input.mousePosition - _rectTransform.transform.position;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isLocked = true;
        }

        private void Awake()
        {
            _rectTransform = _rectTransform == null ? transform.parent.GetComponent<RectTransform>() : _rectTransform;
        }

        private void Update()
        {
            if (isLocked)
                return;
            prevMousePosition = currentMousePosition;
            currentMousePosition = Input.mousePosition;
            mouseVector = currentMousePosition - prevMousePosition;
            var position = currentMousePosition - inputPosition;

            if (position.x < 0f)
            {
                position.x = 0f;
            }
            else if (position.x + _rectTransform.rect.width > Screen.width)
            {
                position.x = Screen.width - _rectTransform.rect.width;
            }
            else
                _rectTransform.transform.position =
                    new Vector3(position.x, _rectTransform.position.y, _rectTransform.position.z);

            if (position.y - _rectTransform.rect.height < 0f)
            {
                position.y = Screen.height - _rectTransform.rect.height;
            }
            else if (position.y > Screen.height)
            {
                position.x = Screen.height - _rectTransform.rect.height;
            }
            else
                _rectTransform.transform.position =
                    new Vector3(_rectTransform.position.x, position.y, _rectTransform.position.z);
        }
    }
}