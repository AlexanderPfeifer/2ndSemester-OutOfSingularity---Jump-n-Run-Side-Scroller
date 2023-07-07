using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using FMOD.Studio;
using UnityEngine.InputSystem.LowLevel;

public class PlayerScript : MonoBehaviour
{
    /// <summary>
    /// enums für animation states
    /// </summary>
    #region Enums
    public enum MovementType { FirstAttack, SecondAttack, ThirdAttack, Movement, Jumping, Falling};
    public MovementType movementtype;
    #endregion

    #region Animation Variables
    /// <summary>
    /// hashs für die action IDs
    /// </summary>
    private static readonly int ActionIdHash = Animator.StringToHash("ActionId");
    /// <summary>
    /// hash für den actiontrigger 
    /// </summary>
    private static readonly int ActionTriggerHash = Animator.StringToHash("ActionTrigger");
    #endregion

    #region Audio Variables
    /// <summary>
    /// eventinstance für player footsteps
    /// </summary>
    private EventInstance playerFootsteps;
    /// <summary>
    /// event instance für den ersten und den zweiten schlag des spielers
    /// </summary>
    private EventInstance firstAndSecondAttack;
    #endregion

    #region Dash Variables
    /// <summary>
    /// stärke des dashs
    /// </summary>
    [Header("Dash")]
    [SerializeField] float dashingPower;
    /// <summary>
    /// zeit die gedashed wird
    /// </summary>
    [SerializeField] float dashingTime;
    /// <summary>
    /// cooldown des dashs
    /// </summary>
    [SerializeField] float dashingCooldown;
    /// <summary>
    /// bool ob man dashen darf
    /// </summary>
    bool canDash = true;
    /// <summary>
    /// bool ob gedashed wird
    /// </summary>
    bool isDashing;
    #endregion

    #region Jump Variables
    /// <summary>
    /// stärke des jumps
    /// </summary>
    [Header("Jump")]
    [SerializeField] float jumpingPower;
    /// <summary>
    /// zeit für coyote jump
    /// </summary>
    [SerializeField] float coyoteTime;
    /// <summary>
    /// position vom groundCheck
    /// </summary>
    [SerializeField] Transform groundCheck;
    /// <summary>
    /// layer vom groundCheck
    /// </summary>
    [SerializeField] LayerMask groundLayer;
    /// <summary>
    /// größe vom groundCheck
    /// </summary>
    [SerializeField] Vector2 groundCheckSize;
    /// <summary>
    /// bool ob man jumpen darf
    /// </summary>
    bool canJump = true;
    /// <summary>
    /// bool ob man auf dem boden ist
    /// </summary>
    bool isGrounded;
    /// <summary>
    /// zeit die vergangen ist nach dem man von einem ground fällt
    /// </summary>
    float coyoteTimeCounter;
    #endregion

    #region Fall Variables
    /// <summary>
    /// zeit die man durch eine plattform fällt
    /// </summary>
    [Header("Fall")]
    [SerializeField] float fallDuration;
    /// <summary>
    /// derzeitige oneway plattform auf der man steht
    /// </summary>
    GameObject currentOneWayPlatform;
    #endregion

    #region Movement Variables
    /// <summary>
    /// geschwindigkeit des spielers
    /// </summary>
    [Header("Movement")]
    [SerializeField] float speed;
    /// <summary>
    /// geschwindigkeit welche der maximalen geschwindigkeit angenähert wird
    /// </summary>
    [SerializeField] float speedChangeRate;
    /// <summary>
    /// moveInput für inspector
    /// </summary>
    [SerializeField] Vector2 moveInput;
    /// <summary>
    /// input action für die wasd bewegung
    /// </summary>
    InputAction moveAction;
    /// <summary>
    /// speicher von der letzten bewegung des spielers
    /// </summary>
    Vector2 lastMovement;
    /// <summary>
    /// bool ob der spierler nach rechts guckt
    /// </summary>
    public bool isFacingRight = true;
    /// <summary>
    /// bool ob der spieler sich bewegen darf
    /// </summary>
    public bool canMove = false;
    /// <summary>
    /// position des tp's für die präsentation
    /// </summary>
    [SerializeField] Vector3 tpPoint;
    #endregion

