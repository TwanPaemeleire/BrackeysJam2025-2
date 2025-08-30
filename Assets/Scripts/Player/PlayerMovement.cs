using Assets.Scripts.General;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private float _movementSpeed = 5.0f;
        [SerializeField] private float _jumpForce = 5.0f;
        [SerializeField] private float _doubleJumpForce = 2.5f;
        [SerializeField] private float _rollCooldown = 1.0f;
        [SerializeField] private float _rollTime = 0.5f;
        [SerializeField] private float _rollSpeed = 5.0f;
        [SerializeField] private float _timeBeforeRollEndTrigger = 0.2f;
        [SerializeField] private LayerMask _groundLayerMask;

        [Header("Sounds")]
        [SerializeField] private AudioClip _rollSound;
        [SerializeField] private float _rollSoundVolume;
        [SerializeField] private AudioSource _footStepAudioSource;

        private Vector2 _inputMoveDirection;
        private Vector2 _previousInputMoveDirection;
        private bool _isGrounded = true;
        private bool _canDoubleJump;
        private bool _isJumping;
        private bool _isFalling;
        private bool _canMove = true;
        private bool _canRoll = true;
        private bool _isRolling;
        private Animator _animator;
        private PlayerHealth _playerHealth;
        private BoxCollider2D _boxCollider2D;
        private float _boxBottomDistanceFromTransform;

        public bool IsGrounded { get { return _isGrounded; } }
        public bool IsRolling { get { return _isRolling; } }
        public bool IsJumping { get { return _isJumping; } }

        public UnityEvent OnMovementBegin = new UnityEvent();
        public UnityEvent OnMovementEnd = new UnityEvent();

        public UnityEvent OnJumpBegin = new UnityEvent();
        public UnityEvent OnJumpEnd = new UnityEvent();

        public UnityEvent OnDoubleJumpBegin = new UnityEvent();
        public UnityEvent OnFallBegin = new UnityEvent();

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _animator.SetTrigger("GoIdle");
            _playerHealth = GetComponent<PlayerHealth>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _boxBottomDistanceFromTransform = transform.position.y - _boxCollider2D.bounds.min.y;
        }

        public void ResetMovement()
        {
            StopAllCoroutines();
            _isJumping = false;
            _isRolling = false;
            _isGrounded = false;
            _canDoubleJump = false;
            _canMove = true;
            _canRoll = true;
            _isFalling = false;
            _animator.SetTrigger("GoIdle");
        }

        private void FixedUpdate()
        {
            if(!_isRolling && _canMove)
            {
                _rigidbody.linearVelocityX = _inputMoveDirection.x * _movementSpeed;
            }
            RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, _boxBottomDistanceFromTransform + 1.1f, _groundLayerMask);
            
            if(hit.collider == null)
            {
                _isGrounded = false;
            }
            else if(hit.distance < _boxBottomDistanceFromTransform + 0.1f) // Distance is grounded
            {
                _isGrounded = true;
            }
            else if(_isFalling && hit.distance < _boxBottomDistanceFromTransform + 1.0f) // Distance is not grounded, but enough to start falling anim
            {
                
                _animator.SetTrigger("JumpGroundReached");
                _isGrounded = false;
            }
            else
            {
                _isGrounded = false;
            }

            if (_isJumping && _rigidbody.linearVelocityY <= 0.0f)
            {
                if (_isGrounded) // Ground hit
                {
                    _isJumping = false;
                    OnJumpEnd?.Invoke();
                    SetAnimationAfterExecutingAttack();
                }
                else // Start going down in air
                {
                    StartGoingDown();
                }
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _previousInputMoveDirection = _inputMoveDirection;
            _inputMoveDirection = context.ReadValue<Vector2>();
            if (!_canMove) return;
            if (context.canceled)
            {
                _inputMoveDirection = Vector2.zero;
                if(_isGrounded) _animator.SetTrigger("GoIdle");
                OnMovementEnd?.Invoke();
            }
            else
            {
                _inputMoveDirection = context.ReadValue<Vector2>();
                CheckSpriteOrientation();

                if (_previousInputMoveDirection.x == 0.0f && _inputMoveDirection.x != 0.0f)
                {
                    if(!_isJumping && _isGrounded) _animator.SetTrigger("Moving");
                    OnMovementBegin?.Invoke();
                }
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (!_canMove) return;
            if (context.started)
            {
                if (_isGrounded)
                {
                    _rigidbody.linearVelocityY = 0.0f;
                    _rigidbody.AddForce(transform.up * _jumpForce, ForceMode2D.Impulse);
                    _canDoubleJump = true;
                    _isJumping = true;
                    _isFalling = false;
                    _animator.SetTrigger("JumpUp");
                    OnJumpBegin?.Invoke();
                }
                else if (_canDoubleJump)
                {
                    _rigidbody.linearVelocityY = 0.0f;
                    _rigidbody.AddForce(transform.up * _doubleJumpForce, ForceMode2D.Impulse);
                    _canDoubleJump = false;
                    _isFalling = false;
                    _animator.SetTrigger("JumpUp");
                    OnDoubleJumpBegin?.Invoke();
                }
            }
        }

        public void OnRoll(InputAction.CallbackContext context)
        {
            if (!_canMove) return;
            if (context.started && !_isRolling && _isGrounded && _canRoll)
            {
                _playerHealth.IsInvincible = true;
                _isRolling = true;
                _canMove = false;
                _canRoll = false;
                SoundManager.Instance.PlaySFX(_rollSound, _rollSoundVolume);
                float rollDirection = transform.localScale.x < 0.0f ? -1f : 1f;
                _rigidbody.linearVelocityX = rollDirection * _rollSpeed;
                _animator.SetTrigger("Roll");
                StartCoroutine(RollCoroutine(rollDirection));
            }
        }

        public void EndRoll()
        {
            if(_inputMoveDirection != Vector2.zero) CheckSpriteOrientation();
            _isRolling = false;
            _canMove = true;
            _playerHealth.IsInvincible = false;
            if (_inputMoveDirection.x != 0.0f)
            {
                _animator.SetTrigger("Moving");
            }
            else
            {
                _animator.SetTrigger("GoIdle");
            }
        }

        private IEnumerator RollCoroutine(float rollDirectionMultiplier)
        {
            float elapsed = 0f;
            bool hasStartedEndAnim = false;
            while (elapsed < _rollTime)
            {
                _rigidbody.linearVelocityX = rollDirectionMultiplier * _rollSpeed;
                elapsed += Time.fixedDeltaTime;
                if(!hasStartedEndAnim && elapsed > _timeBeforeRollEndTrigger)
                {
                    hasStartedEndAnim = true;
                    _animator.SetTrigger("RollEnd");
                }
                yield return new WaitForFixedUpdate();
            }
            EndRoll();
            yield return new WaitForSeconds(_rollCooldown);
            _canRoll = true;
        }

        public void StartGoingDown()
        {
            if (_isGrounded || _isFalling) return;
            _animator.SetTrigger("JumpFalling");
            _rigidbody.linearVelocityY = 0.0f;
            _isFalling = true;
            OnFallBegin.Invoke();
        }

        public void DisableMovement()
        {
            _canMove = false;
            //_inputMoveDirection = Vector2.zero;
            _rigidbody.linearVelocityX = 0.0f;
            OnMovementEnd?.Invoke();
        }

        public void ReEnableMovement()
        {
            _canMove = true;
        }

        public void CheckSpriteOrientation()
        {
            if (_inputMoveDirection.x < 0.0f)
            {
                transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
            }
            else
            {
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
        }

        public void SetAnimationAfterExecutingAttack()
        {
            if (_inputMoveDirection.x != 0.0f)
            {
                _animator.SetTrigger("Moving");
                CheckSpriteOrientation();
            }
            else
            {
                _animator.SetTrigger("GoIdle");
            }
        }

        public void OnFootStepEvent()
        {
            _footStepAudioSource.Play();
        }
    }
}