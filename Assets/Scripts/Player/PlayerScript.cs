using System.Collections;
using System.Dynamic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    #region Dash Variables
    [Header("Dash")]
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private float dashingPower;
    [SerializeField] private float dashingTime;
    [SerializeField] private float dashingCooldown;
    private bool canDash = true;
    private bool isDashing;
    #endregion

    #region Jump Variables
    [Header("Jump")]
    [SerializeField] private float jumpingPower;
    [SerializeField] private float coyoteTime;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    private bool isGrounded;
    private float coyoteTimeCounter;
    #endregion

    #region Fall Variables
    [Header("Fall")]
    [SerializeField] private CapsuleCollider2D playerCollider;
    [SerializeField] private float fallDuration;
    private GameObject currentOneWayPlatform;
    #endregion

    #region Movement Variables
    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] float speedChangeRate;
    [SerializeField] Vector2 moveInput;
    private readonly PlayerInput inputActions;
    private InputAction moveAction;
    public bool isFacingRight = true;
    private Animator anim;
    private SpriteRenderer sr;
    private Vector2 lastMovement;
    #endregion

    #region Wall Jump Variables
    [Header("Wall Jump")]
    [SerializeField] private Transform wallRaycastCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallJumpingDuration;
    [SerializeField] private float wallSlidingSpeed;
    [SerializeField] private float wallRaycastLength;
    private bool isWallJumping;
    private bool isTouchingLeft;
    private bool isTouchingRight;
    private float touchingLeftorRight;
    #endregion

    #region Health Variables
    [Header("Health")]
    [SerializeField] public int maxHealthPlayer;
    public int currentHealthPlayer;
    private bool canHeal = true;
    public HealthbarScript healthbarScript;
    private Vector2 startPos;
    Color c;
    #endregion

    #region Attack Variables
    [Header("Attack")]
    [SerializeField] float attackRange;
    [SerializeField] float heavyAttackRange;
    [SerializeField] float attackRate;
    [SerializeField] int enemyColDamage;
    [SerializeField] int attackDamage;
    [SerializeField] int heavyAttackDamage;
    [SerializeField] LayerMask enemyLayers;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float minCooldownTime;
    [SerializeField] private float maxChargedAttackCooldownTime;
    [SerializeField] private float maxHealCooldownTime;
    private float currentHealCooldownTime;
    private float currentChargedAttackCooldownTime;
    private bool canChargeAttack = true;
    public Transform attackPoint;
    public Transform heavyAttackPoint;
    readonly EnemyScript enemyScript;
    public float chargeTime;
    private float nextAttackTime;
    private float nextHeavyAttackTime;
    private bool isCharging;
    PlayerInput playerInput;
    #endregion

    #region Script Variables
    public SpiraleBulletPattern spiraleBulletPattern;
    public CooldownScript cooldownScript;
    public Animator enemyAnimator;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        playerInput = new PlayerInput();

        moveAction = playerInput.player.movement;
    }

    private void Start()
    {
        currentHealthPlayer = maxHealthPlayer;
        rb = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        c = sr.material.color;
        startPos = transform.position;
        healthbarScript.SetMaxHealth(maxHealthPlayer);
        cooldownScript.SetMinCooldown(minCooldownTime);
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void Update()
    {
        if (isDashing)
        {
            return;
        }

        moveInput = moveAction.ReadValue<Vector2>();

        bool isRightButtonHeld = playerInput.player.chargedAttack.ReadValue<float>() > 0.1f;

        if (isRightButtonHeld)
        {
            isCharging = true;
            if (isCharging == true)
            {
                chargeTime += Time.deltaTime * chargeSpeed;
            }
        }

        WallSlide();

        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if(!canHeal)
        {
            currentHealCooldownTime -= Time.deltaTime;
            cooldownScript.SetCurrentHealCooldown(currentHealCooldownTime);
        }
        if (currentHealCooldownTime <= 0)
        {
            canHeal = true;
        }

        if (!canChargeAttack)
        {
            currentChargedAttackCooldownTime -= Time.deltaTime;
            cooldownScript.SetCurrentChargedAttackCooldown(currentChargedAttackCooldownTime);
        }
        if (currentChargedAttackCooldownTime <= 0)
        {
            canChargeAttack = true;
        }

        CheckCollisions();
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        Move();

        if (isWallJumping && moveInput.x != 0)
        {
            rb.velocity = new Vector2(touchingLeftorRight * speed, jumpingPower);
        }
    }

    private void LateUpdate()
    {
        UpdateAnimator();
    }
    #endregion

    #region CallbackContext Methods 
    public void Jump(InputAction.CallbackContext context)
    {
        if (coyoteTimeCounter > 0f && context.performed)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }

        if (context.canceled && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

            coyoteTimeCounter = 0f;
        }
    }

    public void Fall(InputAction.CallbackContext context)
    {
        if(context.performed && currentOneWayPlatform != null)
        {
            StartCoroutine(DisableCollision());
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if(context.performed && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    public void WallJump(InputAction.CallbackContext context)
    {
        if (context.performed && (isTouchingLeft || isTouchingRight) && !IsGrounded())
        {
            isWallJumping = true;
            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    public void Heal(InputAction.CallbackContext context)
    {
        if (context.performed && currentHealthPlayer < 100 && canHeal)
        {
            canHeal = false;
            currentHealCooldownTime = maxHealCooldownTime;
            cooldownScript.SetCurrentHealCooldown(maxHealCooldownTime);
            currentHealthPlayer = maxHealthPlayer;
            healthbarScript.SetHealth(maxHealthPlayer);
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (Time.time >= nextAttackTime)
        {
            if (context.performed)
            {
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
                foreach (Collider2D collider in hitEnemies)
                {
                    EnemyScript enemyScript;
                    if (enemyScript = collider.GetComponent<EnemyScript>())
                    {
                        enemyScript.TakeDamageEnemy(attackDamage, transform.gameObject);
                    }
                }
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    public void ChargedAttack(InputAction.CallbackContext context)
    {
        if (context.performed && chargeTime >= 5f)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(heavyAttackPoint.position, heavyAttackRange, enemyLayers);
            foreach (Collider2D collider in hitEnemies)
            {
                EnemyScript enemyScript;
                if (enemyScript = collider.GetComponent<EnemyScript>())
                {
                    enemyScript.TakeDamageEnemy(heavyAttackDamage, transform.gameObject);
                    chargeTime = 0;
                    canChargeAttack = false;
                    currentChargedAttackCooldownTime = maxChargedAttackCooldownTime;
                    cooldownScript.SetCurrentChargedAttackCooldown(maxHealCooldownTime);
                }
            }        
        }
    }
    #endregion

    #region Coroutine Methods
    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    IEnumerator GetInvulnerable()
    {
        Physics2D.IgnoreLayerCollision(3, 8, true);
        c.a = 0.5f;
        sr.material.color = c;
        yield return new WaitForSeconds(3f);
        Physics2D.IgnoreLayerCollision(3, 8, false);
        c.a = 1f;
        sr.material.color = c;
    }

    IEnumerator Respawn(float duration)
    {
        if (isFacingRight)
        {
            rb.simulated = false;
            rb.velocity = new Vector2(0, 0);
            transform.localScale = new Vector3(0, 0, 0);
            yield return new WaitForSeconds(duration);
            transform.position = startPos;
            transform.localScale = new Vector3(1f, 1f, 1f);
            rb.simulated = true;
            currentHealthPlayer = maxHealthPlayer;
        }

        if (!isFacingRight)
        {
            rb.simulated = false;
            rb.velocity = new Vector2(0, 0);
            transform.localScale = new Vector3(0, 0, 0);
            yield return new WaitForSeconds(duration);
            transform.position = startPos;
            transform.localScale = new Vector3(-1f, 1f, 1f);
            rb.simulated = true;
            currentHealthPlayer = maxHealthPlayer;
        }
    }

    IEnumerator DisableCollision()
    {
        BoxCollider2D platformCollider = currentOneWayPlatform.GetComponent<BoxCollider2D>();

        Physics2D.IgnoreCollision(playerCollider, platformCollider);
        yield return new WaitForSeconds(fallDuration);
        Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
    }

    IEnumerator StopFire()
    {
        yield return new WaitForSeconds(3f);
        spiraleBulletPattern.enabled = false;
        spiraleBulletPattern.StopFire();
    }
    #endregion

    #region On-Enter/Exit Methods
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy") && currentHealthPlayer > 0)
        {
            TakeDamagePlayer(enemyColDamage);
            StartCoroutine(GetInvulnerable());
        }

        if (currentHealthPlayer <= 0)
        {
            Die();
        }
        else if (col.CompareTag("Checkpoint"))
        {
            startPos = transform.position;
        }

        if(col.gameObject.CompareTag("OneWayPlatform"))
        {
            currentOneWayPlatform = col.gameObject;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("OneWayPlatformBossFight"))
        {
            currentOneWayPlatform = col.gameObject;
            spiraleBulletPattern.enabled = true;
            enemyAnimator.SetTrigger("bulletHell");
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("OneWayPlatform") || col.gameObject.CompareTag("OneWayPlatformBossFight"))
        {
            currentOneWayPlatform = null;
        }
    }
    #endregion

    #region Health Methods
    public void TakeDamagePlayer(int damage)
    {
        currentHealthPlayer -= damage;

        healthbarScript.SetHealth(currentHealthPlayer);

        if(currentHealthPlayer <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        StartCoroutine(Respawn(0.5f));
    }
    #endregion

    #region Movement Methods
    public void Move()
    {
        float targetSpeed = moveInput == Vector2.zero ? 0 : speed * moveInput.magnitude;

        Vector2 currentVelocity = lastMovement;

        currentVelocity.y = 0;

        float currentSpeed = currentVelocity.magnitude;

        if (Mathf.Abs(currentSpeed - targetSpeed) > 0.01f)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, speedChangeRate * Time.deltaTime);
        }
        else
        {
            currentSpeed = targetSpeed;
        }

        if (!isFacingRight && moveInput.x > 0)
        {
            Flip();
        }
        else if (isFacingRight && moveInput.x < 0)
        {
            Flip();
        }

        Vector2 movement = moveInput * currentSpeed;

        rb.velocity = new Vector2(movement.x, rb.velocity.y);

        lastMovement = movement;
    }

    public void Flip()
    {
        if (isFacingRight && moveInput.x < 0 || !isFacingRight && moveInput.x > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void WallSlide()
    {
        if((isTouchingLeft || isTouchingRight) && !IsGrounded() && moveInput.x != 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void CheckCollisions()
    {
        isTouchingLeft = Physics2D.Raycast(wallRaycastCheck.position, Vector2.left, wallRaycastLength, wallLayer);
        isTouchingRight = Physics2D.Raycast(wallRaycastCheck.position, Vector2.right, wallRaycastLength, wallLayer);

        if (isTouchingLeft)
        {
            touchingLeftorRight = 1;
        }
        else if (isTouchingRight)
        {
            touchingLeftorRight = -1;
        }
    }
    #endregion

    #region Animation Methods
    private void UpdateAnimator()
    {
        Vector2 velocity = lastMovement;
        velocity.y = 0;
        float speed = velocity.magnitude;
    }
    #endregion

    #region OnDrawGizmos Method
    private void OnDrawGizmos()
    {
        if (attackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(heavyAttackPoint.position, heavyAttackRange);

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(wallRaycastCheck.position, wallRaycastCheck.position + Vector3.right * wallRaycastLength);
        Gizmos.DrawLine(wallRaycastCheck.position, wallRaycastCheck.position + Vector3.left * wallRaycastLength);
    }
    #endregion
}
