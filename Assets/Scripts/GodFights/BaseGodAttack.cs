using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.GodFights
{
    [System.Serializable]
    public abstract class BaseGodAttack : MonoBehaviour
    {
        private BaseGodFight _god;
        public BaseGodFight God { get { return _god; } set { _god = value; } }

        private Dictionary<string, Action> _animationActions = new Dictionary<string, Action>();

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