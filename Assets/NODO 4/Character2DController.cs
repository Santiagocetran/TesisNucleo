using UnityEngine;

public class Character2DController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float jumpInterval = 2f; // Time between allowed jumps in seconds

    public Sprite characterRight;
    public Sprite characterWalkRight;
    public Sprite characterLeft;
    public Sprite characterLeftWalk;
    public Sprite frontSprite;
    public Sprite idleSprite;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 movement;
    private float animationTime = 0f;

    private float lastJumpTime = -2f; // Track the last jump time

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        gameObject.tag = "Player"; // Tag this GameObject as "Player"
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = 0;

        // Check for jump input and ensure enough time has passed since the last jump
        if (Input.GetKeyDown(KeyCode.W) && Time.time - lastJumpTime >= jumpInterval)
        {
            Jump();
        }

        UpdateSprite();
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(movement.x * moveSpeed, rb.velocity.y);
    }

    void UpdateSprite()
    {
        if (movement.x > 0)
        {
            AnimateRightWalk();
        }
        else if (movement.x < 0)
        {
            AnimateLeftWalk();
        }
        else
        {
            spriteRenderer.sprite = idleSprite;
        }
    }

    void AnimateRightWalk()
    {
        animationTime += Time.deltaTime;
        if (animationTime >= 0.2f)
        {
            animationTime = 0f;
            spriteRenderer.sprite = (spriteRenderer.sprite == characterRight) ? characterWalkRight : characterRight;
        }
    }

    void AnimateLeftWalk()
    {
        animationTime += Time.deltaTime;
        if (animationTime >= 0.2f)
        {
            animationTime = 0f;
            spriteRenderer.sprite = (spriteRenderer.sprite == characterLeft) ? characterLeftWalk : characterLeft;
        }
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        lastJumpTime = Time.time; // Update the last jump time
    }
}