    #region Wall Jump Variables
    /// <summary>
    /// position des raycast zur erfassung ob man an der wand ist
    /// </summary>
    [Header("Wall Jump")]
    [SerializeField] Transform wallRaycastCheck;
    /// <summary>
    /// layer auf der man walljumpen kann
    /// </summary>
    [SerializeField] LayerMask wallLayer;
    /// <summary>
    /// zeit wielange der jump des walljumps andauert
    /// </summary>
    [SerializeField] float wallJumpingDuration;
    /// <summary>
    /// stärke mit der man von der wand abbounced wenn man gegen die wand springt auf der man sich befindet
    /// </summary>
    [SerializeField] float wallBounce;
    /// <summary>
    /// stärke mit der man von, zu einer anderen springen kann
    /// </summary>
    [SerializeField] float wallJumpingPower;
    /// <summary>
    /// x bounce power wenn man den walljump nicht durchführt
    /// </summary>
    [SerializeField] float wallBouncOff;
    /// <summary>
    /// y bounce power wenn man den walljump nicht durchführt
    /// </summary>
    [SerializeField] float wallJumpingPowerOff;
    /// <summary>
    /// geschwindigkeit mit der man an einer wall slided
    /// </summary>
    [SerializeField] float wallSlidingSpeed;
    /// <summary>
    /// länge des raycasts mit dem man checked ob man an einer wand ist
    /// </summary>
    [SerializeField] float wallRaycastLength;
    /// <summary>
    /// reset des movements nachdem man einen walljump durchführt
    /// </summary>
    public float movementSetBack;
    /// <summary>
    /// float ob man an der linken wand oder an der rechten ist in update mit zahlen dargestellt
    /// </summary>
    float touchingLeftorRight;
    /// <summary>
    /// bool ob man die linke wand berührt
    /// </summary>
    bool isTouchingLeft;
    /// <summary>
    /// bool ob man die rechte wand berührt
    /// </summary>
    bool isTouchingRight;
    /// <summary>
    /// bool ob man springt
    /// </summary>
    bool isJumping;
    #endregion

    #region Health Variables
    [Header("Health")]
    [SerializeField] float maxHealCooldown = 0f;
    [SerializeField] float chargedHealCooldown = 35f;
    float currentHealCooldown;
    public int maxHealthPlayer;
    public int currentHealthPlayer;
    Vector2 startPos;
    Color c;
    public bool canHeal = true;
    bool canTakeDamage = true;
    #endregion

    #region Attack Variables
    /// <summary>
    /// range der normalen attacke
    /// </summary>
    [Header("Attack")]
    [SerializeField] float attackRange;
    /// <summary>
    /// range des charge attacks
    /// </summary>
    [SerializeField] float chargeAttackRange;
    /// <summary>
    /// speed mit dem man seinen charge attack auflädt
    /// </summary>
    [SerializeField] float chargeSpeed;
    /// <summary>
    /// maximaler wert vom charge attack cooldown
    /// </summary>
    [SerializeField] float maxChargedAttackCooldown = 0f;
    
    [SerializeField] float chargedChargeAttackCooldown = 20f;
    /// <summary>
    /// layer auf der man enemies hitten kann
    /// </summary>
    [SerializeField] LayerMask enemyLayers;
    /// <summary>
    /// damage beim kollidieren mit enemy
    /// </summary>
    [SerializeField] int enemyColDamage;
    /// <summary>
    /// damage des normalen attacks
    /// </summary>
    [SerializeField] int attackDamage;
    /// <summary>
    /// damage des charge attacks
    /// </summary>
    [SerializeField] int chargeAttackDamage;
    /// <summary>
    /// licht des spielers
    /// </summary>
    [SerializeField] GameObject playerLight;
    /// <summary>
    /// höhlen eingang auf dem berg
    /// </summary>
    [SerializeField] GameObject CaveEntrance;
    /// <summary>
    /// derzeitiger cooldown vom charge attack
    /// </summary>
    float currentChargedAttackCooldown;
    /// <summary>
    /// zeit gezählt wird wenn man charge attack auflädt
    /// </summary>
    public float chargeTime = 0f;
    /// <summary>
    /// position des normalen attacks
    /// </summary>
    public Transform attackPoint;
    /// <summary>
    /// position des charge attacks
    /// </summary>
    public Transform chargeAttackPoint;
    /// <summary>
    /// destroyable stone auf dem berg
    /// </summary>
    public GameObject stone;
    /// <summary>
    /// partikel beim schlagen gegen den stein
    /// </summary>
    public Animator wallParticleAnim;
    /// <summary>
    /// bool ob man grade charged
    /// </summary>
    bool isCharging;
    /// <summary>
    /// bool ob man charge ausführen kann
    /// </summary>
    bool canChargeAttack = true;
    #endregion

