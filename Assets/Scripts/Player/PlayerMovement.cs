using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _doubleJumpForce = 2.5f;
    [SerializeField] private LayerMask _groundLayerMask;

    private Vector2 _inputMoveDirection;
    private Vector2 _previousInputMoveDirection;
    private bool _isGrounded = true;
    private bool _canDoubleJump;
    private bool _isJumping;
    private bool _isFalling;

    public UnityEvent OnMovementBegin = new UnityEvent();
    public UnityEvent OnMovementEnd = new UnityEvent();

    public UnityEvent OnJumpBegin = new UnityEvent();
    public UnityEvent OnJumpEnd = new UnityEvent();

    public UnityEvent OnDoubleJumpBegin = new UnityEvent();
    public UnityEvent OnFallBegin = new UnityEvent();

    private void FixedUpdate()
    {
        _rigidbody.linearVelocityX = _inputMoveDirection.x * _movementSpeed;
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.5f, _groundLayerMask);

        if(_isJumping && _rigidbody.linearVelocityY <= 0.0f)
        {
            if(_isGrounded) // Ground hit
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
        if (context.canceled)
        {
            _inputMoveDirection = Vector2.zero;
            OnMovementEnd?.Invoke();
        }
        else
        {
            _inputMoveDirection = context.ReadValue<Vector2>();
            if (_inputMoveDirection.x < 0.0f)
            {
                _spriteRenderer.flipX = true;
            }
            else
            {
                _spriteRenderer.flipX = false;
            }

            if (_previousInputMoveDirection.x == 0.0f && _inputMoveDirection.x != 0.0f) OnMovementBegin?.Invoke();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
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

    public void StartGoingDown()
    {
        if (_isGrounded || _isFalling) return;
        _rigidbody.linearVelocityY = 0.0f;
        _isFalling = true;
        OnFallBegin.Invoke();
    }
}