using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUISpecialBuildingInfoWindow : GUIDialogueWindow
    {
        [SerializeField] protected Button _closeButton;
        [SerializeField] protected TMP_Text _descriptionText, _titleText, _goldText, _gemsText;
        [SerializeField] protected Image _buildingIcon;
        protected SpecialBuilding _specialBuilding;
        protected Castle _targetCastle;

        protected virtual void Awake()
        {
            _closeButton.onClick.AddListener(Hide);
        }

        public virtual void SetGUI(Castle castle, SpecialBuilding specialBuilding)
        {
            _targetCastle = castle;
            _specialBuilding = specialBuilding;
            _titleText.text = _specialBuilding.title;
            _descriptionText.text = _specialBuilding.description;
            _buildingIcon.sprite = _specialBuilding.GetIcon();
            _goldText.text = _specialBuilding.goldCost.ToString();
            _gemsText.text = _specialBuilding.gemsCost.ToString();
        }
    }
}