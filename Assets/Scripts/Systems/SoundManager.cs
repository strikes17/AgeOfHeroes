using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AgeOfHeroes
{
    public class SoundManager : AbstractManager
    {
        private List<AudioSource> poolOf3DAudioSources = new List<AudioSource>();
        private List<AudioClip> loadedAudioClips = new List<AudioClip>();
        private AudioSource _audioSourcePrefab;

        private void Awake()
        {
            _audioSourcePrefab = ResourcesBase.LoadPrefab("AudioSource").GetComponent<AudioSource>();
        }

        private void Start()
        {
            OnLoaded();
        }
        
        public void CacheAudioClip(AudioClip audioClip)
        {
            if(loadedAudioClips.Contains(audioClip))
                return;
            loadedAudioClips.Add(audioClip);
        }


        public void PreloadAudioClip(string audioClipPath, Action<string> onPreloaded = null)
        {
            ResourcesBase.LoadSoundAsync(audioClipPath, audioClip =>
            {
                AudioClip clip = audioClip as AudioClip;
                onPreloaded?.Invoke(clip.name);
                loadedAudioClips.Add(clip);
            });
        }

        private AudioClip TryGetAudioClipFromCache(string audioClipName)
        {
            return loadedAudioClips.Where(x => x.name == audioClipName).FirstOrDefault();
        }
        
        public void Play3DAudioClip(AudioClip audioClip, Vector3 position, bool loop)
        {
            int poolSize = poolOf3DAudioSources.Count;
            if (poolSize == 0)
            {
                PlayAsNew(audioClip, position, loop);
            }
            else
            {
                foreach (var audioSource in poolOf3DAudioSources)
                {
                    if (!audioSource.isPlaying)
                    {
                        audioSource.loop = loop;
                        audioSource.PlayOneShot(audioClip);
                        return;
                    }
                }
                PlayAsNew(audioClip, position, loop);
            }
        }

        private void PlayAsNew(AudioClip audioClip, Vector3 position, bool loop)
        {
            var newAudioSource = GameObject.Instantiate(_audioSourcePrefab, position, Quaternion.identity);
            newAudioSource.loop = loop;
            newAudioSource.PlayOneShot(audioClip);
            poolOf3DAudioSources.Add(newAudioSource);
        }

        public void Play3DAudioClip(string audioClipName, Vector3 position, bool loop)
        {
            var audioClip = TryGetAudioClipFromCache(audioClipName);
            if (audioClip == null)
            {
                Debug.Log($"{audioClipName} failed to load!");
                return;
            }

            Play3DAudioClip(audioClip, position, loop);
        }
    }
}