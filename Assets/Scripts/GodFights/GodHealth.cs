using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using Assets.Scripts.SharedLogic;

namespace Assets.Scripts.GodFights
{
    public class GodHealth : MonoBehaviour
    {
        [SerializeField] private float _maxHealth = 50.0f;
        [SerializeField] private List<float> _phaseTriggerPercentages = new List<float>();
        [SerializeField] private GradualHealthChanger _healthBarUI;

        private float _currentHealth;
        private int _currentPhase = 0;
        private float _healthToTriggerNextPhase;
        private bool _isInFinalPhase = false;
        private bool _isDead = false;

        public UnityEvent OnDamageTaken = new UnityEvent();
        public UnityEvent OnPhaseChange = new UnityEvent();
        public UnityEvent OnDeath = new UnityEvent();

        public void Initialize()
        {
            _currentHealth = _maxHealth;
            _healthBarUI.Initialize(_maxHealth);

            if(_phaseTriggerPercentages.Count > 0)
            {
                _healthToTriggerNextPhase = _maxHealth * _phaseTriggerPercentages[_currentPhase];
            }
            else
            {
                _isInFinalPhase = true;
            }
        }

        public void TakeDamage(float damage)
        {
            if(_isDead) return;
            _currentHealth -= damage;
            if (_currentHealth <= 0) // God has died
            {
                _healthBarUI.SnapToAmount(_currentHealth);
                _currentHealth = 0;
                _isDead = true;
                OnDeath.Invoke();
            }
            else if (!_isInFinalPhase && _currentHealth <= _healthToTriggerNextPhase) // If the god has more than one phase and has taken enough damage
            {
                ++_currentPhase;
                if (_currentPhase != _phaseTriggerPercentages.Count)
                {
                    _healthToTriggerNextPhase = _maxHealth * _phaseTriggerPercentages[_currentPhase];
                }
                else
                {
                    _isInFinalPhase = true;
                }
                OnPhaseChange.Invoke();
            }
            if(!_isDead) _healthBarUI.SetTargetHealth(_currentHealth);
        }

        public void Heal(float amount)
        {
            if(_isDead) return;
            _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
            _healthBarUI.SetTargetHealth(_currentHealth);
        }
    }
}