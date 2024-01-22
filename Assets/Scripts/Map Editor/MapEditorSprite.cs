using System.Collections;
using UnityEngine;

namespace AgeOfHeroes
{
    public class MapEditorSprite : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Color _color;

        public void CreateWithLifeTime(float time)
        {
            // _spriteRenderer.color = _color;
            StartCoroutine(IELifeTime(time));
        }

        private IEnumerator IELifeTime(float time)
        {
            yield return new WaitForSeconds(time);
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}