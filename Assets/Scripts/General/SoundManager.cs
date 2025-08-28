using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        private AudioSource _sfxSource;

        [SerializeField]
        private AudioClip _ambientClip;
        private AudioSource _ambientAudioSource;

        private AudioSource[] _musicSources = new AudioSource[2];
        private int _activeMusicSource = 0;
        private readonly float _musicFadeDuration = 1f;

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

            if (_ambientAudioSource == null)
            {
                _ambientAudioSource = gameObject.AddComponent<AudioSource>();
                _ambientAudioSource.loop = true;
                _ambientAudioSource.playOnAwake = false;
                _ambientAudioSource.volume = 0.30f;
            }

            for (var i = 0; i < _musicSources.Length; i++)
            {
                _musicSources[i] = gameObject.AddComponent<AudioSource>();
                _musicSources[i].loop = true;
                _musicSources[i].playOnAwake = false;
                _musicSources[i].volume = 0.15f;
            }
        }

        private void OnEnable()// TEMP
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()// TEMP
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) // TEMP
        {
            var sceneName = scene.name;

            if (sceneName == "GameplayScene")
            {
                return;
            }

            var clip = Resources.Load<AudioClip>($"Music/{sceneName + "Theme"}");

            if (clip != null)
            {
                Instance.PlayMusic(clip);
            }
            else
            {
                Debug.LogWarning($"There is no music for scene '{sceneName + "Theme"}' in Resources/Music/");
                Instance.StopMusic();
            }
        }

        public void PlayMusic(AudioClip clip)
        {
            if (clip == null)
            {
                return;
            }

            AudioSource current = _musicSources[_activeMusicSource];
            if (current.clip == clip && current.isPlaying)
            {
                return;
            }

            StartCoroutine(CrossFadeMusic(clip));
        }

        public void PlayMusic(string musicName)
        {
            if (musicName == null)
            {
                return;
            }

            AudioSource current = _musicSources[_activeMusicSource];
            if (current.clip.name == musicName && current.isPlaying)
            {
                return;
            }

            var clip = Resources.Load<AudioClip>($"Music/{musicName}");

            if (clip != null)
            {
                StartCoroutine(CrossFadeMusic(clip));
            }
            else
            {
                Debug.LogWarning($"There is no music titled '{musicName}' in Resources/Music/");
            }
        }

        private IEnumerator CrossFadeMusic(AudioClip newClip)
        {
            var nextIndex = 1 - _activeMusicSource;
            var from = _musicSources[_activeMusicSource];
            var to = _musicSources[nextIndex];

            to.clip = newClip;
            to.volume = 0f;
            to.Play();

            var timer = 0f;
            while (timer < _musicFadeDuration)
            {
                timer += Time.deltaTime;
                var t = Mathf.Clamp01(timer / _musicFadeDuration);
                from.volume = Mathf.Lerp(0.15f, 0.0f, t);
                to.volume = Mathf.Lerp(0.0f, 0.15f, t);
                yield return null;
            }

            from.Stop();
            to.volume = 0.15f;
            _activeMusicSource = nextIndex;
        }

        public void StopMusic()
        {
            StartCoroutine(FadeOutMusic());
        }

        private IEnumerator FadeOutMusic()
        {
            var current = _musicSources[_activeMusicSource];
            var startVol = current.volume;
            var timer = 0f;
            while (timer < _musicFadeDuration)
            {
                timer += Time.deltaTime;
                current.volume = Mathf.Lerp(startVol, 0f, timer / _musicFadeDuration);
                yield return null;
            }
            current.Stop();
        }

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
