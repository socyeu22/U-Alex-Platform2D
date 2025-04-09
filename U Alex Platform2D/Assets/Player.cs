using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _firtJumpForce = 5f;
    [SerializeField] private float _secondJumpForce = 3f;
    
    private bool _canDoubleJump = true;

    [Header("Collision Info")]
    [SerializeField] private float _raycastLenght = 0.5f;
    [SerializeField] private LayerMask _layerMask;
    private bool _isGrounded;
    private bool _isAirborne;

    private bool facingRight = true;
    private int facingDir = 1;
    private float _xInput;
    
    private Rigidbody2D _rb;
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        UpdateAirbornStatus();
        HandleCollision();
        HandleInput();
        HandleMovement();
        HandleAnimations();
        HandleFlip();
    }

    private void UpdateAirbornStatus()
    {
        if (_isGrounded && _isAirborne)
            HandleLanding();

        if (!_isGrounded && !_isAirborne)
            BecomeAirborn();
    }

    private void BecomeAirborn()
    {
        _isAirborne = true;
    }

    private void HandleLanding()
    {
        _isAirborne = false;
        _canDoubleJump = true;
    }

    private void HandleInput()
    {
        _xInput = Input.GetAxisRaw("Horizontal");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpButton();
        }

    }

    private void Jump() => _rb.velocity = new Vector2(_rb.velocity.x, _firtJumpForce);

    private void DoubleJump()
    {
        _rb.velocity = new(_rb.velocity.x, _secondJumpForce);
    }

    private void JumpButton()
    {
        if(_isGrounded)
        {
            Jump();
        }
        else if(_canDoubleJump)
        {
            DoubleJump();
            _canDoubleJump = false;
        }     
    }

    private void HandleCollision()
    {
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, _raycastLenght, _layerMask);
    }

    private void HandleAnimations()
    {
        _animator.SetFloat("xVelocity", _rb.velocity.x);
        _animator.SetFloat("yVelocity", _rb.velocity.y);
        _animator.SetBool("isGrounded", _isGrounded);
    }

    private void HandleMovement()
    {
        _rb.velocity = new Vector2(_xInput * _moveSpeed, _rb.velocity.y);
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * _raycastLenght);
    }
    private void HandleFlip()
    {
        if(_rb.velocity.x < 0 && facingRight || _rb.velocity.x > 0 && facingRight == false)
            Flip(); 
    }
    private void Flip()
    {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
        facingDir = facingDir * (-1);
    }
}
