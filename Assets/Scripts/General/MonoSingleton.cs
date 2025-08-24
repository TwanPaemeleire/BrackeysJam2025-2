using UnityEngine;
namespace Assets.Scripts.General
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        static T _instance;
        static bool hasBeenCreated;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<T>();
                    if (_instance == null)
                    {
                        if (!hasBeenCreated)
                            _instance = new GameObject("_" + typeof(T), typeof(T)).GetComponent<T>();
                    }
                    else
                    {
                        hasBeenCreated = true;
                        DontDestroyOnLoad(_instance);
                        _instance.Init();
                    }
                    return _instance;
                }
                return _instance;
            }
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);
            if (Instance == null)
            {
                _instance = this as T;
                hasBeenCreated = true;
                Instance.Init();
            }
        }
        protected virtual void Init() { }
    }
}