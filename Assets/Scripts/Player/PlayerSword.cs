using Assets.Scripts.General;
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
        [SerializeField] private Transform _playerCenterPoint;

        [Header("SFX")]
        [SerializeField] private AudioClip _parrySound;
        [SerializeField] private float _parrySoundVolume;
        [SerializeField] private AudioSource _swingAudioSource;

        private bool _canParry = true;
        private bool _isParrying = false;
        private Coroutine _parryCoroutine;

        private bool _canAttack = true;
        private bool _isAttacking = false;
        private Coroutine _attackCoroutine;
        private bool _nextAttackBuffered = false;
        private bool _canDoNewMove = true;

        private PlayerMovement _playerMovement;
        private Animator _animator;

        public bool IsParrying { get { return _isParrying; } }
        public Transform PlayerCenterPoint { get { return _playerCenterPoint; } }

        public UnityEvent OnParryStart = new UnityEvent();
        public UnityEvent OnSuccesfullParry = new UnityEvent();

        public UnityEvent OnAttackStart = new UnityEvent();
        public UnityEvent OnAttackEnd = new UnityEvent();

        private void Awake()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _animator = GetComponent<Animator>();
        }

        public void ResetSword()
        {
            StopAllCoroutines();
            _parryCoroutine = null;
            _attackCoroutine = null;
            _canParry = true;
            _canAttack = true;
            _isAttacking = false;
            _isParrying = false;
            _nextAttackBuffered = false;
            _canDoNewMove = true;
        }

        public void AttemptParry(InputAction.CallbackContext context)
        {
            if(_playerMovement.IsRolling || _playerMovement.IsJumping) return; // Return if player is rolling or in air
            if (_canParry && context.started && _canDoNewMove)
            {
                _playerMovement.DisableMovement();
                _canParry = false;
                _isParrying = true;
                _canDoNewMove = false;
                OnParryStart.Invoke();
                _animator.SetTrigger("Parry");
                _parryCoroutine = StartCoroutine(ParryCoroutine());
            }
        }
        public void AttemptAttack(InputAction.CallbackContext context)
        {
            if (_playerMovement.IsRolling || _playerMovement.IsJumping) return; // Return if player is rolling or in air
            if (_canAttack && context.started && _canDoNewMove)
            {
                _playerMovement.DisableMovement();
                _canAttack = false;
                _canDoNewMove = false;
                _isAttacking = true;
                _animator.SetTrigger("Attack");
                OnAttackStart.Invoke();
            }
            else if(_isAttacking && context.started)
            {
                _nextAttackBuffered = true;
            }
        }

        public void DoHitScan() // Call from animation event
        {
            _hitScanObject.ExecuteHitScan(_damage);
        }

        public void OnAttackPartComplete()
        {
            if(_isAttacking &&  _nextAttackBuffered) // Follow up attack, animation can just keep going
            {
                _nextAttackBuffered = false;
            }
            else // Attack done, return to normal state
            {
                OnAttackFinished();
            }
        }

        private void OnAttackFinished()
        {
            _canDoNewMove = true;
            _isAttacking = false;
            _playerMovement.ReEnableMovement();
            _playerMovement.SetAnimationAfterExecutingAttack();
            _attackCoroutine = StartCoroutine(AttackCoroutine());
            OnAttackEnd.Invoke();
        }

        public void OnSuccesfullParryExecuted()
        {
            SoundManager.Instance.PlaySFX(_parrySound, _parrySoundVolume);
            StopCoroutine(_parryCoroutine);
            _parryCoroutine = null;
            OnSuccesfullParry.Invoke();
            _isParrying = false;
            StartCoroutine(ReEnableMovementAfterDelay(_moveDelayAfterParry));
        }

        private IEnumerator ParryCoroutine()
        {
            yield return new WaitForSeconds(_parryWindow);
            _isParrying = false;
            _playerMovement.ReEnableMovement();
            _playerMovement.SetAnimationAfterExecutingAttack();
            _canDoNewMove = true;
            yield return new WaitForSeconds(_parryDelay);
            _canParry = true;
        }

        private IEnumerator ReEnableMovementAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            _playerMovement.ReEnableMovement();
            _playerMovement.SetAnimationAfterExecutingAttack();
            _canParry = true;
            _canDoNewMove = true;
        }

        private IEnumerator AttackCoroutine()
        {
            yield return new WaitForSeconds(_attackDelay);
            _canAttack = true;
        }

        public void OnSwordSwing()
        {
            _swingAudioSource.Play();
        }
    }
}