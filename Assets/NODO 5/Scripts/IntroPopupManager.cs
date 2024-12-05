using UnityEngine;
using UnityEngine.UI;

public class IntroPopupManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Canvas firstIntroPopup;
    [SerializeField] private Canvas secondIntroPopup;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button closeButton;

    private void Start()
    {
        // Hide both popups initially
        firstIntroPopup.gameObject.SetActive(false);
        secondIntroPopup.gameObject.SetActive(false);

        // Setup button listeners
        continueButton.onClick.AddListener(ShowSecondPopup);
        closeButton.onClick.AddListener(CloseIntroSequence);

        // Show first popup after a small delay
        Invoke("ShowFirstPopup", 0.5f);
    }

    private void ShowFirstPopup()
    {
        firstIntroPopup.gameObject.SetActive(true);
        secondIntroPopup.gameObject.SetActive(false);

        // Use the existing PopupManager to handle player state
        PopupManager.Instance.LockPlayerMovement();

        // Show cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void ShowSecondPopup()
    {
        firstIntroPopup.gameObject.SetActive(false);
        secondIntroPopup.gameObject.SetActive(true);
    }

    private void CloseIntroSequence()
    {
        firstIntroPopup.gameObject.SetActive(false);
        secondIntroPopup.gameObject.SetActive(false);

        // Use the existing PopupManager to handle player state
        PopupManager.Instance.UnlockPlayerMovement();

        // Hide cursor and lock it
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}