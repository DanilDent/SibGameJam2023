using Config;
using DG.Tweening;
using Game;
using GameTime;
using Helpers;
using UnityEngine;

namespace Sound
{
    public class MusicController : MonoSingleton<MusicController>
    {
        public AudioSource Audio => _audio;

        private GameConfigSO _config;
        [SerializeField] private AudioSource _audio;
        [SerializeField] private TrackSO[] _tracks;
        [SerializeField] private float _defaultVolume = 0.5f;

        private int _currentTrackIndex = 0;

        protected override void Awake()
        {
            base.Awake();

            _config = ConfigContainer.Instance.Value;
            _tracks = _config.Tracks;
        }

        public void Play(float duration = 0f, bool loop = false)
        {
            _audio.volume = 0;
            _audio.loop = loop;
            DOTween.To(() => _audio.volume, x => _audio.volume = x, _defaultVolume, duration: duration);
            _audio.Play();
        }

        public void PlayScheduled(double time, float duration = 0f, bool loop = false)
        {
            _audio.volume = 0;
            _audio.Stop();
            _audio.loop = loop;
            _audio.PlayScheduled(time);
            DOTween.To(() => _audio.volume, x => _audio.volume = x, _defaultVolume, duration: duration);
        }

        public void SetClip(AudioClip clip)
        {
            _audio.clip = clip;
        }

        public TrackSO GetCurrentTrack()
        {
            return GetTrack(_currentTrackIndex);
        }

        public TrackSO GetTrack(int index)
        {
            if (index < 0 || index >= _tracks.Length)
            {
                Debug.LogWarning($"Music controller:Invalid track index");
                return null;
            }

            return _tracks[index];
        }

        public TrackSO SetTrack(int index, bool loop = false)
        {
            if (index < 0 || index >= _tracks.Length)
            {
                Debug.LogWarning($"Music controller:Invalid track index");
                return null;
            }
            _currentTrackIndex = index;
            _audio.clip = _tracks[index].Clip;
            return _tracks[index];
        }

        public void Stop(float duration = 0f)
        {
            _audio.Stop();
            DOTween.To(() => _audio.volume, x => _audio.volume = x, 0f, duration)
                .OnComplete(() => _audio.Stop());
        }
    }
}