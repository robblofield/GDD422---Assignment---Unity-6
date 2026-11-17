
// GDD422 AY 26/27 Assignment - The following base player movement script has been created using a number of features already explored in
// our SHORT FUSE game. In addition there is crouching that makes the character veritcally smaller and blocks the jump action
// I've added comments on the sections taken directly from short fuse. Good luck!


using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement2D : MonoBehaviour
{
    // INPUT ACTIONS

    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;

    // MOVEMENT (SHORT FUSE)

    [Range(1f, 12f)][SerializeField] private float moveSpeed = 6f;
    [Range(1f, 100f)][SerializeField] private float acceleration = 40f;
    [Range(1f, 20f)][SerializeField] private float jumpForce = 10f;
    [Range(0f, 1f)][SerializeField] private float airControl = 0.3f;

    // GROUND CHECK

    [SerializeField] private Transform groundCheck;
    [Range(0.05f, 0.5f)][SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    // CROUCH

    [Range(0.1f, 1f)][SerializeField] private float crouchHeightMultiplier = 0.5f;
    [Range(0f, 1f)][SerializeField] private float crouchSpeedMultiplier = 0.5f;

    // VISUALS

    [SerializeField] private SpriteRenderer characterSprite;
    [SerializeField] private bool spriteFacesRight = true;

    private Rigidbody2D rb;
    private BoxCollider2D playerCollider;
    private Animator animator;
    private Vector2 standingSize;
    private Vector2 standingOffset;
    private float moveInput;
    private bool crouchInput;
    private bool jumpRequested;
    private bool grounded;
    private bool crouching;
    private float currentSpeed;

    public int FacingDirection { get; private set; } = 1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        if (characterSprite == null) characterSprite = GetComponent<SpriteRenderer>();

        standingSize = playerCollider.size;
        standingOffset = playerCollider.offset;
    }

    private void OnEnable()
    {
        if (moveAction != null) moveAction.action.Enable();
        if (jumpAction != null)
        {
            jumpAction.action.Enable();
            jumpAction.action.performed += OnJump;
        }
    }

    private void OnDisable()
    {
        if (jumpAction != null)
        {
            jumpAction.action.performed -= OnJump;
            jumpAction.action.Disable();
        }
        if (moveAction != null) moveAction.action.Disable();
        jumpRequested = false;
    }

    private void Update()
    {
        // READ INPUT

        Vector2 movement = moveAction != null
            ? moveAction.action.ReadValue<Vector2>()
            : Vector2.zero;

        moveInput = movement.x;
        crouchInput = movement.y < -0.5f;

        // FACE MOVEMENT DIRECTION

        if (moveInput > 0.1f) FacingDirection = 1;
        else if (moveInput < -0.1f) FacingDirection = -1;

        if (characterSprite != null)
            characterSprite.flipX = spriteFacesRight ? FacingDirection < 0 : FacingDirection > 0;
    }

    private void FixedUpdate()
    {
        CheckGround();
        HandleCrouch();
        HandleAcceleration();
        HandleMovement();
        HandleJump();
        HandleAnimation();
    }

    // GROUND CHECK

    private void CheckGround()
    {
        grounded = groundCheck != null &&
            Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) != null;
    }

    // CROUCH

    private void HandleCrouch()
    {
        crouching = grounded && crouchInput;

        if (crouching)
        {
            float newHeight = standingSize.y * crouchHeightMultiplier;
            playerCollider.size = new Vector2(standingSize.x, newHeight);
            playerCollider.offset = standingOffset + Vector2.down * (standingSize.y - newHeight) * 0.5f;
        }
        else
        {
            playerCollider.size = standingSize;
            playerCollider.offset = standingOffset;
        }
    }

    // ACCELERATION (SHORT FUSE)

    private void HandleAcceleration()
    {
        float targetSpeed = moveInput * moveSpeed;
        if (crouching) targetSpeed *= crouchSpeedMultiplier;

        float rate = grounded ? acceleration : acceleration * airControl;
        currentSpeed = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, rate * Time.fixedDeltaTime);
    }

    // MOVEMENT (SHORT FUSE)

    private void HandleMovement()
    {
        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);
    }

    // JUMP INPUT CALLBACK

    private void OnJump(InputAction.CallbackContext context)
    {
        jumpRequested = true;
    }

    // JUMP (SHORT FUSE)

    private void HandleJump()
    {
        if (jumpRequested && grounded && !crouching)
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        jumpRequested = false;
    }

    // ANIMATION

    private void HandleAnimation()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool("IsGrounded", grounded);
        animator.SetBool("IsJumping", !grounded);
        animator.SetBool("IsCrouching", crouching);
    }

    // GIZMOS

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
