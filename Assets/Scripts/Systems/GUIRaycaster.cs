using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUIRaycaster : MonoBehaviour
    {
        [SerializeField] private GraphicRaycaster _graphicRaycaster;
        [SerializeField] private EventSystem _eventSystem;
        private PointerEventData _pointerEventData;

        public bool HasAnyUI()
        {
            List<RaycastResult> raycastResult = new List<RaycastResult>();
            _pointerEventData = new PointerEventData(_eventSystem);
            _pointerEventData.position = Input.mousePosition;
            _graphicRaycaster.Raycast(_pointerEventData, raycastResult);
            if (raycastResult.Count > 0)
                return true;
            return false;
        }

        public T GetUIElement<T>()
        {
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            _pointerEventData = new PointerEventData(_eventSystem);
            _pointerEventData.position = Input.mousePosition;
            _graphicRaycaster.Raycast(_pointerEventData, raycastResults);
            var filtered = raycastResults.Where(x => x.gameObject.transform.parent.GetComponent<T>() != null)
                .Select(x => x.gameObject.transform.parent).Distinct().ToList();
            if (filtered.Count <= 0) return default;
            
            var result = filtered.FirstOrDefault().gameObject.GetComponent<T>();
            return result;
        }
    }
}