using UnityEngine;
using UnityEngine.UI;

public class PopupTriggerManager : MonoBehaviour
{
    public GameObject player; // Reference to the player (First Person Controller)
    public GameObject playerCamera; // Reference to the First Person Camera

    private FirstPersonMovement movementScript; // Reference to the First Person Movement script
    private FirstPersonLook lookScript; // Reference to the First Person Look script
    private bool isPopupOpen = false; // To track if a popup is open

    private GameObject currentPopupUI; // Currently active popup
    private Button currentOkButton; // "OK" button of the current popup

    void Start()
    {
        // Get reference to the movement and look scripts
        movementScript = player.GetComponent<FirstPersonMovement>();
        lookScript = playerCamera.GetComponent<FirstPersonLook>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the collider has a Popup UI component assigned to it
        PopupUI popupUIComponent = other.GetComponent<PopupUI>();
        if (popupUIComponent != null && !isPopupOpen)
        {
            OpenPopup(popupUIComponent);
        }
    }

    void OpenPopup(PopupUI popupUIComponent)
    {
        isPopupOpen = true;
        currentPopupUI = popupUIComponent.popupUI;
        currentOkButton = popupUIComponent.okButton;

        // Show the popup UI
        currentPopupUI.SetActive(true);

        // Disable player movement and camera controls
        movementScript.enabled = false;
        lookScript.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Assign the ClosePopup method to the "OK" button
        currentOkButton.onClick.AddListener(ClosePopup);
    }

    void ClosePopup()
    {
        isPopupOpen = false;

        // Hide the popup UI
        currentPopupUI.SetActive(false);

        // Re-enable player movement and camera controls
        movementScript.enabled = true;
        lookScript.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Remove the listener to prevent multiple calls
        currentOkButton.onClick.RemoveListener(ClosePopup);

        // Clear references to the current popup and button
        currentPopupUI = null;
        currentOkButton = null;
    }
}
