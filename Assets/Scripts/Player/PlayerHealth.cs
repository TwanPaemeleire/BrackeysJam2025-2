using Assets.Scripts.SharedLogic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private float _maxHealth = 10.0f;
        [SerializeField] private GradualHealthChanger _gradualHealthChanger;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private float _flashDuration = 0.1f;
        [SerializeField] private Color _damageFlashColor = Color.red;
        [SerializeField] private Color _healFlashColor = Color.limeGreen;
        [SerializeField] private float _invincibleTimeAfterHit = 0.3f;
        [SerializeField] private Slider _healthBar;

        private float _currentHealth;
        private Color _originalColor;
        private bool _isInvincible = false;
        private bool _hasDied = false;

        private Coroutine _invincibilityCoroutine;
        private Coroutine _flashCoroutine;

        public UnityEvent OnHit = new UnityEvent();
        public UnityEvent OnDeath = new UnityEvent();
        public UnityEvent OnHeal = new UnityEvent();

        private void Awake()
        {
            _currentHealth = _maxHealth;
            _originalColor = _spriteRenderer.color;
            _gradualHealthChanger.Initialize(_maxHealth);
        }

        public void TakeDamage(float damage)
        {
            if(_hasDied) return;
            if(_isInvincible) return;
            _currentHealth -= damage;
            _gradualHealthChanger.SetTargetHealth(_currentHealth);

            if (_currentHealth <= 0.0f)
            {
                _currentHealth = 0.0f;
                _hasDied = true;
                OnDeath?.Invoke();
            }
            else
            {
                FlashSprite(_damageFlashColor);
                StartInvincibilityTime();
                OnHit?.Invoke();
            }
        }

        public void Heal(float healAmount)
        {
            if(_hasDied) return;
            FlashSprite(_healFlashColor);
            _currentHealth = Mathf.Clamp(_currentHealth += healAmount, 0, _maxHealth);
            OnHeal?.Invoke();
            _gradualHealthChanger.SetTargetHealth(_currentHealth);
        }

        private void FlashSprite(Color flashColor)
        {
            if(_flashCoroutine != null)
            {
                StopCoroutine(_flashCoroutine);
                _spriteRenderer.color = _originalColor;
            }
            _flashCoroutine = StartCoroutine(FlashCoroutine(flashColor));
        }

        private void StartInvincibilityTime()
        {
            if(_invincibilityCoroutine != null)
            {
                StopCoroutine(_invincibilityCoroutine);
            }
            _invincibilityCoroutine = StartCoroutine(InvincibilityCoroutine());
        }

        private IEnumerator FlashCoroutine(Color flashColor)
        {
            _spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(_flashDuration);
            _spriteRenderer.color = _originalColor;
        }

        private IEnumerator InvincibilityCoroutine()
        {
            _isInvincible = true;
            yield return new WaitForSeconds(_invincibleTimeAfterHit);
            _isInvincible = false;
        }
    }
}