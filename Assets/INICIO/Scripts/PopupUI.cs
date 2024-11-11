using UnityEngine;
using UnityEngine.UI;

public class PopupUI : MonoBehaviour
{
    public GameObject popupUI; // Reference to the popup UI for this specific collider
    public Button okButton; // Reference to the "OK" button for this popup

    void Start()
    {
        // Ensure the popup starts hidden
        if (popupUI != null)
        {
            popupUI.SetActive(false);
        }
    }
}