    #region Camera Variables
    /// <summary>
    /// zeit wielange der screen shaken soll
    /// </summary>
    float shakeTimer;
    #endregion

    #region Scene Variables
    /// <summary>
    /// zweites chapter als string
    /// </summary>
    string secondChapter = "2.Chapter";
    #endregion

    #region ComponentAndScript Variables
    /// <summary>
    /// input map des spielers
    /// </summary>
    [Header("Components&Scripts")]
    PlayerInput playerInput;
    /// <summary>
    /// script für die cooldowns
    /// </summary>
    public CooldownScript cooldownScript;
    /// <summary>
    /// script von der healthbar
    /// </summary>
    public HealthbarScript healthbarScript;
    /// <summary>
    /// animator von den shockwaves
    /// </summary>
    [SerializeField] Animator attackPointAnimator;
    /// <summary>
    /// animator des spielers
    /// </summary>
    Animator anim;
    /// <summary>
    /// spriterenderer des spielers
    /// </summary>
    SpriteRenderer sr;
    /// <summary>
    /// rigidbody des spielers
    /// </summary>
    Rigidbody2D rb;
    /// <summary>
    /// collider des spielers
    /// </summary>
    CapsuleCollider2D playerCollider;
    /// <summary>
    /// camera des spielers
    /// </summary>
    [SerializeField] CinemachineVirtualCamera playerVC;
    /// <summary>
    /// script mit dem man die szene wechselt 
    /// </summary>
    public ChapterChange chapterChange;
    /// <summary>
    /// animator von wasd einblendung
    /// </summary>
    [SerializeField] Animator wasdUIAnim;
    /// <summary>
    /// animator von spacebar einblendung
    /// </summary>
    [SerializeField] Animator spacebarUIAnim;
    /// <summary>
    /// animator von shift einblendung
    /// </summary>
    [SerializeField] Animator shiftUIAnim;
    /// <summary>
    /// animator von right click einblendung
    /// </summary>
    [SerializeField] Animator rightClickUIAnim;
    /// <summary>
    /// gameobject von ingame UI einblendung
    /// </summary>
    [SerializeField] GameObject UICanvas;
    /// <summary>
    /// animator von e einblendung
    /// </summary>
    [SerializeField] Animator healUIAnim;
    #endregion

    #region Unity Methods
    /// <summary>
    /// bestimmt input map und movement action
    /// </summary>
    private void Awake()
    {
        playerInput = new PlayerInput();

        moveAction = playerInput.player.movement;
    }

    /// <summary>
    /// setzt derzeitiges leben mit maximalem gleich, bestimmt mehere komponenten, bestimmt material color vom player, bestimmt
    /// startposition für den checkpoint, setzt alle ingame slider auf maximalen wert und erstellt sound instanzen
    /// </summary>
    private void Start()
    {
        currentHealthPlayer = maxHealthPlayer;
        rb = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<CapsuleCollider2D>();
        c = sr.material.color;
        startPos = transform.position;
        cooldownScript.SetMaxHealCooldown(chargedHealCooldown);
        cooldownScript.SetMaxChargedAttackCooldown(chargedChargeAttackCooldown);
        healthbarScript.SetMaxHealth(maxHealthPlayer);
        playerFootsteps = AudioManager.instance.CreateInstance(FMODEvents.instance.footSteps);
        firstAndSecondAttack = AudioManager.instance.CreateInstance(FMODEvents.instance.firstAndSecondAttack);
    }

    /// <summary>
    /// aktiviert input map
    /// </summary>
    private void OnEnable()
    {
        playerInput.Enable();
    }

    /// <summary>
    /// deaktiviert input map
    /// </summary>
    private void OnDisable()
    {
        playerInput.Disable();
    }

