using UnityEngine;
using System.Collections;

namespace AgeOfHeroes
{
    [RequireComponent(typeof(BaseGridSnapResolver))]
    public abstract class AbstractCollectable : MonoBehaviour
    {
        public delegate void OnCollectableEventDelegate(Hero heroCollector);

        public event OnCollectableEventDelegate Collected
        {
            add => collected += value;
            remove => collected -= value;
        }

        private event OnCollectableEventDelegate collected;
        protected SpriteRenderer _spriteRenderer;
        public int overallValue;
        public int UniqueId;
        public abstract void ShowDialogue(Hero heroCollector);
        public abstract void Init();
        public Vector2Int Position
        {
            get => new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
            set => transform.position = new Vector3(value.x, value.y, 0);
        }
        public virtual void OnCollected(Hero heroCollector)
        {
            collected?.Invoke(heroCollector);
        }

        public virtual void OnAICollected(Hero hero)
        {
            collected?.Invoke(hero);
        }

        public virtual IEnumerator IEDestroy()
        {
            // yield return new WaitForSeconds(0.5f);
            // Debug.Log($"TREASURE DESTROY {GetInstanceID()}");
            Destroy(gameObject);
            yield break;
        }
    }
}