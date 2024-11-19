using UnityEngine;

public class NPCFollow : MonoBehaviour
{
    public Transform player; // Reference to the player
    public float followDistance = 0.5f; // Smaller distance to stay close
    public float speed = 3f; // Speed of NPC movement

    public Sprite npcIdleRight; // Sprite for walking idle frame
    public Sprite npcStepRight; // Sprite for walking step frame
    public Sprite npcStoppedSprite; // Sprite for when the NPC stops moving
    public float animationInterval = 0.2f; // Time between sprite changes

    private bool isFollowing = false; // Whether the NPC is following the player
    private Camera mainCamera; // Reference to the main camera for bounds checking
    private SpriteRenderer spriteRenderer; // Reference to the NPC's SpriteRenderer
    private float animationTimer = 0f; // Timer for animating the walk cycle
    private bool usingIdleSprite = true; // Track the current sprite state
    private Vector2 lastPosition; // Track the NPC's last position

    void Start()
    {
        mainCamera = Camera.main; // Get the main camera
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer
        lastPosition = transform.position; // Initialize last position
    }

    void Update()
    {
        // Log positions for debugging
        Debug.Log($"Player Position: {player.position}, NPC Position: {transform.position}, Is Following: {isFollowing}");

        if (!isFollowing)
        {
            // Check if the player is within interaction range
            if (Vector2.Distance(transform.position, player.position) < 2f)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    StartFollowing();
                }
            }
        }

        if (isFollowing)
        {
            FollowPlayer();
            AnimateWalking(); // Handle sprite animation
        }
        else
        {
            // If not following, ensure the stopped sprite is set
            spriteRenderer.sprite = npcStoppedSprite;
        }
    }

    private void StartFollowing()
    {
        isFollowing = true;
        Debug.Log($"NPC started following at position: {transform.position}");
    }

    private void FollowPlayer()
    {
        // Define the target position closer to the player
        Vector2 targetPosition = player.position;

        // Adjust to maintain a smaller follow distance
        if (targetPosition.x > transform.position.x)
        {
            targetPosition.x -= followDistance; // Stay slightly behind the player
        }

        // Move the NPC smoothly towards the target position
        Vector2 newPosition = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        transform.position = newPosition;
    }

    private void AnimateWalking()
    {
        // Check if the NPC is moving
        Vector2 currentPosition = transform.position;
        if (currentPosition == lastPosition)
        {
            // NPC is not moving; set the stopped sprite
            spriteRenderer.sprite = npcStoppedSprite;
            return;
        }

        // NPC is moving; update the animation timer
        animationTimer += Time.deltaTime;

        if (animationTimer >= animationInterval)
        {
            animationTimer = 0f; // Reset the timer
            usingIdleSprite = !usingIdleSprite; // Toggle sprite state

            // Set the appropriate walking sprite
            spriteRenderer.sprite = usingIdleSprite ? npcIdleRight : npcStepRight;
        }

        // Update last position
        lastPosition = currentPosition;
    }
}
