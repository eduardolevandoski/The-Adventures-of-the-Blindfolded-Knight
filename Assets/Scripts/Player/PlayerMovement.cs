using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    // [SerializeField] used to  show the value on unity
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    // Unity variables
    private BoxCollider2D boxCollider;
    private Rigidbody2D body;
    private Animator animator;
    private float horizontalInput;

    // Cooldowns
    private float wallJumpCooldown;

    // Variables
    const int size = 6;

    private void Awake()
    {
        // Get references from the unity object
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        // Flip the player
        if (horizontalInput > 0.01f)
        {
            transform.localScale = new Vector3(size, size, size);
        }
        else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-size, size, size);
        }

        // Check cooldowns
        if (wallJumpCooldown > 0.2f)
        {
            // Move the player to left and right
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);


            // Wall jump gravity
            if (onWall() && !isGrounded())
            {
                body.gravityScale = 0;
                body.velocity = Vector2.zero;
            }
            else
            {
                body.gravityScale = 1.5f;
            }

            // Jump 
            if (Input.GetKey(KeyCode.Space))
            {
                Jump();
            }
        }
        else
        {
            wallJumpCooldown += Time.deltaTime;
        }

        // Set animation parameters
        animator.SetBool("run", horizontalInput != 0);
        animator.SetBool("grounded", isGrounded());
    }

    private void Jump()
    {
        if (isGrounded())
        {
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            animator.SetTrigger("jump");
        }
        else if (onWall() && !isGrounded())
        {
            body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 7);
            wallJumpCooldown = 0;
        }
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack(bool canRun = false)
    {
        return isGrounded() && !onWall();
    }
}
