using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUIChestTreasureDialogue : GUIDialogueWindow
    {
        [SerializeField] protected Button okButton;
        [SerializeField] protected Button goldButton;
        [SerializeField] protected Button experienceButton;
        [SerializeField] protected Button gemsButton;
        public GameObject goldGUIRoot, expGUIRoot, gemsGUIRoot;
        public TMP_Text goldValueTMPText, expValueTMPText, gemsValueTMPText, messageTMPText;

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
            goldButton.onClick.RemoveAllListeners();
            gemsButton.onClick.RemoveAllListeners();
            experienceButton.onClick.RemoveAllListeners();
        }

        public void SetOnGoldButtonEvent(OnTreasureEventDelegate selected)
        {
            goldButton.onClick.AddListener(() =>
            {
                okButton.gameObject.SetActive(true);
                selected.Invoke();
            });
        }

        public void SetOnExpButtonEvent(OnTreasureEventDelegate selected)
        {
            experienceButton.onClick.AddListener(() =>
            {
                okButton.gameObject.SetActive(true);
                selected.Invoke();
            });
        }

        public void SetOnGemsButtonEvent(OnTreasureEventDelegate selected)
        {
            gemsButton.onClick.AddListener(() =>
            {
                okButton.gameObject.SetActive(true);
                selected.Invoke();
            });
        }

        public void SetOnOkButtonEvent(OnTreasureEventDelegate selected)
        {
            okButton.onClick.AddListener(() => { selected.Invoke(); });
        }
    }
}