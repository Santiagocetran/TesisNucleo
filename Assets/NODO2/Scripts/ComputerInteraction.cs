using UnityEngine;
using UnityEngine.UI;

public class ComputerInteraction : MonoBehaviour
{
    public Camera playerCamera;
    public float rayDistance = 5f;
    public LayerMask computerLayer;

    public GameObject pressEText; // UI Text for "Press E to interact"
    public GameObject popupMessage; // Popup panel with a close button
    public Button closeButton; // Button to close the popup

    private GameObject currentComputer;

    // Reference to the CardHighlight script to check if the player has the card
    public CardHighlight cardHighlight;

    // References for controlling player movement and camera
    public FirstPersonMovement playerMovement;
    public FirstPersonLook playerLook;

    void Start()
    {
        pressEText.SetActive(false); // Hide "Press E" prompt initially
        popupMessage.SetActive(false); // Hide popup message initially

        // Set up the button click listener
        closeButton.onClick.AddListener(ClosePopup);

        // Ensure cursor is locked initially for FPS gameplay
        LockCursor();
    }

    void Update()
    {
        RaycastForComputer();
        HandleComputerInteraction();
    }

    void RaycastForComputer()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance, computerLayer))
        {
            GameObject hitComputer = hit.collider.gameObject;

            // If the player is looking at the computer
            if (hitComputer != currentComputer)
            {
                currentComputer = hitComputer;
                ShowPressEText(true); // Show "Press E to interact" text when looking at computer
            }
        }
        else
        {
            ShowPressEText(false); // Hide "Press E to interact" text when not looking at computer
            currentComputer = null;
        }
    }

    void HandleComputerInteraction()
    {
        // If player presses E while looking at the computer
        if (Input.GetKeyDown(KeyCode.E) && currentComputer != null)
        {
            if (cardHighlight.hasCard) // Check if the player has the card
            {
                InteractWithComputer();
            }
            else
            {
                ShowCluePopup();
            }
        }
    }

    void ShowPressEText(bool show)
    {
        pressEText.SetActive(show);
    }

    void InteractWithComputer()
    {
        // Code to interact with the computer if the player has the card
        Debug.Log("Player is interacting with the computer.");
        // Add logic for interacting with the computer here (e.g., open terminal UI)
    }

    void ShowCluePopup()
    {
        // Show the popup with the clue
        popupMessage.SetActive(true);

        // Lock player movement and camera
        LockPlayerControls();

        // Unlock the cursor so the player can click the button
        UnlockCursor();
    }

    void ClosePopup()
    {
        // Hide the popup message when the button is clicked
        popupMessage.SetActive(false);

        // Unlock player movement and camera
        UnlockPlayerControls();

        // Lock the cursor back to the center for FPS controls
        LockCursor();
    }

    void LockPlayerControls()
    {
        // Disable movement and camera look scripts
        playerMovement.enabled = false;
        playerLook.enabled = false;
    }

    void UnlockPlayerControls()
    {
        // Re-enable movement and camera look scripts
        playerMovement.enabled = true;
        playerLook.enabled = true;
    }

    // Unlock the cursor so the player can click the UI
    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Make cursor visible
    }

    // Lock the cursor for FPS gameplay
    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center
        Cursor.visible = false; // Hide the cursor
    }
}
