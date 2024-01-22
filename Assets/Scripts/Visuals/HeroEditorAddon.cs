using UnityEngine;

namespace AgeOfHeroes
{
    [Icon("Assets/Editor/hero_addon.png")]
    [ExecuteInEditMode]
    public class HeroEditorAddon : MonoBehaviour
    {
        private Hero _hero;
        private SpriteRenderer _spriteRenderer;

        private void OnEnable()
        {
            _hero = GetComponent<Hero>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            _hero._quantityBackgroundSprite.color = GlobalVariables.playerColors[(PlayerColor) _hero.playerOwnerColor];
            _hero._tmpTextHealthLeft.text = _hero.HealthLeft.ToString();
            if (_spriteRenderer.sprite != _hero.HeroObject.mainSprite)
                _spriteRenderer.sprite = _hero.HeroObject.mainSprite;
            Vector3 unclampedPosition = transform.position;
            _hero.coloredFlag.PlayerOwnerColor = _hero.playerOwnerColor;
        }
    }
}