using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class EnemyScript : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] int maxHealthEnemy;
    public int currentHealthEnemy;
    public UnityEvent<GameObject> OnHitWithReference;

    [Header("Attack")]
    [SerializeField] float attackRange;
    [SerializeField] float attackDelay;
    [SerializeField] float passedTimeBetweenAttacks;
    [SerializeField] float speed;
    [SerializeField] int attackDamage;
    [SerializeField] private Transform player;
    public Transform attackPoint;
    public LayerMask attackMask;
    private Animator anim;

    [Header("Movement")]
    [SerializeField] private Transform detectorOrigin;
    [SerializeField] float detectionDelay;
    public Vector2 detectorSize = Vector2.one;
    public Vector2 detectorOriginOffset = Vector2.zero;
    public LayerMask detectorLayerMask;
    public bool isFlipped = false;
    public bool showGizmos = true;
    private Rigidbody2D rb;
    private GameObject target;

    [field: SerializeField] public bool PlayerDetected { get; private set; }

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

        if(Vector2.Distance(player.position, rb.position) <= attackRange)
        {
            if(passedTimeBetweenAttacks >= attackDelay)
            {
                passedTimeBetweenAttacks = 0;
                Attack();
            }
        }

        if(passedTimeBetweenAttacks < attackDelay)
        {
            passedTimeBetweenAttacks += Time.deltaTime;
        }
    }

    public void TakeDamageEnemy(int damage, GameObject sender)
    {
        currentHealthEnemy -= damage;

        if (currentHealthEnemy > 0)
        {
            OnHitWithReference?.Invoke(sender);
        }
        else
        {
            anim.SetTrigger("Death");
        }
    }

    public void Attack()
    {
        Collider2D colInfo = Physics2D.OverlapCircle(attackPoint.position, attackRange, attackMask);
        if (colInfo != null)
        {
            PlayerScript playerScript;
            if(playerScript = colInfo.GetComponent<PlayerScript>())
            {
                FindObjectOfType<HitStopScript>().FreezeTime(0.1f);
                playerScript.TakeDamagePlayer(attackDamage);
            }
        }
    }

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

    private void Die()
    {
        Destroy(gameObject);
    }

    IEnumerator DetectionCoroutine()
    {
        yield return new WaitForSeconds(detectionDelay);
        PerformDetection();
        StartCoroutine(DetectionCoroutine());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

        if (showGizmos && detectorOrigin != null)
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
