using UnityEngine;
using UnityEngine.UI;

public class CardHighlight : MonoBehaviour
{
    public Camera playerCamera;
    public float rayDistance = 5f;
    public LayerMask cardLayer;
    public GameObject pressEText; // UI Text for "Press E to grab"
    public Image cardIcon; // UI icon that appears when the card is grabbed
    public Sprite cardSprite; // The card icon sprite

    private GameObject currentCard;
    private Material originalMaterial;
    public Material highlightMaterial;

    private bool hasCard = false;

    void Start()
    {
        pressEText.SetActive(false); // Hide "Press E" prompt initially
        cardIcon.enabled = false;    // Hide card icon initially
    }

    void Update()
    {
        RaycastForCard();
        HandleCardGrab();
    }

    void RaycastForCard()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance, cardLayer))
        {
            GameObject hitCard = hit.collider.gameObject;

            // If the player is looking at a new card, highlight it
            if (hitCard != currentCard && !hasCard)
            {
                RemoveHighlight();
                currentCard = hitCard;
                HighlightCard(currentCard);
                ShowPressEText(true); // Show "Press E to grab" text
            }
        }
        else
        {
            RemoveHighlight();
            ShowPressEText(false); // Hide "Press E to grab" text
        }
    }

    void HandleCardGrab()
    {
        // If player presses E and there's a card highlighted, grab it
        if (Input.GetKeyDown(KeyCode.E) && currentCard != null && !hasCard)
        {
            GrabCard();
        }
    }

    void HighlightCard(GameObject card)
    {
        Renderer renderer = card.GetComponent<Renderer>();
        if (renderer != null)
        {
            originalMaterial = renderer.material;
            renderer.material = highlightMaterial;
        }
    }

    void RemoveHighlight()
    {
        if (currentCard != null)
        {
            Renderer renderer = currentCard.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = originalMaterial; // Restore original material
            }
            currentCard = null;
        }
    }

    void ShowPressEText(bool show)
    {
        pressEText.SetActive(show);
    }

    void GrabCard()
    {
        // Make the card disappear
        currentCard.SetActive(false);

        // Set the player to have the card
        hasCard = true;

        // Show card icon on UI
        cardIcon.enabled = true;
        cardIcon.sprite = cardSprite;

        // Hide "Press E to grab" text
        ShowPressEText(false);
    }
}
