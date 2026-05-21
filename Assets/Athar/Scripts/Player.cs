using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 4f;          // tiles per second
    public float tileSize = 1f;           // world units per tile

    [Header("Layer Collision")]
    public LayerMask collisionLayer;      // assign "Obstacle" layer in Inspector

    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 moveDirection;        // current facing / move dir
    private Vector2 targetPosition;      // tile we are moving toward
    private bool isMoving = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        // Default facing direction is down
        moveDirection = Vector2.down;
        animator.SetFloat("moveX", 0f);
        animator.SetFloat("moveY", -1f);

        // Snap player to nearest tile on start
        targetPosition = SnapToGrid(transform.position);
        rb.position = targetPosition;
    }

    void Update()
    {
        if (!isMoving)
            ReadInput();

        UpdateAnimator();
    }

    void FixedUpdate()
    {
        if (isMoving)
            MoveToTarget();
    }

    // ─────────────────────────────────────────
    //  INPUT
    // ─────────────────────────────────────────
    void ReadInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        // No diagonal movement (Pokemon style)
        if (x != 0) y = 0;

        if (x != 0 || y != 0)
        {
            Vector2 desiredDir = new Vector2(x, y).normalized;
            Vector2 desiredTarget = rb.position + desiredDir * tileSize;

            // Always update facing direction
            moveDirection = desiredDir;

            // Only start moving if tile is walkable
            if (!IsBlocked(desiredTarget))
            {
                targetPosition = desiredTarget;
                isMoving = true;
            }
        }
    }

    // ─────────────────────────────────────────
    //  MOVEMENT
    // ─────────────────────────────────────────
    void MoveToTarget()
    {
        Vector2 newPos = Vector2.MoveTowards(
            rb.position,
            targetPosition,
            moveSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPos);

        // Snap and stop when tile is reached
        if (Vector2.Distance(rb.position, targetPosition) < 0.001f)
        {
            rb.position = targetPosition;
            isMoving = false;
        }
    }

    // ─────────────────────────────────────────
    //  ANIMATOR
    // ─────────────────────────────────────────
    void UpdateAnimator()
    {
        animator.SetBool("isMoving", isMoving);

        // Only update blend direction when actually moving
        // so idle frame matches last walk direction (like Pokemon)
        if (isMoving)
        {
            animator.SetFloat("moveX", moveDirection.x, 0.2f, Time.deltaTime);
            animator.SetFloat("moveY", moveDirection.y, 0.2f, Time.deltaTime);
        }
    }

    // ─────────────────────────────────────────
    //  HELPERS
    // ─────────────────────────────────────────

    // Check if target tile has a collider on the collision layer
    bool IsBlocked(Vector2 target)
    {
        Collider2D hit = Physics2D.OverlapCircle(target, 0.1f, collisionLayer);
        return hit != null;
    }

    // Snap any position to the nearest tile center
    Vector2 SnapToGrid(Vector2 pos)
    {
        return new Vector2(
            Mathf.Round(pos.x / tileSize) * tileSize,
            Mathf.Round(pos.y / tileSize) * tileSize
        );
    }
}