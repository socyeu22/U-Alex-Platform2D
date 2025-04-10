using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _firtJumpForce = 5f;
    [SerializeField] private float _secondJumpForce = 3f;
    private bool _canDoubleJump = true;

    [Header("Wall Interactions")]

    [SerializeField] private float _wallJumpDuration = 0.6f;
    [SerializeField] private Vector2 _wallJumpForce;
    private bool _isWallJumping;

    [Header("Collision Info")]
    [SerializeField] private float _groundCheckDistance = 0.5f;
    [SerializeField] private float _wallCheckDistance = 0.05f;
    [SerializeField] private LayerMask _groundLayerMasks;
    
    private bool _isGrounded;
    private bool _isAirborne;
    private bool _isWallDetected;

    private bool facingRight = true;
    private int facingDir = 1;
    private float _xInput;
    private float _yInput;
    
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

        HandleInput();
        HandleWallSlide();
        HandleMovement();
        HandleFlip();
        HandleCollision();
        HandleAnimations();
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
    private void HandleWallSlide()
    {
        bool isCanWallSlide = _isWallDetected && _rb.velocity.y < 0;
        float yVelocityModifier = (_yInput < 0)? 1 : 0.5f;

        if(!isCanWallSlide)
            return;

        _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * yVelocityModifier);
    }
    private void HandleLanding()
    {
        _isAirborne = false;
        _canDoubleJump = true;
    }

    private void HandleInput()
    {
        _xInput = Input.GetAxisRaw("Horizontal");
        _yInput = Input.GetAxisRaw("Vertical");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpButton();
        }
    }

    private void Jump() => _rb.velocity = new Vector2(_rb.velocity.x, _firtJumpForce);

    private void DoubleJump()
    {
        _isWallJumping = false;
        _rb.velocity = new(_rb.velocity.x, _secondJumpForce);
    }

    private void WallJump()
    {
        _canDoubleJump = true;
        _rb.velocity = new Vector2(_wallJumpForce.x * -facingDir, _wallJumpForce.y);
        Flip();
        StopAllCoroutines();
        StartCoroutine(WallJumpRoutine());
    }

    private IEnumerator WallJumpRoutine()
    {
        _isWallJumping = true;
        yield return new WaitForSeconds(_wallJumpDuration);
        _isWallJumping = false;
    }

    private void JumpButton()
    {
        if(_isGrounded)
        {
            Jump();
        }
        else if(_isWallDetected && !_isGrounded)
        {
            WallJump();
        }
        else if(_canDoubleJump)
        {
            DoubleJump();
            _canDoubleJump = false;
        }     
    }

    private void HandleCollision()
    {
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, _groundCheckDistance, _groundLayerMasks);
        _isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, _wallCheckDistance, _groundLayerMasks);
    }

    private void HandleAnimations()
    {
        _animator.SetFloat("xVelocity", _rb.velocity.x);
        _animator.SetFloat("yVelocity", _rb.velocity.y);
        _animator.SetBool("isGrounded", _isGrounded);
        _animator.SetBool("isWallDetected", _isWallDetected);
    }

    private void HandleMovement()
    {
        if(_isWallDetected)
            return;
        if(_isWallJumping)
            return;
        _rb.velocity = new Vector2(_xInput * _moveSpeed, _rb.velocity.y);
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * _groundCheckDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * facingDir *_wallCheckDistance);

    }
    private void HandleFlip()
    {
        if(_xInput < 0 && facingRight || _xInput > 0 && facingRight == false)
            Flip(); 
    }
    private void Flip()
    {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
        facingDir = facingDir * (-1);
    }
}
