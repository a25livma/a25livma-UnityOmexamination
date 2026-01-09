using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints; // set patrol points here, this is just a list of patrol points
    public float moveSpeed;
    public float waitTime; // how much time to wait at each point

    [Header("Platformer settings")] 
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;
    public bool flipSprite = true; // if we should flip the sprite or not
    
    [Header("Components")]
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Collider2D enemyCollider;

    private int currentPointIndex = 0;
    private float waitCounter = 0f;
    private bool isWaiting = false;
    private Vector2 currentVelocity;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // get components
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<Collider2D>();

        // start at first patrol point
        if (patrolPoints.Length > 0)
        {
            transform.position = patrolPoints[0].position;
        }

        // make sure we have rigidbody2D
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 3f; // gravity strength for platformer
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (patrolPoints.Length == 0)
        {
            return;
        }

        if (isWaiting)
        {
            waitCounter -= Time.deltaTime;
            if (waitCounter <= 0)
            {
                isWaiting = false;
            }
            return;
        }
        
        // get direction to the target (aka next patrol point)
        Transform targetPoint = patrolPoints[currentPointIndex];
        Vector2 direction = (targetPoint.position - transform.position).normalized;

        // flip sprite if needed
        if (flipSprite && spriteRenderer != null)
        {
            if (direction.x > 0.1f)
            {
                spriteRenderer.flipX = false;
            }
            else if (direction.x < -0.1f)
            {
                spriteRenderer.flipX = true;
            }
        }

        // check if we are on ground before moving
        if (IsGrounded())
        {
            // move using rigidbody2D
            currentVelocity = rb.linearVelocity;
            currentVelocity.x = direction.x * moveSpeed;
            rb.linearVelocity = currentVelocity;
        }
        else
        {
            // stop moving if not grounded
            currentVelocity = rb.linearVelocity;
            currentVelocity.x = 0;
            rb.linearVelocity = currentVelocity;
        }
        
        // check if we reached the patrol point (horizontal distance only)
        float horizontalDistance = Mathf.Abs(transform.position.x - targetPoint.position.x);
        if (horizontalDistance < 0.2f && IsGrounded())
        {
            StartWaiting();
        }
    }

    public bool IsGrounded()
    {
        if (enemyCollider == null)
        {
            return false;
        }
        
        // catch rays from the bottom of the collider
        Vector2 colliderBottom = new Vector2(enemyCollider.bounds.center.x, enemyCollider.bounds.min.y);
        RaycastHit2D hit = Physics2D.Raycast(colliderBottom, Vector2.down, groundCheckDistance, groundLayer);
        
        return hit.collider != null;
    }

    public void StartWaiting()
    {
        isWaiting = true;
        waitCounter = waitTime;
        
        // stop movement
        currentVelocity = rb.linearVelocity;
        currentVelocity.x = 0;
        rb.linearVelocity = currentVelocity;

        // move to the next patrol point
        currentPointIndex++;
        if (currentPointIndex >= patrolPoints.Length)
        {
            currentPointIndex = 0; // loop back to start
        }
    }
}

