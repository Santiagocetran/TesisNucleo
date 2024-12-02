using UnityEngine;
using UnityEngine.UI;

public class CardHighlight : MonoBehaviour
{
    public Camera playerCamera;
    public float rayDistance = 1f;
    public LayerMask cardLayer;
    public GameObject pressEText;       // UI Text for "Press E to grab"
    public Image cardIcon;              // UI icon that appears when the card is grabbed
    public Sprite cardSprite;           // The card icon sprite

    private GameObject currentCard;
    private Material originalMaterial;
    public Material highlightMaterial;

    public bool hasCard = false;

    // New UI elements for the popup
    public GameObject cardPopup;        // Popup panel for enlarged card view
    public Image cardPopupImage;        // Image component to show the enlarged card
    public Button closeButton;          // Button to close the popup

    private bool isPopupActive = false; // Flag to track if popup is active

    // Player movement and look scripts
    public FirstPersonMovement playerMovement;
    public FirstPersonLook playerLook;

    void Start()
    {
        pressEText.SetActive(false);       // Hide "Press E" prompt initially
        cardIcon.enabled = false;          // Hide card icon initially
        cardPopup.SetActive(false);        // Hide card popup initially

        closeButton.onClick.AddListener(ClosePopup);  // Add listener for the Close button
    }

    void Update()
    {
        if (!isPopupActive)
        {
            RaycastForCard();
            HandleCardGrab();
        }
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

        // Show popup with enlarged card details
        ShowCardPopup();
    }

    void ShowCardPopup()
    {
        cardPopupImage.sprite = cardSprite;   // Set the popup image to the card's sprite
        cardPopup.SetActive(true);            // Show the popup
        isPopupActive = true;                 // Set popup active flag to true

        // Lock player movement and camera
        LockPlayerControls(true);

        // Show and unlock the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ClosePopup()
    {
        cardPopup.SetActive(false);           // Hide the popup
        isPopupActive = false;                // Set popup active flag to false

        // Unlock player movement and camera
        LockPlayerControls(false);

        // Hide and lock the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerMovement.enabled = true;
        playerLook.enabled = true;
    }

    void LockPlayerControls(bool lockControls)
    {
        playerMovement.enabled = false;
        playerLook.enabled = false;
    }
}
