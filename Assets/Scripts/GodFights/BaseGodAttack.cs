using System;
using System.Collections.Generic;
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

        private Dictionary<string, Action> _animationActions = new Dictionary<string, Action>();

        public bool CanExecuteConsecutive { get { return _canExecuteConsecutive; } }
        public BaseGodAttack NextGuaranteedAttack { get { return _nextGuaranteedAttack; } }
        public float DelayAfterAttack { get { return _delayAfterAttack; } }

        protected void RegisterAction(string name, Action action)
        {
            if(string.IsNullOrEmpty(name) || action == null) return;
            _animationActions[name] = action;
        }

        public bool TryExecuteAction(string name)
        {
            if(_animationActions == null) return false;
            if(_animationActions.TryGetValue(name, out var action))
            {
                action.Invoke();
                return true;
            }
            return false;
        }

        public abstract void InitializeAttack();
        public abstract void StartAttack();
        public abstract void StopAttack();
        public UnityEvent OnAttackFinished = new UnityEvent();
    }
}