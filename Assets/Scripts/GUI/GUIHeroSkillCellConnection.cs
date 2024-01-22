using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes
{
    public class GUIHeroSkillCellConnection : GUIBaseHeroSkillCell
    {
        [SerializeField] private Image _image;

        public void SetConnectionVariant(int directionX, int directionY)
        {
            int v = 0;
            switch (directionY)
            {
                case 0:
                    v = directionX == 1 ? 4 : 2;
                    break;
                case 1:
                    v = 0;
                    break;
                case 2:
                    v = directionX == 1 ? 3 : 1;
                    break;
            }

            var sprite = ResourcesBase.GetSprite($"GUI/stc+{v}");
            _image.sprite = sprite;
        }
    }
}