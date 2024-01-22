using UnityEngine;

namespace AgeOfHeroes
{
    [IconAttribute("Assets/Editor/character_addon.png")]
    [ExecuteAlways]
    public class CharacterEditorAddon : MonoBehaviour
    {
        private Character _character;
        private SpriteRenderer _spriteRenderer;

        private void OnEnable()
        {
            _character = GetComponent<Character>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            _character._quantityBackgroundSprite.color = GlobalVariables.playerColors[(PlayerColor) _character.playerOwnerColor];
            _character._tmpTextQuantity.text = _character.Count.ToString();
            if (_spriteRenderer.sprite != _character.CharacterObject.mainSprite)
                _spriteRenderer.sprite = _character.CharacterObject.mainSprite;
        }
    }
}