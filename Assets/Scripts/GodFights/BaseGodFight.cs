using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.GodFights
{
    public abstract class BaseGodFight : MonoBehaviour
    {
        [Header("Base god fight")]
        [SerializeField] private GodType _godType;
        [SerializeField] private GodHealth _health;
        [SerializeField] private Animator _animator;

        private float _currentDirectionMultiplier = 1.0f;

        public float CurrentDirectionMultiplier { set { _currentDirectionMultiplier = value; } get { return _currentDirectionMultiplier; } }
        public GodType GodType { get { return _godType; } }
        public Animator Animator { get { return _animator; } }
        public GodHealth Health { get { return _health; } }

        public UnityEvent OnDeath = new UnityEvent();

        public virtual void StartBossFight()
        {
            _health.Initialize();
            _health.OnDeath.AddListener(OnDeathInternal);
            _health.HealthBarUI.transform.parent.gameObject.SetActive(true);
        }
        public abstract void RestartBossFight();
        protected abstract void OnDeathInternal();
    }
}