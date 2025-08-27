using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.SharedLogic
{
    public class GradualHealthChanger : MonoBehaviour
    {
        [SerializeField] private Image _healthBar;
        [SerializeField] private float _timeToReachTargetHealth = 1f;
        private float _currentTargetHealth;
        private float _maxHealth;

        public void Initialize(float maxHealth)
        {
            _maxHealth = maxHealth;
            _currentTargetHealth = maxHealth;
            _healthBar.fillAmount = 1.0f;
        }

        public void SetTargetHealth(float targetHealth)
        {
            StopAllCoroutines();
            _currentTargetHealth = Mathf.Clamp(targetHealth, 0, _maxHealth);
            StartCoroutine(GraduallyUpdateHealth());
        }

        private IEnumerator GraduallyUpdateHealth()
        {
            float initialHealth = _healthBar.fillAmount;
            float elapsedTime = 0f;
            float targetFill = _currentTargetHealth / _maxHealth;
            while (elapsedTime < _timeToReachTargetHealth)
            {
                elapsedTime += Time.deltaTime;
                _healthBar.fillAmount = Mathf.SmoothStep(initialHealth, targetFill, elapsedTime / _timeToReachTargetHealth);
                yield return null;
            }
            _healthBar.fillAmount = targetFill;
        }
    }
}