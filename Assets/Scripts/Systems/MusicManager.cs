using System;
using System.Collections;
using System.Collections.Generic;
using Redcode.Moroutines;
using UnityEngine;

namespace AgeOfHeroes
{
    public enum FractionMusicType
    {
        Main,
        Turn,
        Combat,
        Siege,
        Castle
    }

    public enum AudioClipType
    {
        Sound,
        Music
    }

    public enum MusicImportance
    {
        Default, NotInterruptable
    }

    public class MusicManager : AbstractManager
    {
        [SerializeField] private AudioSource _firstAudioSource, _secondAudioSource, _ambienceAudioSource;
        [SerializeField] private float _fadeSpeedMultiplier;
        private float _firstVolume, _secondVolume, _ambienceVolume;
        private Dictionary<AudioClip, float> _musicData = new Dictionary<AudioClip, float>();
        private AudioClip _latestAmbienceClip;
        private MusicImportance _latestMusicImportance;

        private void Start()
        {
            OnLoaded();
            _firstVolume = _firstAudioSource.volume;
            _secondVolume = _secondAudioSource.volume;
            _ambienceVolume = _ambienceAudioSource.volume;
            _firstAudioSource.loop = true;
            _secondAudioSource.loop = true;
            _ambienceAudioSource.loop = true;
        }

        public void ResetImportance()
        {
            _latestMusicImportance = MusicImportance.Default;
        }

        public void PlayFractionMusic(Fraction fraction, FractionMusicType fractionMusicType, MusicImportance musicImportance = MusicImportance.Default)
        {
            AudioClip audioClip = null;
            if(_latestMusicImportance == MusicImportance.NotInterruptable && musicImportance != MusicImportance.NotInterruptable)
                return;
            _latestMusicImportance = musicImportance;
            switch (fractionMusicType)
            {
                case FractionMusicType.Main:
                    switch (fraction)
                    {
                        case Fraction.Human:
                            audioClip = ResourcesBase.GetAudioClip("Human", AudioClipType.Music);
                            break;
                        case Fraction.Undead:
                            audioClip = ResourcesBase.GetAudioClip("Necromant", AudioClipType.Music);
                            break;
                    }

                    break;
                case FractionMusicType.Turn:
                    switch (fraction)
                    {
                        default:
                            audioClip = ResourcesBase.GetAudioClip("Turn", AudioClipType.Music);
                            break;
                    }

                    break;
                case FractionMusicType.Siege:
                    switch (fraction)
                    {
                        default:
                            audioClip = ResourcesBase.GetAudioClip("Siege", AudioClipType.Music);
                            break;
                    }

                    break;
                case FractionMusicType.Castle:
                    switch (fraction)
                    {
                        default:
                            audioClip = ResourcesBase.GetAudioClip("HumanCastle", AudioClipType.Music);
                            break;
                    }

                    break;
            }

            PlayMusic(audioClip);
        }

        private Moroutine amb, amb1, amb2;

        public void PlayTerrainAmbience(TerrainTileMaterialName terrainTileMaterialName)
        {
            string ambienceName = string.Empty;
            switch (terrainTileMaterialName)
            {
                case TerrainTileMaterialName.Dirt:
                case TerrainTileMaterialName.Grass:
                    ambienceName = "WindGrass";
                    break;
                case TerrainTileMaterialName.Snow:
                    ambienceName = "Snow";
                    break;
                case TerrainTileMaterialName.WaterDeep:
                case TerrainTileMaterialName.WaterShallow:
                    ambienceName = "Ocean";
                    break;
            }

            if (string.IsNullOrEmpty(ambienceName)) return;
            var clip = ResourcesBase.GetAudioClip(ambienceName, AudioClipType.Music);
            if (clip == _latestAmbienceClip) return;
            _latestAmbienceClip = clip;
            amb?.Stop();
            amb1?.Stop();
            amb2?.Stop();
            amb = Moroutine.Run(PlayAmbience(clip));
        }

        public IEnumerator PlayAmbience(AudioClip audioClip)
        {
            float time = 0f;
            if (!_musicData.TryGetValue(audioClip, out time))
            {
                _musicData.TryAdd(audioClip, time);
            }

            if (_ambienceAudioSource.isPlaying)
            {
                amb1 = Moroutine.Run(FadeInAudioSource(_ambienceAudioSource));
                yield return amb1.WaitForComplete();
            }

            _ambienceAudioSource.clip = audioClip;
            _ambienceAudioSource.Play();

            amb2 = Moroutine.Run(FadeOutAudioSource(_ambienceAudioSource, _ambienceVolume));
            yield return null;
        }

        public void PlayMusic(AudioClip audioClip)
        {
            float time = 0f;
            if (audioClip == null) return;
            if (!_musicData.TryGetValue(audioClip, out time))
            {
                _musicData.TryAdd(audioClip, time);
            }

            if (_firstAudioSource.clip == audioClip || _secondAudioSource.clip == audioClip)
                return;
            bool isFirstPlaying = _firstAudioSource.isPlaying;
            bool isSecondPlaying = _secondAudioSource.isPlaying;
            if (isFirstPlaying)
            {
                _secondAudioSource.clip = audioClip;
                _secondAudioSource.time = time;
                Moroutine.Run(FadeOutAudioSource(_secondAudioSource, _secondVolume));
                Moroutine.Run(FadeInAudioSource(_firstAudioSource));
            }
            else if (isSecondPlaying)
            {
                _firstAudioSource.clip = audioClip;
                _firstAudioSource.time = time;
                Moroutine.Run(FadeOutAudioSource(_firstAudioSource, _firstVolume));
                Moroutine.Run(FadeInAudioSource(_secondAudioSource));
            }
            else
            {
                _firstAudioSource.clip = audioClip;
                _firstAudioSource.time = time;
                Moroutine.Run(FadeOutAudioSource(_firstAudioSource, _firstVolume));
            }
        }

        private IEnumerator FadeInAudioSource(AudioSource audioSource)
        {
            yield return new WaitForEndOfFrame();
            float volume = audioSource.volume;
            while (volume > 0)
            {
                volume -= Time.deltaTime * _fadeSpeedMultiplier;
                audioSource.volume = volume;
                yield return null;
            }

            if (audioSource.clip != null)
                _musicData[audioSource.clip] = audioSource.time;
            audioSource.clip = null;
            audioSource.Stop();
        }

        private IEnumerator FadeOutAudioSource(AudioSource audioSource, float targetVolume)
        {
            audioSource.Play();
            float volume = audioSource.volume;
            while (volume < targetVolume)
            {
                volume += Time.deltaTime;
                audioSource.volume = volume;
                yield return null;
            }
        }

        private void OnDestroy()
        {
            amb?.Stop();
            amb1?.Stop();
            amb2?.Stop();
        }
    }
}