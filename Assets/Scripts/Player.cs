using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 4f;
    public float tileSize = 1f;

    [Header("Layers")]
    public LayerMask collisionLayer;

    [Header("Input")]
    public KeyCode interactKey = KeyCode.Z;
   [SerializeField] private float interactRange;

    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 moveDirection;
    private Vector2 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();

        // Default facing direction is down like Pokemon
        moveDirection = Vector2.down;
        animator.SetFloat("moveX", 0f);
        animator.SetFloat("moveY", -1f);

        targetPosition = SnapToGrid(transform.position);
        rb.position = targetPosition;
    }

    void Update()
    {
        if (!isMoving)
        {
            HandleInteractInput();
            ReadInput();
        }

        UpdateAnimator();
    }

    void FixedUpdate()
    {
        if (isMoving)
            MoveToTarget();
    }

    // ─────────────────────────────────────────
    //  INTERACTION
    // ─────────────────────────────────────────
   void HandleInteractInput()
{
    // Prevent interaction while dialogue is already open
    if (DialogueManager.Instance.IsActive)
        return;

    if (!Input.GetKeyDown(interactKey))
        return;

    // Check one tile ahead
    Vector2 interactPos = rb.position + moveDirection * tileSize;

    Collider2D hit = Physics2D.OverlapCircle(interactPos, interactRange, collisionLayer);

    if (hit != null)
    {
        IInteractable interactable = hit.GetComponent<IInteractable>();

        if (interactable != null)
            interactable.Interact();
    }
}

    // ─────────────────────────────────────────
    //  INPUT
    // ─────────────────────────────────────────
    void ReadInput()
    {
        // Block movement while dialogue is open
        if (DialogueManager.Instance.IsActive) return;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        // No diagonals - Pokemon style
        if (x != 0) y = 0;

        if (x != 0 || y != 0)
        {
            Vector2 desiredDir = new Vector2(x, y).normalized;
            Vector2 desiredTarget = rb.position + desiredDir * tileSize;

            moveDirection = desiredDir;

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

        // // Only update direction while moving so idle holds last direction
        // if (isMoving)
        // {
        //     animator.SetFloat("moveX", moveDirection.x, 0.1f, Time.deltaTime);
        //     animator.SetFloat("moveY", moveDirection.y, 0.1f, Time.deltaTime);
        // }

        // Alwas update the direction of the player to face the last input direction.
        animator.SetFloat("moveX", moveDirection.x, 0.1f, Time.deltaTime);
        animator.SetFloat("moveY", moveDirection.y, 0.1f, Time.deltaTime);  
    }

    // ─────────────────────────────────────────
    //  HELPERS
    // ─────────────────────────────────────────
    bool IsBlocked(Vector2 target)
    {
        return Physics2D.OverlapCircle(target, 0.1f, collisionLayer) != null;
    }

    Vector2 SnapToGrid(Vector2 pos)
    {
        return new Vector2(
            Mathf.Round(pos.x / tileSize) * tileSize,
            Mathf.Round(pos.y / tileSize) * tileSize
        );
    }

    // ─────────────────────────────────────────
    //  GIZMOS
    // ─────────────────────────────────────────
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + (Vector3)(moveDirection * tileSize), interactRange);
    }
}