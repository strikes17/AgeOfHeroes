using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes.MapEditor
{
    public class GUIMapEditorWidget : MonoBehaviour
    {
        public Button SelectButton => _button;

        public Sprite ImageIcon
        {
            set => _image.sprite = value;
        }
        
        [SerializeField] protected Image _image;
        [SerializeField] protected Button _button;
    }
}