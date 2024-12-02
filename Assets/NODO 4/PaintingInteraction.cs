using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class PaintingInteraction : MonoBehaviour
{
    public GameObject popupPanel; // Reference to the popup UI Panel
    public Image paintingImage; // The UI Image to display the painting
    public Sprite paintingSprite; // The Sprite for this painting
    public TMP_Text interactionText; // The "Press E to open" text

    // Reference to the player's GameObject instead of specific script
    private GameObject player;
    private MonoBehaviour playerMovementScript;
    private bool isPlayerNearby = false;
    public Button closeButton;
    private Rigidbody2D playerRb;

    private void Start()
    {
        interactionText.enabled = false; // Hide the interaction text initially

        // Find the player
        player = GameObject.FindGameObjectWithTag("Player");

        // Add click listener to the close button
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePopup);
        }

        // Ensure popup is closed at start
        popupPanel.SetActive(false);
    }

    private void Update()
    {
        // Only check for E key when popup is not active
        if (isPlayerNearby && !popupPanel.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            OpenPopup();
        }

        // Add escape key as alternative way to close
        if (popupPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePopup();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            interactionText.enabled = true; // Show the "Press E" text
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            interactionText.enabled = false; // Hide the "Press E" text
        }
    }

    private void OpenPopup()
    {
        popupPanel.SetActive(true);
        paintingImage.sprite = paintingSprite;

        if (player != null)
        {
            // Stop any current movement
            if (playerRb != null)
            {
                playerRb.velocity = Vector2.zero;
            }

            // Disable movement scripts
            MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                if (script != null &&
                    !(script is PaintingInteraction) &&
                    !(script is AudioSource))
                {
                    script.enabled = false;
                }
            }
        }

        interactionText.enabled = false;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ClosePopup()
    {
        popupPanel.SetActive(false);

        if (player != null)
        {
            // Re-enable all scripts
            MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                if (script != null)
                {
                    script.enabled = true;
                }
            }
        }

        EventSystem.current.SetSelectedGameObject(null);
    }
}