using UnityEngine;
using UnityEngine.UI;

public class PopupTrigger1 : MonoBehaviour
{
    public GameObject popupUI; // Reference to the popup UI
    public Button okButton; // Reference to the "OK" button
    public GameObject player; // Reference to the player (First Person Controller)
    public GameObject playerCamera; // Reference to the First Person Camera

    private FirstPersonMovement movementScript; // Reference to the First Person Movement script
    private FirstPersonLook lookScript; // Reference to the First Person Look script
    private bool isPopupOpen = false; // To track if the popup is open

    void Start()
    {
        // Ensure popup starts hidden
        popupUI.SetActive(false);

        // Get reference to the movement and look scripts
        movementScript = player.GetComponent<FirstPersonMovement>();
        lookScript = playerCamera.GetComponent<FirstPersonLook>();

        // Assign the OnClick event for the OK button
        okButton.onClick.AddListener(ClosePopup);
    }

    // This method triggers when the player enters the collider
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player1") && !isPopupOpen) // Ensure it's the player and popup is not already open
        {
            OpenPopup();
        }
    }

    // Method to open the popup and lock the player controls
    void OpenPopup()
    {
        isPopupOpen = true;
        popupUI.SetActive(true);

        // Disable player movement and camera controls
        movementScript.enabled = false; // Disable the First Person Movement script
        lookScript.enabled = false; // Disable the First Person Look script
        Cursor.lockState = CursorLockMode.None; // Show cursor for UI interaction
        Cursor.visible = true; // Make cursor visible
    }

    // Method to close the popup and re-enable player controls
    void ClosePopup()
    {
        isPopupOpen = false;
        popupUI.SetActive(false);

        // Re-enable player movement and camera controls
        movementScript.enabled = true;
        lookScript.enabled = true;
        Cursor.lockState = CursorLockMode.Locked; // Hide and lock the cursor back to center
        Cursor.visible = false; // Hide cursor again
    }
}
