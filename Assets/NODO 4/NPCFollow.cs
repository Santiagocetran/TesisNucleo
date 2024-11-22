using UnityEngine;

public class NPCFollow : MonoBehaviour
{
    public Transform leader; // The NPC or player to follow
    public Transform player; // Reference to the player
    public float followDistance = 1f; // Distance to maintain behind the leader
    public float interactionDistance = 2f; // Distance for the player to trigger interaction
    public float speed = 3f; // Speed of movement

    public Sprite npcIdleRight; // Idle sprite (right-facing)
    public Sprite npcStepRight; // Walking step sprite (right-facing)
    public Sprite npcStoppedSprite; // Idle/stopped sprite
    public float animationInterval = 0.2f; // Time between sprite changes

    public GameObject textBubble; // Text bubble for interaction

    private bool isFollowing = false; // Whether this NPC is following the leader
    private SpriteRenderer spriteRenderer; // Reference to the NPC's sprite renderer
    private float animationTimer = 0f; // Timer for sprite animation
    private bool usingIdleSprite = true; // Whether the current sprite is the idle frame
    private Vector2 lastPosition; // To track NPC movement for idle detection

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastPosition = transform.position;
        textBubble.SetActive(false); // Hide the text bubble initially
    }

    void Update()
    {
        if (!isFollowing)
        {
            // Show the popup if the player is near this NPC
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer < interactionDistance)
            {
                textBubble.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    StartFollowing();
                }
            }
            else
            {
                textBubble.SetActive(false);
            }
        }
        else
        {
            textBubble.SetActive(false); // Hide the popup when following

            FollowLeader();
            SyncWithPlayerInput(); // Synchronize NPC animations with player's movement
        }
    }

    private void StartFollowing()
    {
        isFollowing = true;

        // Find the last NPC in the chain or set the leader to the player if no chain exists
        NPCFollow[] allNPCs = FindObjectsOfType<NPCFollow>();
        Transform lastInLine = player; // Default to the player

        foreach (NPCFollow npc in allNPCs)
        {
            if (npc.isFollowing && npc != this)
            {
                lastInLine = npc.transform; // Set to the last following NPC
            }
        }

        leader = lastInLine; // Set the leader to the last NPC in the chain
        Debug.Log($"{gameObject.name} is now following {leader.name}!");
    }

    private void FollowLeader()
    {
        if (leader == null) return;

        // Calculate the target position, preserving the NPC's current Y-coordinate
        Vector2 leaderPosition = leader.position;
        Vector2 targetPosition = new Vector2(leaderPosition.x - followDistance, transform.position.y);

        // Only move if the NPC is not within the desired horizontal range
        if (Mathf.Abs(transform.position.x - targetPosition.x) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
    }

    private void SyncWithPlayerInput()
    {
        if (Input.GetKey(KeyCode.D)) // Player walking right
        {
            AnimateRightWalk();
        }
        else if (Input.GetKey(KeyCode.A)) // Player walking left
        {
            AnimateLeftWalk();
        }
        else
        {
            // Idle if no horizontal movement
            spriteRenderer.sprite = npcStoppedSprite;
        }
    }

    private void AnimateRightWalk()
    {
        animationTimer += Time.deltaTime;
        if (animationTimer >= animationInterval)
        {
            animationTimer = 0f; // Reset the timer
            usingIdleSprite = !usingIdleSprite; // Toggle between walking sprites
            spriteRenderer.sprite = usingIdleSprite ? npcIdleRight : npcStepRight;
        }

        // Ensure the sprite is not flipped
        spriteRenderer.flipX = false;
    }

    private void AnimateLeftWalk()
    {
        animationTimer += Time.deltaTime;
        if (animationTimer >= animationInterval)
        {
            animationTimer = 0f; // Reset the timer
            usingIdleSprite = !usingIdleSprite; // Toggle between walking sprites
            spriteRenderer.sprite = usingIdleSprite ? npcIdleRight : npcStepRight;
        }

        // Flip the sprite for left direction
        spriteRenderer.flipX = true;
    }
}
