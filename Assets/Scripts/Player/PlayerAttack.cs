using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float normalAttackCooldown;
    [SerializeField] private int normalAttackDamage;
    [SerializeField] private float fireballCooldown;
    [SerializeField] private Transform firepoint;
    [SerializeField] private GameObject[] fireballs;

    [Header("Collider Parameters")]
    [SerializeField] private float range;
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Enemy Layer")]
    [SerializeField] private LayerMask enemyLayer;

    private Animator animator;
    private Health enemyHealth;
    private PlayerMovement playerMovement;
    private float cooldownTimer = Mathf.Infinity;
    private float horizontalInput;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        if (playerMovement.canAttack())
        {

            if (Input.GetMouseButtonDown(0) && cooldownTimer > normalAttackCooldown) {
                NormalAttack();
            }

            if (Input.GetMouseButtonDown(1) && cooldownTimer > fireballCooldown && fireballs.Length > 0 && horizontalInput == 0)
            {
                Fireball();
            }
        }
        
        cooldownTimer += Time.deltaTime;
    }

    private void NormalAttack()
    {
        cooldownTimer = 0;
        animator.SetTrigger("attack");
    }

    private bool EnemyInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, enemyLayer);

        if (hit.collider != null)
        {
            enemyHealth = hit.transform.GetComponent<Health>();
        }

        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }

    private void Fireball()
    {
        animator.SetTrigger("fireball");
        cooldownTimer = 0;

        fireballs[FindFireball()].transform.position = firepoint.position;
        fireballs[FindFireball()].GetComponent<Fireball>().SetDirection(Mathf.Sign(transform.localScale.x));
    }

    private int FindFireball()
    {
        for (int i = 0; i < fireballs.Length; i++)
        {
            if (!fireballs[i].activeInHierarchy)
            {
                return i;
            }
        }
        return 0;
    }

    private void DamageEnemy()
    {
        if (EnemyInSight())
        {
            enemyHealth.TakeDamage(normalAttackDamage);
        }
    }
}
