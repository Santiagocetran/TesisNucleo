using UnityEngine;

public class Character2DController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Sprite frontSprite;
    public Sprite backSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;
    public Sprite[] walkingSprites; // Array de sprites para la animación de caminar

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 movement;
    private float animationTime = 0f;
    private int currentSpriteIndex = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Captura el input del jugador
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Normaliza el vector de movimiento para evitar movimiento más rápido en diagonal
        movement.Normalize();

        // Actualiza el sprite basado en la dirección del movimiento
        UpdateSprite();
    }

    void FixedUpdate()
    {
        // Mueve al personaje
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void UpdateSprite()
    {
        if (movement.x > 0)
        {
            spriteRenderer.sprite = rightSprite;
        }
        else if (movement.x < 0)
        {
            spriteRenderer.sprite = leftSprite;
        }
        else if (movement.y > 0)
        {
            spriteRenderer.sprite = backSprite;
        }
        else if (movement.y < 0)
        {
            spriteRenderer.sprite = frontSprite;
        }

        // Si el personaje se está moviendo, anima los sprites de caminar
        if (movement != Vector2.zero)
        {
            animationTime += Time.deltaTime;
            if (animationTime >= 0.1f) // Cambia el sprite cada 0.1 segundos
            {
                animationTime = 0f;
                currentSpriteIndex = (currentSpriteIndex + 1) % walkingSprites.Length;
                spriteRenderer.sprite = walkingSprites[currentSpriteIndex];
            }
        }
    }
}
