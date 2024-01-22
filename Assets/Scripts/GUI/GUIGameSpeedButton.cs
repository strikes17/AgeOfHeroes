using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUIGameSpeedButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text _speedValueText;
        private Button _button;
        public int mode = 0;

        private void Awake()
        {
            _button = GetComponent<Button>();
            mode = 0;
            SetTimeScale();
            _button.onClick.AddListener(() =>
            {
                SetTimeScale();
            });
        }

        void SetTimeScale()
        {
            mode = mode > 3 ? 1 : mode + 1;
            Time.timeScale = mode;
            _speedValueText.text = $"X{mode}";
        }
    }
}