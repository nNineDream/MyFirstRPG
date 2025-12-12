using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator anim { get; private set; }
    public Rigidbody2D rb;

    public PlayerInputSet input { get; private set; }
    private StateMachine stateMachine;
    public EntityState idleState { get; private set; }
    public EntityState moveState { get; private set; }
    public EntityState jumpState { get; private set; }
    public EntityState fallState { get; private set; }
    public EntityState wallSlideState { get; private set; }
    public EntityState wallJumpState { get; private set; }
    public EntityState dashState { get; private set; }
    public EntityState basicAttackState { get; private set; }
    public EntityState jumpAttackState { get; private set; }

    [Header("Attack details")]
    public Vector2[] attackVelocity;
    public Vector2 jumpAttackVelocity;
    public float attackVelocityDuration = .1f;
    public float comboResetTime = 1;
    private Coroutine queuedAttackCo;
   

    [Header("Movement details")]
    public float moveSpeed;
    public float jumpForce = 5;
    public Vector2 wallJumpForce; 

    [Range(0, 1)]
    public float inAirMoveMultiplier = .7f;
    [Range(0, 1)]
    public float wallSlideSlowMultiplier = .7f;
    [Space]
    public float dashDuration = .25f;
    public float dashSpeed = 20;
    
    private bool facingRight = true;
    public int facingDir { get; private set; } = 1;
    public Vector2 moveInput { get; private set; }

    [Header("Collision detection")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform primaryWallDetected;
    [SerializeField] private Transform secondaryWallDetected;

    public bool groundDetected { get; private set; }
    public bool wallDetected { get; private set; }

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        stateMachine = new StateMachine();
        input = new PlayerInputSet();

        idleState = new Player_IdleState(this,stateMachine, "idle");
        moveState = new Player_MoveState(this, stateMachine, "move");
        jumpState = new Player_JumpState(this, stateMachine, "jumpFall");
        fallState = new Player_FallState(this, stateMachine, "jumpFall");
        wallSlideState = new Player_WallSlideState(this, stateMachine, "wallSlide");
        wallJumpState = new Player_WallJumpState(this, stateMachine, "jumpFall");
        dashState = new Player_DashState(this, stateMachine, "dash");
        basicAttackState = new Player_BasicAttackState(this, stateMachine, "basicAttack");
        jumpAttackState = new Player_JumpAttack(this, stateMachine, "jumpAttack");
    }
    private void OnEnable()
    {
        input.Enable();
        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero; 
    }

    private void OnDisable()
    {
        input.Disable();
    }
    private void Start()
    {
        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        HandleCollisionDetection();
        stateMachine.UpdateActiveState();
    }
    
    public void CallAnimationTrigger()
    {
        stateMachine.currentState.CallAnimationTrigger();
    }

    public void EnterAttackStateWithDelay()
    {
        if(queuedAttackCo != null)
        {
            //清空之前的缓存
            StopCoroutine(queuedAttackCo);
        }
        //更新协程
        queuedAttackCo = StartCoroutine(EnterAttackStateWithDelyCo());
    }
    private IEnumerator EnterAttackStateWithDelyCo()
    {
        yield return new WaitForEndOfFrame();
        stateMachine.ChangeState(basicAttackState);
    }

    public void SetVelocity(float xVelocity, float yVelocity) 
    {
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }
    private void HandleFlip(float xVelocity)
    {
        if (xVelocity > 0 && !facingRight) Flip();
        else if (xVelocity < 0 && facingRight) Flip();
    }
    public void Flip()
    {
        transform.Rotate(0,180,0);
        facingRight = !facingRight;
        facingDir = -facingDir;
    }
    
    private void HandleCollisionDetection()
    {
        groundDetected = Physics2D.Raycast(transform.position,Vector2.down, groundCheckDistance, whatIsGround);
        wallDetected = Physics2D.Raycast(primaryWallDetected.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround)
                    && Physics2D.Raycast(secondaryWallDetected.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -groundCheckDistance));
        Gizmos.DrawLine(primaryWallDetected.position, primaryWallDetected.position + new Vector3(wallCheckDistance*facingDir,0));
        Gizmos.DrawLine(secondaryWallDetected.position, secondaryWallDetected.position + new Vector3(wallCheckDistance*facingDir,0));
    }
}
