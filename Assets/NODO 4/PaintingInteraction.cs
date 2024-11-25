using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PaintingInteraction : MonoBehaviour
{
    public GameObject popupPanel; // Reference to the popup UI Panel
    public Image paintingImage; // The UI Image to display the painting
    public Sprite paintingSprite; // The Sprite for this painting
    public TMP_Text interactionText; // The "Press E to open" text
    private bool isPlayerNearby = false;

    private void Start()
    {
        interactionText.enabled = false; // Hide the interaction text initially
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            OpenPopup();
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
        popupPanel.SetActive(true); // Show the popup
        paintingImage.sprite = paintingSprite; // Set the painting sprite
        Time.timeScale = 0f; // Freeze the game
        interactionText.enabled = false; // Hide the interaction text
    }

    public void ClosePopup()
    {
        popupPanel.SetActive(false); // Hide the popup
        Time.timeScale = 1f; // Resume the game
    }
}
