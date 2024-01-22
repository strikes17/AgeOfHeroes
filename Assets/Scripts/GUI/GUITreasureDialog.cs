using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUITreasureDialog : GUIDialogueWindow
    {
        [SerializeField] protected Button okButton;
        public TMP_Text messageTMPText;

        public void SetOnOkButtonEvent(OnTreasureEventDelegate selected)
        {
            okButton.onClick.AddListener(() => { selected.Invoke(); });
        }

        public override void Show()
        {
            base.Show();
            okButton.gameObject.SetActive(false);
            okButton.onClick.AddListener(Hide);
        }

        public override void Hide()
        {
            base.Hide();
            okButton.onClick.RemoveAllListeners();
        }
    }
}