    /// <summary>
    /// returned wenn man dashed, updated die move methode, ein isGrounded bool als overlap box und sound der footsteps
    /// </summary>
    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        Move();

        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer);

        UpdateSound();
    }

    /// <summary>
    /// hier wird returned wenn dashing true ist, das movment und der charge wird zugewiesen. 
    /// außerdem werden cooldowns und collision geupdated
    /// </summary>
    private void Update()
    {
        if (isDashing)
        {
            return;
        }

        moveInput = moveAction.ReadValue<Vector2>();

        bool isRightButtonHeld = playerInput.player.chargedAttack.ReadValue<float>() > 0.1f;

        if (isRightButtonHeld && canChargeAttack && isGrounded && !isDashing)
        {
            canJump = false;
            canMove = false;
            isCharging = true;
            if (isCharging == true)
            {
                chargeTime += Time.deltaTime * chargeSpeed;
            }
        }

        CheckWallCollisions();

        SetShakeTimer();

        SetCoyoteTimer();

        SetCooldownTimer();
    }

    /// <summary>
    /// update von animator method
    /// </summary>
    private void LateUpdate()
    {
        UpdateAnimator();
    }
    #endregion

    #region CallbackContext Methods 
    /// <summary>
    /// teleport auf dem berg
    /// </summary>
    /// <param name="context"></param>
    public void Teleport(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            transform.position = tpPoint;
        }
    }

    /// <summary>
    /// lässt character springen und wall jumpen
    /// </summary>
    /// <param name="context"></param>
    public void Jump(InputAction.CallbackContext context)
    {
        if(canJump)
        {
            isJumping = context.performed ? true : false;

            if (!isDashing)
            {
                if (context.performed && !isGrounded && (isTouchingRight || isTouchingLeft))
                {
                    canMove = false;

                    StartCoroutine(WallJumpExecute());
                }
                else if (coyoteTimeCounter > 0f && context.performed)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                    CallAnimationAction(1);
                }
                else if (context.canceled && rb.velocity.y > 0)
                {
                    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

                    coyoteTimeCounter = 0f;
                }
            }
        }
    }

    /// <summary>
    /// lässt character durch one way platformen fallen
    /// </summary>
    /// <param name="context"></param>
    public void Fall(InputAction.CallbackContext context)
    {
        if(context.performed && currentOneWayPlatform != null)
        {
            StartCoroutine(DisableCollision());
        }
    }

    /// <summary>
    /// lässt character dashen
    /// </summary>
    /// <param name="context"></param>
    public void Dash(InputAction.CallbackContext context)
    {
        if(context.performed && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    /// <summary>
    /// lässt character healen
    /// </summary>
    /// <param name="context"></param>
    public void Heal(InputAction.CallbackContext context)
    {
        if (context.performed && currentHealthPlayer < 100 && canHeal)
        {
            CallAnimationAction(3);
        }
    }

    /// <summary>
    /// lässt character eine attack animation durchführen
    /// </summary>
    /// <param name="context"></param>
    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && !isDashing && isGrounded)
        {
            isCharging = false;

            switch (anim.GetInteger(ActionIdHash))
            {
                case 0:
                    CallAttack(10, "shockWave", false);
                    break;
                case 10:
                    CallAttack(11, "middleShockwave", true);
                    break;
                case 11:
                    CallAttack(12, "heavyShockwave", true);
                    break;


                default:
                    SetActionId(0);
                    break;
            }      
        }
    }

    /// <summary>
    /// lässt player attackieren bzw schaden zufügen
    /// </summary>
    /// <param name="actionId"></param>
    /// <param name="shockWave"></param>
    /// <param name="comboAttack"></param>
    void CallAttack(int actionId, string shockWave, bool comboAttack)
    {
        rb.velocity = Vector2.zero;
        StartCoroutine(MovementLock());
        CallAnimationAttackAction(actionId, comboAttack);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D collider in hitEnemies)
        {
            EnemyScript enemyScript;
            if (enemyScript = collider.GetComponent<EnemyScript>())
            {
                enemyScript.TakeDamageEnemy(attackDamage, transform.gameObject);
                DecreaseChargeCooldown();
                DecreaseHealCooldown();
                firstAndSecondAttack.start();
                ShakeCamera(2f, 0.1f);
                attackPointAnimator.SetTrigger(shockWave);
            }

            if (collider.CompareTag("Stone"))
            {
                DecreaseChargeCooldown();
                DecreaseHealCooldown();
                ShakeCamera(2f, 0.1f);
                wallParticleAnim.SetTrigger("wallTrigger");
                attackPointAnimator.SetTrigger(shockWave);
            }
        }
    }

    /// <summary>
    /// lässt player charge attack durchführen
    /// </summary>
    /// <param name="context"></param>
    public void ChargedAttack(InputAction.CallbackContext context)
    {
        if (context.performed && chargeTime >= 5f)
        {
            ShakeCamera(5f, 0.1f);
            isCharging = false;
            SetChargeAttackCooldown();
            CallAnimationAction(15);
            attackPointAnimator.SetTrigger("chargedShockwave");
            AudioManager.instance.PlayOneShot(FMODEvents.instance.chargedAttack, this.transform.position);

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(chargeAttackPoint.position, chargeAttackRange, enemyLayers);
            foreach (Collider2D collider in hitEnemies)
            {
                EnemyScript enemyScript;
                if (enemyScript = collider.GetComponent<EnemyScript>())
                {
                    enemyScript.TakeDamageEnemy(chargeAttackDamage, transform.gameObject);
                    DecreaseHealCooldown();
                }

                if (collider.CompareTag("Stone"))
                {
                    rightClickUIAnim.SetBool("isTriggered", false);
                    ShakeCamera(7f, 0.3f);
                    Destroy(stone);
                    CaveEntrance.SetActive(true);
                    AudioManager.instance.PlayOneShot(FMODEvents.instance.stoneDestroyed, this.transform.position);
                }
            }
        }
        else if(context.performed && chargeTime < 5f)
        {
            if (chargeTime == 0 && canChargeAttack)
                CallAnimationAction(13);

            isCharging = false;
            chargeTime = 0;
            canMove = true;
            canJump = true;
        }
    }
    #endregion

    #region Coroutine Methods
    /// <summary>
    /// locked player movement
    /// </summary>
    /// <returns></returns>
    IEnumerator MovementLock()
    {
        canMove = false;
        yield return new WaitForSeconds(0.3f);
        canMove = true;
    }

    /// <summary>
    /// lässt character dashen
    /// </summary>
    /// <returns></returns>
    IEnumerator Dash()
    {
        if(!isCharging)
        {
            canDash = false;
            isDashing = true;
            CallAnimationAction(2);
            float originalGravity = rb.gravityScale;
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
            yield return new WaitForSeconds(dashingTime);
            rb.gravityScale = originalGravity;
            isDashing = false;
            yield return new WaitForSeconds(dashingCooldown);
            canDash = true;
        }
    }

    /// <summary>
    /// lässt spieler unsterblich machen
    /// </summary>
    /// <returns></returns>
    IEnumerator GetInvulnerable()
    {
        CallAnimationAction(4);
        canTakeDamage = false;
        c.a = 0.5f;
        sr.material.color = c;
        yield return new WaitForSeconds(2f);
        canTakeDamage = true;
        c.a = 1f;
        sr.material.color = c;
    }

    /// <summary>
    /// lässt spieler respawnen
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
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
            healthbarScript.SetHealth(currentHealthPlayer);
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
            healthbarScript.SetHealth(currentHealthPlayer);
        }
    }

    /// <summary>
    /// deaktiviert die collision zwischnen player und col one way platform
    /// </summary>
    /// <returns></returns>
    IEnumerator DisableCollision()
    {
        BoxCollider2D platformCollider = currentOneWayPlatform.GetComponent<BoxCollider2D>();

        Physics2D.IgnoreCollision(playerCollider, platformCollider);
        yield return new WaitForSeconds(fallDuration);
        Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
    }

    /// <summary>
    /// lässt wall jump ausführen
    /// </summary>
    /// <returns></returns>
    IEnumerator WallJumpExecute()
    {
        yield return new WaitForSeconds(0.01f);
        if ((isTouchingLeft && moveInput.x > 0) || isTouchingRight && moveInput.x < 0)
        {
            StartCoroutine(MovementSetBack(movementSetBack));
            rb.AddForce(new Vector2(touchingLeftorRight * wallBounce, wallJumpingPower), ForceMode2D.Impulse);
            CallAnimationAction(5);
            Flip();
        }
        else
        {
            StartCoroutine(MovementSetBack(movementSetBack / 2));
            rb.AddForce(new Vector2(touchingLeftorRight * wallBouncOff, wallJumpingPowerOff), ForceMode2D.Impulse);
            CallAnimationAction(5);
        }
    }

    /// <summary>
    /// resettet das movement nach einem wall jump
    /// </summary>
    /// <param name="timer"></param>
    /// <returns></returns>
    IEnumerator MovementSetBack(float timer)
    {
        yield return new WaitForSeconds(timer);
        canMove = true;
    }

    /// <summary>
    /// checked ob spieler grade springt und lässt ihn ansonsten normale weiter bewegen 
    /// </summary>
    /// <param name="movement"></param>
    /// <returns></returns>
    IEnumerator CheckWallMovement(Vector2 movement)
    {
        yield return new WaitForSeconds(0.04f);
        if (!isJumping)
        {
            rb.velocity = new Vector2(movement.x, rb.velocity.y);
        }
    }
    #endregion

    #region On-Enter/Exit Methods
    /// <summary>
    /// checked ganz viele compare tags und startet animation, fügt spieler schaden zu oder setzt checkpoint
    /// </summary>
    /// <param name="col"></param>
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy") && currentHealthPlayer > 0)
        {
            TakeDamagePlayer(enemyColDamage);
        }

        if (currentHealthPlayer <= 0)
        {
            Die();
        }
        else if (col.CompareTag("Checkpoint"))
        {
            startPos = transform.position;
        }

        if (col.CompareTag("Chapter2"))
        {
            StartCoroutine(chapterChange.LoadLevel(secondChapter));
        }

        if(col.CompareTag("WASDSetBack"))
        {
            wasdUIAnim.SetBool("WASDisShowing", false);
        }

        if (col.CompareTag("ShiftSet"))
        {
            shiftUIAnim.SetBool("isTriggered", true);
        }

        if(col.CompareTag("ShiftSetBack"))
        {
            shiftUIAnim.SetBool("isTriggered", false);
        }

        if(col.CompareTag("SpacebarSet"))
        {
            spacebarUIAnim.SetBool("isTriggered", true);
        }

        if (col.CompareTag("SpacebarSetBack"))  
        {
            spacebarUIAnim.SetBool("isTriggered", false);
        }

        if(col.CompareTag("ChargeAttackSet"))
        {
            rightClickUIAnim.SetBool("isTriggered", true);
        }
    }

    /// <summary>
    /// checked collision mit oneway platform und setzt es als col objekt
    /// </summary>
    /// <param name="col"></param>
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("OneWayPlatform"))
        {
            currentOneWayPlatform = col.gameObject;
        }
    }

    /// <summary>
    /// checked col exit und setzt one way platform auf null
    /// </summary>
    /// <param name="col"></param>
    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("OneWayPlatform"))
        {
            currentOneWayPlatform = null;
        }
    }
    #endregion

    #region Health Methods
    /// <summary>
    /// zieht spieler hp ab
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamagePlayer(int damage)
    {
        if(canTakeDamage)
        {
            currentHealthPlayer -= damage;
            StartCoroutine(GetInvulnerable());
            FindObjectOfType<HitStopScript>().FreezeTime(0.2f);
            ShakeCamera(5f, 0.1f);
        }

        healthbarScript.SetHealth(currentHealthPlayer);

        if(currentHealthPlayer <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// healed den spieler
    /// </summary>
    public void SetHealing()
    {
        canHeal = false;
        currentHealCooldown = maxHealCooldown;
        cooldownScript.SetCurrentHealCooldown(maxHealCooldown);
        currentHealthPlayer = maxHealthPlayer;
        healthbarScript.SetHealth(maxHealthPlayer);
    }

    /// <summary>
    /// lässt spieler bei tot respawnen
    /// </summary>
    void Die()
    {
        StartCoroutine(Respawn(0.5f));
    }
    #endregion

    #region Attack Methods
    /// <summary>
    /// resettet combo attacke nach erster attacke
    /// </summary>
    public void EndComboAttack1()
    {
        if(anim.GetInteger(ActionIdHash) != 11)
        {
            SetActionId(0);
        }
    }
    /// <summary>
    /// resettet combo attacke nach zweiter attacke
    /// </summary>
    public void EndComboAttack2()
    {
        if (anim.GetInteger(ActionIdHash) != 12)
        {
            SetActionId(0);
        }
    }

    /// <summary>
    /// setzt action id mit enums gleich
    /// </summary>
    /// <param name="id"></param>
    public void SetActionId(int id)
    {
        anim.SetInteger(ActionIdHash, id);

        switch (id)
        {
            case 0:
                movementtype = MovementType.Movement;
                break;
            case 10:
                movementtype = MovementType.FirstAttack;
                break;
            case 11:
                movementtype = MovementType.SecondAttack;
                break;
            case 12:
                movementtype = MovementType.ThirdAttack;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// reset von movement nach den charge attack
    /// </summary>
    void ChargeAttackMovementSetBack()
    {
        canMove = true;
        canJump = true;
    }
    #endregion

    #region Update Methods
    /// <summary>
    /// updated footstep sound 
    /// </summary>
    void UpdateSound()
    {
        if (rb.velocity.x != 0 && isGrounded)
        {
            PLAYBACK_STATE playbackState;
            playerFootsteps.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                playerFootsteps.start();
            }
        }
        else
        {
            playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }

    /// <summary>
    /// lässt camera shaken
    /// </summary>
    void SetShakeTimer()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0f)
            {
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = playerVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
            }
        }
    }

    /// <summary>
    /// setzt coyote timer
    /// </summary>
    void SetCoyoteTimer()
    {
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    /// <summary>
    /// setzt cooldowns 
    /// </summary>
    void SetCooldownTimer()
    {
        if (currentHealCooldown >= chargedHealCooldown)
        {
            canHeal = true;
        }

        if (currentChargedAttackCooldown >= chargedHealCooldown)
        {
            canChargeAttack = true;
        }

        if(chargeTime >= 5)
        {
            CallAnimationAction(14);
        }
    }

    /// <summary>
    /// checked wall collision von player 
    /// </summary>
    void CheckWallCollisions()
    {
        if (!isGrounded)
        {
            isTouchingLeft = Physics2D.Raycast(wallRaycastCheck.position, Vector2.left, wallRaycastLength, wallLayer);
            isTouchingRight = Physics2D.Raycast(wallRaycastCheck.position, Vector2.right, wallRaycastLength, wallLayer);

            if (isTouchingLeft)
            {
                touchingLeftorRight = 1;
                WallSlide();
            }
            else if (isTouchingRight)
            {
                touchingLeftorRight = -1;
                WallSlide();
            }
            else if ((!isTouchingLeft && anim.GetBool("isWalled")) || (!isTouchingRight && anim.GetBool("isWalled")))
            {
                anim.SetBool("isWalled", false);
                touchingLeftorRight = 0;
            }
        }
        else if (anim.GetBool("isWalled") && isGrounded)
        {
            anim.SetBool("isWalled", false);
            touchingLeftorRight = 0;
        }
    }

    /// <summary>
    /// updated animator bools und floats
    /// </summary>
    void UpdateAnimator()
    {
        Vector2 velocity = lastMovement;
        velocity.y = 0;
        float speed = velocity.magnitude;

        anim.SetFloat("isWalking", Mathf.Abs(moveInput.x));
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yPlayerVelocity", rb.velocity.y);
        anim.SetBool("isCharging", isCharging);
        anim.SetBool("canMove", canMove);
        if (isCharging)
        {
            SetPlayerLightTrue();
        }
        else
        {
            SetPlayerLightFalse();
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Healing"))
        {
            SetPlayerLightTrue();
        }
        else
        {
            SetPlayerLightFalse();
        }
    }

    #endregion

    #region Cooldown Methods
    /// <summary>
    /// vermindert heal cooldown bei attack
    /// </summary>
    void DecreaseHealCooldown()
    {
        if (!canHeal)
        {
            currentHealCooldown += 1;
            cooldownScript.SetCurrentHealCooldown(currentHealCooldown);
        }
    }

    /// <summary>
    /// vermindert charge cooldown bei attack
    /// </summary>
    void DecreaseChargeCooldown()
    {
        if (!canChargeAttack)
        {
            currentChargedAttackCooldown += 1;
            cooldownScript.SetCurrentChargedAttackCooldown(currentChargedAttackCooldown);
        }
    }

    /// <summary>
    /// resettet charge attack cooldown
    /// </summary>
    void SetChargeAttackCooldown()
    {
        chargeTime = 0;
        canChargeAttack = false;
        currentChargedAttackCooldown = maxChargedAttackCooldown;
        cooldownScript.SetCurrentChargedAttackCooldown(maxChargedAttackCooldown);
    }
    #endregion

    #region Movement Methods

    /// <summary>
    /// logik hinter dem movment des spielers
    /// </summary>
    public void Move()
    {
        if (canMove)
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
            if (touchingLeftorRight != 0)
            {
                StartCoroutine(CheckWallMovement(movement));
            }
            else
            {
                rb.velocity = new Vector2(movement.x, rb.velocity.y);
            }

            lastMovement = movement;
        }
    }

    /// <summary>
    /// lässt spieler flippen
    /// </summary>
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

    /// <summary>
    /// lässt spieler wallsliden
    /// </summary>
    private void WallSlide()
    {
        anim.SetBool("isWalled", true);
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
    }

    /// <summary>
    /// aktiviert wieder das movement
    /// </summary>
    public void EnableMovement()
    {
        canMove = true;
        movementtype = MovementType.Movement;
    }

    /// <summary>
    /// aktiviert wieder das jumpen
    /// </summary>
    public void EnableJumping()
    {
        canMove = true;
        movementtype = MovementType.Jumping;
    }

    /// <summary>
    /// setzt enum beim fallen
    /// </summary>
    public void EnableFalling()
    {
        movementtype = MovementType.Falling;
    }
    #endregion

    #region UI Methods
    /// <summary>
    /// setzt anim bool für wasd ingame ui
    /// </summary>
    public void SetUIAnim()
    {
        wasdUIAnim.SetBool("WASDisShowing", true);
    }

    /// <summary>
    /// setzt anim bool für e ingame ui
    /// </summary>
    public void HealUIFadeOut()
    {
        healUIAnim.SetBool("isTriggered", false);
    }

    public void HealUIFadeIn()
    {
        healUIAnim.SetBool("isTriggered", true);
    }

    /// <summary>
    /// aktiviert ingame ui canvas 
    /// </summary>
    public void ActivateUI()
    {
        UICanvas.SetActive(true);
    }
    #endregion

    #region Light Methods
    /// <summary>
    /// aktiviert player light
    /// </summary>
    void SetPlayerLightTrue()
    {
        playerLight.SetActive(true);
    }

    /// <summary>
    /// deaktiviert player light
    /// </summary>
    void SetPlayerLightFalse()
    {
        playerLight.SetActive(false);
    }
    #endregion

    #region Camera Methods
    /// <summary>
    /// lässt kamera shaken
    /// </summary>
    /// <param name="intensity"></param>
    /// <param name="time"></param>
    public void ShakeCamera(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = playerVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        shakeTimer = time;
    }
    #endregion

    #region Animation Methods
    /// <summary>
    /// ruft actiontrigger und idhash für animationen auf
    /// </summary>
    /// <param name="ActionId"></param>
    public void CallAnimationAction(int ActionId)
    {
        anim.SetTrigger(ActionTriggerHash);
        anim.SetInteger(ActionIdHash, ActionId);
    }

    /// <summary>
    /// setzt animation trigger und action id für attacks
    /// </summary>
    /// <param name="actionId"></param>
    /// <param name="comboAttack"></param>
    public void CallAnimationAttackAction(int actionId, bool comboAttack)
    {
        if(!comboAttack)
            anim.SetTrigger(ActionTriggerHash);

        SetActionId(actionId);
    }

    /// <summary>
    /// lässt spieler wieder laufen
    /// </summary>
    public void MovementUnlock()
    {
        canMove = true;
    }
    #endregion

    #region OnDrawGizmos Method
    void OnDrawGizmos()
    {
        if (attackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(chargeAttackPoint.position, chargeAttackRange);

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(wallRaycastCheck.position, wallRaycastCheck.position + Vector3.right * wallRaycastLength);
        Gizmos.DrawLine(wallRaycastCheck.position, wallRaycastCheck.position + Vector3.left * wallRaycastLength);

        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
    #endregion
}