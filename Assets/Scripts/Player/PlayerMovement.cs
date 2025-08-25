using System.Collections;
using UnityEngine;
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
        [SerializeField] private LayerMask _groundLayerMask;

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

        public bool IsGrounded { get { return _isGrounded; } }
        public bool IsRolling { get { return _isRolling; } }

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
        }
        private void FixedUpdate()
        {
            if(!_isRolling) 
            {
                _rigidbody.linearVelocityX = _inputMoveDirection.x * _movementSpeed;
            }
            _isGrounded = Physics2D.Raycast(transform.position, -transform.up, 1.1f, _groundLayerMask);

            if (_isJumping && _rigidbody.linearVelocityY <= 0.0f)
            {
                if (_isGrounded) // Ground hit
                {
                    _isJumping = false;
                    OnJumpEnd?.Invoke();
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
                _animator.SetTrigger("GoIdle");
                OnMovementEnd?.Invoke();
            }
            else
            {
                _inputMoveDirection = context.ReadValue<Vector2>();
                CheckSpriteOrientation();

                if (_previousInputMoveDirection.x == 0.0f && _inputMoveDirection.x != 0.0f)
                {
                    _animator.SetTrigger("Moving");
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
                    OnJumpBegin?.Invoke();
                }
                else if (_canDoubleJump)
                {
                    _rigidbody.linearVelocityY = 0.0f;
                    _rigidbody.AddForce(transform.up * _doubleJumpForce, ForceMode2D.Impulse);
                    _canDoubleJump = false;
                    _isFalling = false;
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
                float rollDirection = _spriteRenderer.flipX ? -1f : 1f;
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
            while (elapsed < _rollTime)
            {
                _rigidbody.linearVelocityX = rollDirectionMultiplier * _rollSpeed;
                elapsed += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            EndRoll();
            yield return new WaitForSeconds(_rollCooldown);
            _canRoll = true;
        }

        public void StartGoingDown()
        {
            if (_isGrounded || _isFalling) return;
            _rigidbody.linearVelocityY = 0.0f;
            _isFalling = true;
            OnFallBegin.Invoke();
        }

        public void DisableMovement()
        {
            _canMove = false;
            _inputMoveDirection = Vector2.zero;
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
                _spriteRenderer.flipX = true;
            }
            else
            {
                _spriteRenderer.flipX = false;
            }
        }
    }
}