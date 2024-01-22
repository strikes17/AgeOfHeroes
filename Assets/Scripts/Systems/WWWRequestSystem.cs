using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace AgeOfHeroes
{
    public class WWWRequestSystem : MonoBehaviour
    {
        private static WWWRequestSystem _request;

        public delegate void OnRequestDelegate();

        public event OnRequestDelegate RequestCompleted
        {
            add => requestCompleted += value;
            remove => requestCompleted -= value;
        }

        private event OnRequestDelegate requestCompleted;

        public static WWWRequestSystem Request => _request;

        private void Awake()
        {
            _request = GetComponent<WWWRequestSystem>();
            DontDestroyOnLoad(gameObject);
        }

        public IEnumerator Texture2D(string path, Action<Texture2D> texture2D)
        {
            using (var loader = new WWW(path))
            {
                yield return loader;
                texture2D.Invoke(loader.texture);
            }
        }

        public IEnumerator TextFile(string path, Action<string> fileContents)
        {
            var loader = new WWW(path);
            while (!loader.isDone)
            {
                yield return null;
            }

            fileContents.Invoke(loader.text);
        }
    }
}