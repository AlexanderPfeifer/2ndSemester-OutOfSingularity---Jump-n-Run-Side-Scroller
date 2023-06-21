using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class EnemyScript : MonoBehaviour
{
    #region Health Variables
    [Header("Health")]
    [SerializeField] int maxHealthEnemy;
    public int currentHealthEnemy;
    public UnityEvent<GameObject> OnHitWithReference;
    #endregion

    #region Attack Variables
    [Header("Attack")]
    [SerializeField] float attackRange;
    [SerializeField] float attackRate = 2;
    [SerializeField] float nextAttackTime = 0;
    [SerializeField] float playerInRangeDistance;
    [SerializeField] float speed;
    [SerializeField] int attackDamage;
    [SerializeField] private Transform player;
    public Transform attackPoint;
    public LayerMask attackMask;
    #endregion

    #region Detection Variables
    [Header("Movement")]
    [SerializeField] private Transform detectorOrigin;
    [SerializeField] float detectionDelay;
    public Vector2 detectorSize = Vector2.one;
    public Vector2 detectorOriginOffset = Vector2.zero;
    public LayerMask detectorLayerMask;
    public bool isFlipped = false;
    private GameObject target;
    [field: SerializeField] public bool PlayerDetected { get; private set; }
    #endregion

    #region ScriptAndScript Variables
    PlayerScript playerScript;
    Rigidbody2D rb;
    Animator anim;
    #endregion

    #region Target Methods
    public Vector2 DirectionToTarget => target.transform.position - detectorOrigin.position;

    public GameObject Target
    {
        get => target;
        private set
        {
            target = value;
            PlayerDetected = target != null;
        }
    }
    #endregion

    #region Unity Methods
    private void Start()
    {
        currentHealthEnemy = maxHealthEnemy;
        StartCoroutine(DetectionCoroutine());
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        LookAtPlayer();
        PerformDetection();

        if(Vector2.Distance(player.position, rb.position) <= playerInRangeDistance)
        {
            anim.SetTrigger("EnemyAttack");
        }
    }
    #endregion

    public void TakeDamageEnemy(int damage, GameObject sender)
    {
        currentHealthEnemy -= damage;

        if (currentHealthEnemy > 0)
        {
            OnHitWithReference?.Invoke(sender);
        }

        if(currentHealthEnemy <= 0)
        {
            anim.SetTrigger("Death");
        }
    }

    public void Attack()
    {
        Collider2D colInfo = Physics2D.OverlapCircle(attackPoint.position, attackRange, attackMask);
        {
            if (colInfo != null)
            {
                if (playerScript = colInfo.GetComponent<PlayerScript>())
                {
                    if (Time.time >= nextAttackTime)
                    {
                        StartCoroutine(MovementLock());
                        playerScript.TakeDamagePlayer(attackDamage);
                        nextAttackTime = Time.time + 1f / attackRate;
                        FindObjectOfType<HitStopScript>().FreezeTime(0.2f);
                    }
                }
            }
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    #region Player Detection Methods
    public void LookAtPlayer()
    {
        Vector3 flipped = transform.localScale;
        flipped.x *= -1f;

        if (transform.position.x > player.position.x && isFlipped)
        {
            transform.localScale = flipped;
            isFlipped = false;
        }
        else if (transform.position.x < player.position.x && !isFlipped)
        {
            transform.localScale = flipped;
            isFlipped = true;
        }
    }

    public void PerformDetection()
    {
        Collider2D collider = Physics2D.OverlapBox((Vector2)detectorOrigin.position + detectorOriginOffset, detectorSize, 0, detectorLayerMask);
        if (collider != null)
        {
            Target = collider.gameObject;
            Vector2 target = new Vector2(player.position.x, rb.position.y);
            Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
        }
        else
        {
            Target = null;
        }
    }

    IEnumerator DetectionCoroutine()
    {
        yield return new WaitForSeconds(detectionDelay);
        PerformDetection();
        StartCoroutine(DetectionCoroutine());
    }

    private void StopMoving()
    {
        rb.constraints = RigidbodyConstraints2D.FreezePositionX;
    }

    private void Move()
    {
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezePositionY;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    IEnumerator MovementLock()
    {
        StopMoving();
        yield return new WaitForSeconds(2f);
        Move();
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

        if (detectorOrigin != null)
        {
            Gizmos.color = Color.green;
            if (PlayerDetected)
            {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawWireCube((Vector2)detectorOrigin.position + detectorOriginOffset, detectorSize);
        }
    }
}
