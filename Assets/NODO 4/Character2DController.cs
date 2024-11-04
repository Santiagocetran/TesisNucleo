using UnityEngine;

public class Character2DController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f; // Force applied when jumping
    public LayerMask groundLayer; // Layer mask for ground detection

    // Sprites for standing and walking in different directions
    public Sprite characterRight;
    public Sprite characterWalkRight;
    public Sprite characterLeft;
    public Sprite characterLeftWalk;
    public Sprite frontSprite; // For when facing down
    public Sprite idleSprite; // For idle state

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 movement;
    private float animationTime = 0f;
    private bool isGrounded = true; // Check if the character is on the ground

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Freeze rotation on the Z-axis to prevent tipping over
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        // Capture horizontal input only
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = 0; // Ensure no vertical movement for walking

        // Check for jump input
        if (Input.GetKeyDown(KeyCode.W) && IsGrounded())
        {
            Jump();
        }

        // Update the sprite based on movement
        UpdateSprite();
    }

    void FixedUpdate()
    {
        // Move the character
        rb.velocity = new Vector2(movement.x * moveSpeed, rb.velocity.y);
    }

    void UpdateSprite()
    {
        if (movement.x > 0) // Moving right
        {
            AnimateRightWalk();
        }
        else if (movement.x < 0) // Moving left
        {
            AnimateLeftWalk();
        }
        else
        {
            // Reset to idle when not moving
            spriteRenderer.sprite = idleSprite;
        }
    }

    void AnimateRightWalk()
    {
        // Alternate between standing and walking right sprites
        animationTime += Time.deltaTime;
        if (animationTime >= 0.2f) // Adjust this value for animation speed
        {
            animationTime = 0f;
            spriteRenderer.sprite = (spriteRenderer.sprite == characterRight) ? characterWalkRight : characterRight;
        }
    }

    void AnimateLeftWalk()
    {
        // Alternate between standing and walking left sprites
        animationTime += Time.deltaTime;
        if (animationTime >= 0.2f) // Adjust this value for animation speed
        {
            animationTime = 0f;
            spriteRenderer.sprite = (spriteRenderer.sprite == characterLeft) ? characterLeftWalk : characterLeft;
        }
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    bool IsGrounded()
    {
        // Check if the character is grounded using a raycast
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, groundLayer);
        return hit.collider != null;
    }
}
