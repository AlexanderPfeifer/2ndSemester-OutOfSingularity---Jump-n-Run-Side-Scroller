using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossEnemyScript : MonoBehaviour
{
    #region Audio Variables
    /// <summary>
    /// Sound Variable vom enemy attack
    /// </summary>
    private EventInstance enemyAttack;
    #endregion

    #region Health Variables
    /// <summary>
    /// maximale healthpoints des enemy
    /// </summary>
    [Header("Health")]
    [SerializeField] int maxHealthEnemy;
    /// <summary>
    /// derzeitge healthpoints des enemy
    /// </summary>
    public int currentHealthEnemy;
    /// <summary>
    /// Unity Event für wenn der enemy geschlagen wird
    /// </summary>
    public UnityEvent<GameObject> OnHitWithReference;
    #endregion

    #region Attack Variables
    /// <summary>
    /// reichweite des schlages vom enemy
    /// </summary>
    [Header("Attack")]
    [SerializeField] float attackRange;

    [SerializeField] float heavyAttackRange;
    /// <summary>
    /// Rate in der, der enemy schlagen darf
    /// </summary>
    [SerializeField] float attackRate = 2;
    /// <summary>
    /// die zeit in welcher der enemy schlagen darf
    /// </summary>
    [SerializeField] float nextAttackTime = 0;
    /// <summary>
    /// Distanz in welcher der Spieler in Reichweite ist für einen Schlag
    /// </summary>
    [SerializeField] float playerInRangeDistance;
    /// <summary>
    /// Schnelligkeit des enemy
    /// </summary>
    [SerializeField] float speed;
    /// <summary>
    /// Schaden der normalen attacke des enemy
    /// </summary>
    [SerializeField] int attackDamage;

    [SerializeField] int heavyAttackDamage;
    /// <summary>
    /// bool ob der enemy grade eine Attacke ausführt
    /// </summary>
    private bool isAttacking;
    /// <summary>
    /// Transform des spielers
    /// </summary>
    [SerializeField] private Transform player;
    /// <summary>
    /// Position der attacke
    /// </summary>
    public Transform attackPoint;

    public Transform heavyAttackPoint;
    /// <summary>
    /// layer auf welcher der registriert wird
    /// </summary>
    public LayerMask attackMask;
    #endregion

    #region Detection Variables
    /// <summary>
    /// Position von der aus der Spieler detected wird
    /// </summary>
    [Header("Movement")]
    [SerializeField] private Transform detectorOrigin;
    /// <summary>
    /// sekunden die vergehen bis der spieler detected wird
    /// </summary>
    [SerializeField] float detectionDelay;
    /// <summary>
    /// Größe des detectors für die detection des spielers
    /// </summary>
    public Vector2 detectorSize = Vector2.one;
    /// <summary>
    /// Offset der detector position
    /// </summary>
    public Vector2 detectorOriginOffset = Vector2.zero;
    /// <summary>
    /// Die layer auf der die detection des spielers registriert wird
    /// </summary>
    public LayerMask detectorLayerMask;
    /// <summary>
    /// bool für ob der enemy geflipped ist
    /// </summary>
    public bool isFlipped = false;
    /// <summary>
    /// Objekt welches detected und verfolgt werden soll
    /// </summary>
    private GameObject target;
    /// <summary>
    /// bool ob das target Objekt detected wurde
    /// </summary>
    [field: SerializeField] public bool PlayerDetected { get; private set; }
    #endregion

    #region Patrol Variables
    /// <summary>
    /// Punkte welche der enemy nach einem attack abgeht
    /// </summary>
    public Transform[] patrolSpots;
    /// <summary>
    /// ein index für einen random spot von den patrol spots
    /// </summary>
    int randomSpot;
    /// <summary>
    /// bool für ob der enemy patrollieren soll oder nicht
    /// </summary>
    public bool canPatrol;
    /// <summary>
    /// zeit welche der enemy auf einem patrol spot wartet
    /// </summary>
    float waitTime;
    /// <summary>
    /// reset der zeit welche der Enemy auf einem spot wartet
    /// </summary>
    public float startWaitTime;
    /// <summary>
    /// zeit welche der enemy patrolliert
    /// </summary>
    [SerializeField] float patrolTime = 2f;
    #endregion

    #region ScriptAndScript Variables
    /// <summary>
    /// Script des players
    /// </summary>
    PlayerScript playerScript;
    /// <summary>
    /// rigidbody des enemy
    /// </summary>
    Rigidbody2D rb;
    /// <summary>
    /// animator des enemy
    /// </summary>
    Animator anim;
    ChapterChange chapterChange;
    #endregion

    #region Target Methods
    /// <summary>
    /// erfassung der position des spielers 
    /// </summary>
    public Vector2 DirectionToTarget => target.transform.position - detectorOrigin.position;

    /// <summary>
    /// Setzt target zu dem player detection objekt
    /// </summary>
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

    /// <summary>
    /// Hier setze ich das derzeitige leben des enemy mit dem maximalem leben gleich. Außerdem wird die detection des Spielers gestartet, der rigidbody und der animator
    /// mit get component und der Player mit findgameobjectwithtag gefunden. Dazu wird eine instanz für den enemy attack und ein randomSpot für die patrol spots erstellt
    /// </summary>
    private void Start()
    {
        currentHealthEnemy = maxHealthEnemy;
        StartCoroutine(DetectionCoroutine());
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        randomSpot = Random.Range(0, patrolSpots.Length);
        enemyAttack = AudioManager.instance.CreateInstance(FMODEvents.instance.enemyAttack);
    }

    /// <summary>
    /// Hier kann der enemy patrollieren, die richtung des players wird berechnet, die detection des Spieler wird durchgeführt und außerdem fragt ein if statement ab 
    /// ob der spieler in reichweite für einen angriff ist
    /// </summary>
    private void Update()
    {
        if (canPatrol)
        {
            transform.position = Vector2.MoveTowards(transform.position, patrolSpots[randomSpot].position, speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, patrolSpots[randomSpot].position) < Mathf.Epsilon)
            {
                if (Vector2.Distance(player.position, rb.position) <= playerInRangeDistance)
                {
                    if (randomSpot == patrolSpots.Length)
                        --randomSpot;
                    else
                        ++randomSpot;
                }
                if (waitTime <= 0)
                {
                    randomSpot = Random.Range(0, patrolSpots.Length);
                    startWaitTime = waitTime;
                }
                else
                {
                    waitTime -= Time.deltaTime;
                }
            }
        }

        LookAtPlayer();
        PerformDetection();

        if (Vector2.Distance(player.position, rb.position) <= playerInRangeDistance && !canPatrol)
        {
            if (!isAttacking && !canPatrol)
            {
                StartCoroutine(MovementLock());
            }
        }
    }

    #endregion

    #region Health Methods
    /// <summary>
    /// hier wird das derzeitige leben minus dem mitgegebenem damage int genommen. Außerdem wird ein GameObject als sender mitgegeben von dem aus der enemy knockback bekommt
    /// bei verfügbarem leben und einem hit des spieler wird die damaged animation abgespielt und ein invoke gesendet für knockback, bei fehlendem leben ist der enemy tot
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="sender"></param>
    public void TakeDamageEnemy(int damage, GameObject sender)
    {
        currentHealthEnemy -= damage;

        if (currentHealthEnemy > 0)
        {
            OnHitWithReference?.Invoke(sender);
            anim.SetTrigger("damaged");
        }

        if (currentHealthEnemy <= 0)
        {
            anim.SetTrigger("Death");
            chapterChange.LoadLevel("3.Chapter");
        }
    }
    #endregion 

    #region Player Detection Methods
    /// <summary>
    /// flipped den enemy abhängig von der position des spielers
    /// </summary>
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

    /// <summary>
    /// verfolgt den spieler wenn dieser in reichweite ist 
    /// </summary>
    public void PerformDetection()
    {
        if (!canPatrol && !isAttacking)
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
    }

    /// <summary>
    /// Führt die detection und methode immer wieder aus
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectionCoroutine()
    {
        yield return new WaitForSeconds(detectionDelay);
        PerformDetection();
        StartCoroutine(DetectionCoroutine());
    }

    /// <summary>
    /// lässt enemy patrollieren
    /// </summary>
    /// <returns></returns>
    public IEnumerator Patrol()
    {
        canPatrol = true;
        yield return new WaitForSeconds(patrolTime);
        canPatrol = false;
    }

    /// <summary>
    /// attack methode mit einem collider welcher dem spieler leben abzieht wenn in reichweite und setzt eine attaack time damit der spieler nicht dauerhaft attackieren kann
    /// </summary>
    #endregion

    #region Attack Methods
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
                        playerScript.TakeDamagePlayer(attackDamage);
                        nextAttackTime = Time.time + 1f / attackRate;
                    }
                }
            }
        }
    }

    public void HeavyAttack()
    {
        Collider2D colInfo = Physics2D.OverlapCircle(heavyAttackPoint.position, heavyAttackRange, attackMask);
        {
            if (colInfo != null)
            {
                if (playerScript = colInfo.GetComponent<PlayerScript>())
                {
                    if (Time.time >= nextAttackTime)
                    {

                        playerScript.TakeDamagePlayer(heavyAttackDamage);
                        nextAttackTime = Time.time + 1f / attackRate;
                    }
                }
            }
        }
    }

    /// <summary>
    /// lässt den spieler beim attackieren stehen bleiben und danach patrollieren
    /// </summary>
    /// <returns></returns>
    IEnumerator MovementLock()
    {
        if (Random.Range(0, 2) == 1)
        {
            enemyAttack.start();
            anim.SetTrigger("EnemyAttack");
        }
        else if (Random.Range(0, 2) == 0)
        {
            enemyAttack.start();
            anim.SetTrigger("EnemyAttack");
        }
        else
            anim.SetTrigger("heavyAttack");

        isAttacking = true;
        yield return new WaitForSeconds(1f);
        StartCoroutine(Patrol());
        isAttacking = false;
    }
    #endregion

    #region OnDrawGizmo Method
    /// <summary>
    /// zeichnet gizmos
    /// </summary>
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
    #endregion
}
