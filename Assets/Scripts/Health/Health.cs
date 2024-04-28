using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float startingHealth;
    public float currentHealth { get; private set; }
    private Animator animatior;
    private bool dead;

    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOffFlashes;
    private SpriteRenderer spriteRenderer;


    private void Awake()
    {
        currentHealth = startingHealth;
        animatior = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);
        
        if (currentHealth > 0)
        {
            animatior.SetTrigger("hurt");
            StartCoroutine(Invulnerability());
        } else
        {
            if(!dead) {
                animatior.SetTrigger("die");

                //Player
                if (GetComponent<PlayerMovement>() != null)
                {
                    GetComponent<PlayerMovement>().enabled = false;
                }
                

                //Enemies
                if(GetComponentInParent<EnemyPatrol>() != null)
                {
                    GetComponentInParent<EnemyPatrol>().enabled = false;
                    
                }

                if (GetComponent<ClubEnemy>() != null)
                {
                    GetComponent<ClubEnemy>().enabled = false;
                }

                if(GetComponent<SpearEnemy>() != null)
                {
                    GetComponent<SpearEnemy>().enabled = false;
                }

                dead = true;
            }
            
        }
    }

    private IEnumerator Invulnerability()
    {
        Physics2D.IgnoreLayerCollision(10, 11, true);
        for (int i = 0; i < numberOffFlashes; i++)
        {
            spriteRenderer.color = new Color(0.75f, 0.75f, 0.75f, 0.5f);
            yield return new WaitForSeconds(iFramesDuration /  (numberOffFlashes));
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOffFlashes));
        }
        Physics2D.IgnoreLayerCollision(10, 11, false);
    }
}
