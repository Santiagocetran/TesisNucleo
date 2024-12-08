using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupTrigger : MonoBehaviour
{
    [Header("UI References")]
    public GameObject popupPanel;
    public Button closeButton;

    private bool hasBeenShown = false;  // Track if popup has been shown

    private void Start()
    {
        // Hide popup at start
        popupPanel.SetActive(false);

        // Add listener to close button
        closeButton.onClick.AddListener(ClosePopup);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only show popup if it hasn't been shown before
        if (other.CompareTag("Player") && !hasBeenShown)
        {
            // Pause game and show popup
            Time.timeScale = 0f;
            ShowPopup();
        }
    }

    private void ShowPopup()
    {
        popupPanel.SetActive(true);
        hasBeenShown = true;  // Mark as shown
    }

    private void ClosePopup()
    {
        // Resume game
        Time.timeScale = 1f;

        // Hide popup
        popupPanel.SetActive(false);

        // Optionally, disable the trigger collider so it doesn't fire again
        GetComponent<Collider2D>().enabled = false;
    }
}