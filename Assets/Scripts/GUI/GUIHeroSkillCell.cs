using System;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUIHeroSkillCell : GUIBaseHeroSkillCell
    {
        public delegate void OnHeroSkillCellEventDelegate(GUIHeroSkillCell skillCell);

        public event OnHeroSkillCellEventDelegate Clicked
        {
            add => clicked += value;
            remove => clicked -= value;
        }
        private event OnHeroSkillCellEventDelegate clicked;
        
        [SerializeField] private Image _image;
        [SerializeField] private Button _button;
        [SerializeField] private GUISpriteAnimation _guiSpriteAnimation;
        public int tier;
        public HeroSkillType HeroSkillType;
        private HeroSkill _heroSkill;

        public HeroSkill Skill => _heroSkill;

        public void Init(HeroSkill heroSkill)
        {
            _heroSkill = heroSkill;
            _image.sprite = heroSkill.spriteIcon;
            _button.onClick.AddListener(OnClicked);
        }

        private void OnClicked()
        {
            clicked?.Invoke(this);
        }

        public void SetEnabled()
        {
            _guiSpriteAnimation.gameObject.SetActive(true);
            _guiSpriteAnimation.PlayLooped(10f);
            _button.interactable = true;
        }

        public void SetDisabled()
        {
            _guiSpriteAnimation.gameObject.SetActive(false);
            _guiSpriteAnimation.Stop();
            _button.interactable = false;
        }
    }
}