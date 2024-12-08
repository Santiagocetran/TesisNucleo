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
    private bool hasBeenShown = false;

    private CursorLockMode originalLockState;
    private bool originalCursorVisible;

    // Add reference to player's Rigidbody
    private Rigidbody playerRigidbody;
    private RigidbodyConstraints originalConstraints;

    private void Awake()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
        IsPopupOpen = false;
    }

    private void Start()
    {
        SetupCloseButton();
        originalLockState = Cursor.lockState;
        originalCursorVisible = Cursor.visible;

        // Get player's Rigidbody reference
        if (playerMovementScript != null)
        {
            playerRigidbody = playerMovementScript.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                originalConstraints = playerRigidbody.constraints;
            }
        }
    }

    private void SetupCloseButton()
    {
        if (closeButton == null)
        {
            Debug.LogError("Close button not assigned!");
            return;
        }

        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(OnCloseButtonClicked);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !IsPopupOpen && !hasBeenShown)
        {
            ShowPopup();
        }
    }

    private void ShowPopup()
    {
        Debug.Log("Showing Popup");

        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        if (playerCameraScript != null)
            playerCameraScript.enabled = false;

        // Freeze player rotation
        if (playerRigidbody != null)
        {
            // Store current velocity
            Vector3 currentVelocity = playerRigidbody.velocity;

            // Freeze all rotation but keep existing constraints for position
            playerRigidbody.constraints = playerRigidbody.constraints |
                                        RigidbodyConstraints.FreezeRotationX |
                                        RigidbodyConstraints.FreezeRotationY |
                                        RigidbodyConstraints.FreezeRotationZ;

            // Stop any existing rotation
            playerRigidbody.angularVelocity = Vector3.zero;

            // Maintain current velocity to prevent sudden stops
            playerRigidbody.velocity = currentVelocity;
        }

        originalLockState = Cursor.lockState;
        originalCursorVisible = Cursor.visible;

        IsPopupOpen = true;
        hasBeenShown = true;

        popupPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnCloseButtonClicked()
    {
        Debug.Log("Close button clicked!");
        ClosePopup();
    }

    public void ClosePopup()
    {
        Debug.Log("Closing Popup");

        popupPanel.SetActive(false);

        Cursor.lockState = originalLockState;
        Cursor.visible = originalCursorVisible;

        // Restore original Rigidbody constraints
        if (playerRigidbody != null)
        {
            playerRigidbody.constraints = originalConstraints;
        }

        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        if (playerCameraScript != null)
            playerCameraScript.enabled = true;

        IsPopupOpen = false;

        Collider triggerCollider = GetComponent<Collider>();
        if (triggerCollider != null)
        {
            triggerCollider.enabled = false;
        }
    }

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

        Cursor.lockState = originalLockState;
        Cursor.visible = originalCursorVisible;

        // Restore Rigidbody constraints on destroy
        if (playerRigidbody != null)
        {
            playerRigidbody.constraints = originalConstraints;
        }
    }
}