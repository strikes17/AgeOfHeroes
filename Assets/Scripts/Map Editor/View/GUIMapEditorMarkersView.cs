using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes.MapEditor
{
    public class GUIMapEditorMarkersView : MonoBehaviour
    {
        [SerializeField] private Transform _contentTransform;
        [SerializeField] private GUIMapEditorWidget _widgetPrefab;  
        private List<GUIMapEditorWidget> _widgets = new List<GUIMapEditorWidget>();
        
        public void Show()
        {
            gameObject.SetActive(true);
            UpdateGUI(MarkerType.AI);
        }
        private void Clear()
        {
            for (int i = 0; i < _widgets.Count; i++)
            {
                Destroy(_widgets[i].gameObject);
            }

            _widgets.Clear();
        }
        
        public void UpdateGUI(MarkerType markerType)
        {
            Clear();
            // var widgetInstance =
            //     GameObject.Instantiate(_widgetPrefab, Vector3.zero, Quaternion.identity, _contentTransform);

            // _widgets.Add(widgetInstance);
        }

        public void Hide()
        {
            Clear();
            gameObject.SetActive(false);
        }
    }
}