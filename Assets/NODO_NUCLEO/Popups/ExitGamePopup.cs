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

    private void Awake()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
        IsPopupOpen = false;
    }

    private void Start()
    {
        SetupExitButton();
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
        IsPopupOpen = true;
        popupPanel.SetActive(true);

        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        if (playerCameraScript != null)
            playerCameraScript.enabled = false;

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
        // Call Application.Quit to close the game.
        Application.Quit();

        // Optional: Add a debug message to indicate this doesn't work in the Editor.
#if UNITY_EDITOR
        Debug.LogWarning("Application.Quit() does not work in the Unity Editor.");
#endif
    }

    private void OnDestroy()
    {
        if (exitButton != null)
            exitButton.onClick.RemoveListener(OnExitButtonClicked);

        IsPopupOpen = false;
    }
}

