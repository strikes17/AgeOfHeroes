using TMPro;
using UnityEngine;

namespace AgeOfHeroes
{
    public class GUICharacterStatWidget : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        public string Text
        {
            get { return _text.text; }
            set { _text.text = value; }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}