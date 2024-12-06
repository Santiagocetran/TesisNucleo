using UnityEngine;
using UnityEngine.UI;

public class Node5Popups : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private Button closeButton;

    [Header("Optional References")]
    [SerializeField] private MonoBehaviour playerMovementScript;
    [SerializeField] private MonoBehaviour playerCameraScript;

    public static bool IsPopupOpen { get; private set; }
    private bool hasBeenShown = false; // New variable to track if popup has been shown

    private void Awake()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
        IsPopupOpen = false;
    }

    private void Start()
    {
        // Set up the button listener in Start instead of Awake
        SetupCloseButton();
    }

    private void SetupCloseButton()
    {
        if (closeButton == null)
        {
            Debug.LogError("Close button not assigned!");
            return;
        }

        // Remove any existing listeners and add our close function
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(OnCloseButtonClicked);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only show popup if it hasn't been shown before
        if (other.CompareTag("Player") && !IsPopupOpen && !hasBeenShown)
        {
            ShowPopup();
        }
    }

    private void ShowPopup()
    {
        Debug.Log("Showing Popup");
        IsPopupOpen = true;
        hasBeenShown = true; // Mark that the popup has been shown
        popupPanel.SetActive(true);

        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        if (playerCameraScript != null)
            playerCameraScript.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // New method specifically for the button click
    public void OnCloseButtonClicked()
    {
        Debug.Log("Close button clicked!");
        ClosePopup();
    }

    public void ClosePopup()
    {
        Debug.Log("Closing Popup");
        IsPopupOpen = false;
        popupPanel.SetActive(false);

        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        if (playerCameraScript != null)
            playerCameraScript.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Disable the trigger collider after closing
        Collider triggerCollider = GetComponent<Collider>();
        if (triggerCollider != null)
        {
            triggerCollider.enabled = false;
        }
    }

    // Optional: Add keyboard escape key as alternative way to close
    private void Update()
    {
        if (IsPopupOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePopup();
        }
    }

    private void OnDestroy()
    {
        if (closeButton != null)
            closeButton.onClick.RemoveListener(OnCloseButtonClicked);

        IsPopupOpen = false;
    }
}