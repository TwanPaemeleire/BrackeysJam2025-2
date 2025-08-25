using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.GodFights
{
    [System.Serializable]
    public abstract class BaseGodAttack : MonoBehaviour
    {
        [Header("Base Attack Settings")]
        [SerializeField] private bool _canExecuteConsecutive = false;
        [SerializeField] private BaseGodAttack _nextGuaranteedAttack;
        [SerializeField] private float _delayAfterAttack = 1.0f;

        public bool CanExecuteConsecutive { get { return _canExecuteConsecutive; } }
        public BaseGodAttack NextGuaranteedAttack { get { return _nextGuaranteedAttack; } }
        public float DelayAfterAttack { get { return _delayAfterAttack; } }

        public abstract void StartAttack();
        public abstract void StopAttack();
        public UnityEvent OnAttackFinished = new UnityEvent();
    }
}