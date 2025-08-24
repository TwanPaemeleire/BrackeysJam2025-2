using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.SharedLogic
{
    public class GradualHealthChanger : MonoBehaviour
    {
        [SerializeField] private Slider _healthBar;
        [SerializeField] private float _timeToReachTargetHealth = 1f;
        private float _currentTargetHealth;
        private float _maxHealth;

        public void Initialize(float maxHealth)
        {
            _maxHealth = maxHealth;
            _currentTargetHealth = maxHealth;
            _healthBar.value = 1.0f;
        }

        public void SetTargetHealth(float targetHealth)
        {
            StopAllCoroutines();
            _currentTargetHealth = Mathf.Clamp(targetHealth, 0, _maxHealth);
            StartCoroutine(GraduallyUpdateHealth());
        }

        private IEnumerator GraduallyUpdateHealth()
        {
            float initialHealth = _healthBar.value;
            float elapsedTime = 0f;
            while (elapsedTime < _timeToReachTargetHealth)
            {
                elapsedTime += Time.deltaTime;
                _healthBar.value = Mathf.SmoothStep(initialHealth, _currentTargetHealth, elapsedTime / _timeToReachTargetHealth);
                yield return null;
            }
            _healthBar.value = _currentTargetHealth;
        }
    }
}