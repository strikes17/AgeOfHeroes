using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHeroes
{
    public class CharacterAmbienceAudioSource
    {
        private AudioSource _audioSource;
        private List<AudioClip> _audioClips;
        private float _updateTime;
        private float _averageACTime;
        private Vector2 _timeIntervalValues;
        private float _timeInterval;

        public CharacterAmbienceAudioSource(AudioSource audioSource, List<AudioClip> audioClips)
        {
            _audioSource = audioSource;
            _audioClips = audioClips;
            _audioSource.volume = GlobalVariables.globalAmbienceVolume;
            _timeIntervalValues = new Vector2(35f, 60f);
            int acCount = _audioClips.Count;
            foreach (var ac in _audioClips)
            {
                _averageACTime += ac.length;
            }

            _averageACTime /= acCount;
            _timeInterval = Random.Range(_timeIntervalValues.x, _timeIntervalValues.y) + _averageACTime;
        }

        public void Update()
        {
            _updateTime += Time.deltaTime;
            if (_updateTime < _timeInterval)
                return;
            _updateTime = 0f;
            float rand = Random.Range(0f, 1f);
            if (rand <= 0.6f)
                return;
            _timeInterval = Random.Range(_timeIntervalValues.x, _timeIntervalValues.y) + _averageACTime;
            var audioClip = _audioClips[Random.Range(0, _audioClips.Count)];
            _audioSource.PlayOneShot(audioClip);
        }
    }
}