using Helpers;
using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    public class SFXController : MonoSingleton<SFXController>
    {
        [SerializeField] private float _defaultVolume;
        [SerializeField] private AudioClip[] _sfx;

        private Dictionary<string, AudioClip> _clips;

        protected override void Awake()
        {
            base.Awake();

            _clips = new Dictionary<string, AudioClip>();

            for (int i = 0; i < _sfx.Length; ++i)
            {
                _clips.Add(_sfx[i].name, _sfx[i]);
                Debug.Log($"sfx name: {_sfx[i].name}");
            }
        }

        public void PlaySFX(string sfxName, AudioSource audio)
        {
            AudioClip clip = _clips[sfxName];
            audio.volume = _defaultVolume;
            audio.Stop();
            audio.PlayOneShot(clip);
        }
    }
}
