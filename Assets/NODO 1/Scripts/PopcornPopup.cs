using UnityEngine;
using UnityEngine.UI;

public class PopcornPopup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private Button closeButton;

    [Header("Optional References")]
    [SerializeField] private MonoBehaviour playerMovementScript;
    [SerializeField] private CinemaFirstPersonLook playerCameraScript; // Changed to specific type

    public static bool IsPopupOpen { get; private set; }
    private bool hasBeenShown = false;

    private CursorLockMode originalLockState;
    private bool originalCursorVisible;
    private Quaternion originalCameraRotation; // Store camera rotation

    private Rigidbody playerRigidbody;
    private RigidbodyConstraints originalConstraints;
    private Transform cameraTransform; // Reference to camera transform

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

        if (playerMovementScript != null)
        {
            playerRigidbody = playerMovementScript.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                originalConstraints = playerRigidbody.constraints;
            }
        }

        // Store camera transform if camera script is assigned
        if (playerCameraScript != null)
        {
            cameraTransform = playerCameraScript.transform;
            originalCameraRotation = cameraTransform.rotation;
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
        {
            playerCameraScript.enabled = false;
            // Store and lock camera rotation
            if (cameraTransform != null)
            {
                originalCameraRotation = cameraTransform.rotation;
            }
        }

        if (playerRigidbody != null)
        {
            Vector3 currentVelocity = playerRigidbody.velocity;
            playerRigidbody.constraints = RigidbodyConstraints.FreezeAll; // Freeze everything
            playerRigidbody.velocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
        }

        originalLockState = Cursor.lockState;
        originalCursorVisible = Cursor.visible;

        IsPopupOpen = true;
        hasBeenShown = true;

        popupPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        // Keep camera locked in position while popup is open
        if (IsPopupOpen && cameraTransform != null)
        {
            cameraTransform.rotation = originalCameraRotation;
        }

        if (IsPopupOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePopup();
        }
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

    private void OnDestroy()
    {
        if (closeButton != null)
            closeButton.onClick.RemoveListener(OnCloseButtonClicked);

        IsPopupOpen = false;

        Cursor.lockState = originalLockState;
        Cursor.visible = originalCursorVisible;

        if (playerRigidbody != null)
        {
            playerRigidbody.constraints = originalConstraints;
        }
    }
}