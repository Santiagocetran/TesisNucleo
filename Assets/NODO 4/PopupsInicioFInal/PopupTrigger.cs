using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupTrigger : MonoBehaviour
{
    [Header("UI References")]
    public GameObject popupPanel;
    public Button closeButton;

    private void Start()
    {
        // Hide popup at start
        popupPanel.SetActive(false);

        // Add listener to close button
        closeButton.onClick.AddListener(ClosePopup);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Pause game and show popup
            Time.timeScale = 0f;
            ShowPopup();
        }
    }

    private void ShowPopup()
    {
        popupPanel.SetActive(true);
        
    }

    private void ClosePopup()
    {
        // Resume game
        Time.timeScale = 1f;

        // Hide popup
        popupPanel.SetActive(false);
    }
}