using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUIBuildFailedDialogue : GUIDialogueWindow
    {
        [SerializeField] private TMP_Text _message;
        [SerializeField] private Button _closeButton;
        [SerializeField] private GUIBuildRedirectRequirementWidget _widgetPrefab;
        [SerializeField] private Transform _contentTransform;
        private List<GUIBuildRedirectRequirementWidget> _widgets = new List<GUIBuildRedirectRequirementWidget>();

        private void Awake()
        {
            _closeButton.onClick.AddListener(Hide);
        }

        public void NotEnoughResources(int goldValue, int gemsValue)
        {
            
        }

        private void Clear()
        {
            for (int i = 0; i < _widgets.Count; i++)
            {
                Destroy(_widgets[i].gameObject);
            }
            _widgets.Clear();
        }

        public void RequiredBuildingsAreNotBuilt(AbstractBuilding building, List<AbstractBuilding> buildings)
        {
            Clear();
            foreach (var b in buildings)
            {
                var widgetInstance = GameObject.Instantiate(_widgetPrefab, Vector3.zero, Quaternion.identity, _contentTransform);
                if (b.GetType() == typeof(CharacterShopBuilding))
                {
                    var shopBuilding = (CharacterShopBuilding) b;
                    widgetInstance.Icon = shopBuilding.basicCharacterForm.mainSprite;
                    // widgetInstance.button.onClick.AddListener(()=>RedirectToBuildMenu());
                    // widgetInstance.FrameColor = 
                }
                _widgets.Add(widgetInstance);
            }
        }
        
    }
}