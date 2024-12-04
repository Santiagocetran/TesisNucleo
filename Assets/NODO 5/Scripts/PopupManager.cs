using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    [Header("UI References")]
    public Canvas popup1Canvas;
    public Canvas popup2Canvas;
    public Canvas popup3Canvas;
    public Button closeButton1;
    public Button closeButton2;
    public Button closeButton3;

    [Header("Player References")]
    public FirstPersonLook lookScript;
    public FirstPersonMovement movementScript;

    public static bool IsPopupOpen { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Ensure all popups are hidden at start
        popup1Canvas.gameObject.SetActive(false);
        popup2Canvas.gameObject.SetActive(false);
        popup3Canvas.gameObject.SetActive(false);

        // Add listeners to close buttons
        closeButton1.onClick.AddListener(() => ClosePopup(1));
        closeButton2.onClick.AddListener(() => ClosePopup(2));
        closeButton3.onClick.AddListener(() => ClosePopup(3));
    }

    public void ShowPopup(int popupID)
    {
        IsPopupOpen = true;
        // Lock player movement
        if (movementScript != null)
        {
            movementScript.enabled = false;
        }

        // Show appropriate popup based on ID
        switch (popupID)
        {
            case 1:
                popup1Canvas.gameObject.SetActive(true);
                break;
            case 2:
                popup2Canvas.gameObject.SetActive(true);
                break;
            case 3:
                popup3Canvas.gameObject.SetActive(true);
                break;
            default:
                Debug.LogWarning("Invalid popup ID: " + popupID);
                return;
        }

        // Enable cursor for button interaction
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ClosePopup(int popupID)
    {
        // Hide appropriate popup
        switch (popupID)
        {
            case 1:
                popup1Canvas.gameObject.SetActive(false);
                break;
            case 2:
                popup2Canvas.gameObject.SetActive(false);
                break;
            case 3:
                popup3Canvas.gameObject.SetActive(false);
                break;
        }

        // Check if all popups are closed
        if (!popup1Canvas.gameObject.activeSelf &&
            !popup2Canvas.gameObject.activeSelf &&
            !popup3Canvas.gameObject.activeSelf)
        {
            IsPopupOpen = false;

            // Unlock player movement
            if (movementScript != null) movementScript.enabled = true;

            // Reset cursor state
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void LockPlayerMovement()
    {
        IsPopupOpen = true;
        if (movementScript != null)
        {
            movementScript.enabled = false;
        }
    }

    public void UnlockPlayerMovement()
    {
        IsPopupOpen = false;
        if (movementScript != null)
        {
            movementScript.enabled = true;
        }
    }
}
