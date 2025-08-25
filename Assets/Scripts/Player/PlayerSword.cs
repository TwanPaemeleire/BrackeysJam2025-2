using Assets.Scripts.SharedLogic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Player
{
    public class PlayerSword : MonoBehaviour
    {
        [SerializeField] private HitScanObject _hitScanObject;
        [SerializeField] private float _damage = 2.0f;
        [SerializeField] private float _attackDelay = 0.5f;
        [SerializeField] private float _parryDelay = 1.0f;
        [SerializeField] private float _parryWindow = 0.3f;
        [SerializeField] private float _moveDelayAfterParry = 0.1f;

        private bool _canParry = true;
        private bool _isParrying = false;
        private Coroutine _parryCoroutine;

        private bool _canAttack = true;
        private Coroutine _attackCoroutine;
        private bool _canDoNewMove = true;

        private PlayerMovement _playerMovement;

        public bool IsParrying { get { return _isParrying; } }

        public UnityEvent OnParryStart = new UnityEvent();
        public UnityEvent OnSuccesfullParry = new UnityEvent();

        public UnityEvent OnAttackStart = new UnityEvent();
        public UnityEvent OnAttackEnd = new UnityEvent();

        private void Awake()
        {
            _playerMovement = GetComponent<PlayerMovement>();
        }

        public void AttemptParry(InputAction.CallbackContext context)
        {
            if(_playerMovement.IsRolling || !_playerMovement.IsGrounded) return; // Return if player is rolling or in air
            if (_canParry && context.started && _canDoNewMove)
            {
                _playerMovement.DisableMovement();
                _canParry = false;
                _isParrying = true;
                _canDoNewMove = false;
                OnParryStart.Invoke();
                _parryCoroutine = StartCoroutine(ParryCoroutine());
            }
        }
        public void AttemptAttack(InputAction.CallbackContext context)
        {
            if (_playerMovement.IsRolling || !_playerMovement.IsGrounded) return; // Return if player is rolling or in air
            if (_canAttack && context.started && _canDoNewMove)
            {
                _playerMovement.DisableMovement();
                _canAttack = false;
                _canDoNewMove = false;
                OnAttackStart.Invoke();
                _hitScanObject.ExecuteHitScan(_damage);   
            }
        }

        public void OnAttackAnimationFinished() // Call from animation event
        {
            _playerMovement.ReEnableMovement();
            _attackCoroutine = StartCoroutine(AttackCoroutine());
            OnAttackEnd.Invoke();
            _canDoNewMove = true;
        }

        public void OnSuccesfullParryExecuted()
        {
            StopCoroutine(_parryCoroutine);
            _parryCoroutine = null;
            OnSuccesfullParry.Invoke();
            StartCoroutine(ReEnableMovementAfterDelay(_moveDelayAfterParry));
        }

        private IEnumerator ParryCoroutine()
        {
            yield return new WaitForSeconds(_parryWindow);
            _isParrying = false;
            _playerMovement.ReEnableMovement();
            _canDoNewMove = true;
            yield return new WaitForSeconds(_parryDelay);
            _canParry = true;
        }

        private IEnumerator ReEnableMovementAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            _playerMovement.ReEnableMovement();
            _canParry = true;
            _canDoNewMove = true;
        }

        private IEnumerator AttackCoroutine()
        {
            yield return new WaitForSeconds(_attackDelay);
            _canAttack = true;
        }
    }
}