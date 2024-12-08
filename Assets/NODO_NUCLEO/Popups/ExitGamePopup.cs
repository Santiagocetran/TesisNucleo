using UnityEngine;
using UnityEngine.UI;

public class ExitGamePopup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private Button exitButton;

    [Header("Optional References")]
    [SerializeField] private MonoBehaviour playerMovementScript;
    [SerializeField] private MonoBehaviour playerCameraScript;

    public static bool IsPopupOpen { get; private set; }

    // Add these new variables
    private CursorLockMode originalLockState;
    private bool originalCursorVisible;
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
        SetupExitButton();

        // Store initial cursor state
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

    private void SetupExitButton()
    {
        if (exitButton == null)
        {
            Debug.LogError("Exit button not assigned!");
            return;
        }

        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !IsPopupOpen)
        {
            ShowPopup();
        }
    }

    private void ShowPopup()
    {
        Debug.Log("Showing Exit Popup");

        // Disable scripts first
        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        if (playerCameraScript != null)
            playerCameraScript.enabled = false;

        // Handle Rigidbody rotation and movement
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

        // Store cursor state before changing it
        originalLockState = Cursor.lockState;
        originalCursorVisible = Cursor.visible;

        // Show popup and update cursor
        IsPopupOpen = true;
        popupPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnExitButtonClicked()
    {
        Debug.Log("Exit button clicked!");
        ExitGame();
    }

    private void ExitGame()
    {
        Debug.Log("Exiting Game...");
        Application.Quit();

#if UNITY_EDITOR
        Debug.LogWarning("Application.Quit() does not work in the Unity Editor.");
#endif
    }

    private void OnDestroy()
    {
        if (exitButton != null)
            exitButton.onClick.RemoveListener(OnExitButtonClicked);

        IsPopupOpen = false;

        // Restore cursor state
        Cursor.lockState = originalLockState;
        Cursor.visible = originalCursorVisible;

        // Restore Rigidbody constraints
        if (playerRigidbody != null)
        {
            playerRigidbody.constraints = originalConstraints;
        }
    }
}