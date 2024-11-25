using UnityEngine;

public class PopupCloseButton : MonoBehaviour
{
    public GameObject popupPanel;

    public void ClosePopup()
    {
        popupPanel.SetActive(false); // Hide the popup
        Time.timeScale = 1f; // Resume the game
    }
}
