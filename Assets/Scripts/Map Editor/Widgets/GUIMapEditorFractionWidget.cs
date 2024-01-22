using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes.MapEditor
{
    public class GUIMapEditorFractionWidget : GUIMapEditorWidget
    {
        private FractionObject _fractionObject;

        public void Set(FractionObject fractionObject)
        {
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(SelectFraction);
            _fractionObject = fractionObject;
            _image.sprite = _fractionObject.Icon;
        }

        private void SelectFraction()
        {
            MapEditorManager.Instance.SelectedFraction = _fractionObject.Fraction;
        }
    }

}