using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace AgeOfHeroes.MapEditor
{
    public class GUIMapEditorStructuresView : MonoBehaviour
    {
        [SerializeField] private Transform _contentTransform;
        [SerializeField] private GUIMapEditorStructureWidget _castleWidgetPrefab, _dwellWidgetPrefab;
        [SerializeField] private List<CastleObject> _castleObjects;
        private List<GUIMapEditorStructureWidget> _widgets = new List<GUIMapEditorStructureWidget>();
        private List<AbstractBuilding> dwellBuildings = new List<AbstractBuilding>();

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Clear()
        {
            for (int i = 0; i < _widgets.Count; i++)
            {
                Destroy(_widgets[i].gameObject);
            }

            _widgets.Clear();
        }

        public void Set(StructureType structureType)
        {
            Clear();
            switch (structureType)
            {
                case StructureType.Castle:
                    foreach (var castle in _castleObjects)
                    {
                        var castleWidgetInstance = GameObject.Instantiate(_castleWidgetPrefab, Vector3.zero,
                            Quaternion.identity, _contentTransform) as GUIMapEditorCastleWidget;
                        castleWidgetInstance.Set(castle);
                        _widgets.Add(castleWidgetInstance);
                    }

                    break;
                case StructureType.Dwell:
                    dwellBuildings = MapEditorDatabase.Instance.GetAllBuildings().Where(x => x is DwellBuilding).ToList();
                    Debug.Log(dwellBuildings.Count);
                    foreach (var abstractBuilding in dwellBuildings)
                    {
                        var dwellWidgetInstance =
                            GameObject.Instantiate(_dwellWidgetPrefab, Vector3.zero, Quaternion.identity,
                                _contentTransform) as GUIMapEditorDwellWidget;
                        dwellWidgetInstance.Set(abstractBuilding as DwellBuilding);
                        _widgets.Add(dwellWidgetInstance);
                    }
                    break;
            }
        }
    }
}