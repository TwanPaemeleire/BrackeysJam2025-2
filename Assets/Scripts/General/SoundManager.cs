using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.General
{
    public class SoundManager : MonoSingleton<SoundManager>
    {
        [System.Serializable]
        public struct SFXEntry
        {
            public string key;
            public AudioClip clip;
        }

        [SerializeField]
        private AudioSource _sfxSource;

        [SerializeField]
        private List<SFXEntry> _sfxEntries = new List<SFXEntry>();
        private Dictionary<string, AudioClip> _sfxLibrary = new Dictionary<string, AudioClip>();

        protected override void Init()
        {
            if (_sfxSource == null)
            {
                _sfxSource = gameObject.AddComponent<AudioSource>();
                _sfxSource.loop = false;
                _sfxSource.playOnAwake = false;
            }
        }

        //private void Awake()
        //{
        //    if (_sfxSource == null)
        //    {
        //        _sfxSource = gameObject.AddComponent<AudioSource>();
        //        _sfxSource.loop = false;
        //        _sfxSource.playOnAwake = false;
        //    }
        //}

        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
            if (clip == null)
            {
                return;
            }
            _sfxSource.PlayOneShot(clip, volume);
        }

        public void RegisterSFX(string key, AudioClip clip)
        {
            if (!_sfxLibrary.ContainsKey(key) && clip != null)
            {
                _sfxLibrary.Add(key, clip);
            }
        }

        public void PlaySFX(string key, float volume = 1f)
        {
            var entry = _sfxEntries.FirstOrDefault(e => e.key == key);

            if (entry.clip != null && entry.key == key)
            {
                _sfxSource.PlayOneShot(entry.clip, volume);
            }
        }
    }
}
