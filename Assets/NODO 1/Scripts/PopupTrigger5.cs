using UnityEngine;
using UnityEngine.UI;

public class PopupTrigger5 : MonoBehaviour
{
    public GameObject popupUI;
    public Button okButton;
    public GameObject player;
    public GameObject playerCamera;
    public Collider triggerCollider;

    private FirstPersonMovement movementScript;
    private CinemaFirstPersonLook lookScript; // Changed to CinemaFirstPersonLook
    private bool isPopupOpen = false;

    void Start()
    {
        popupUI.SetActive(false);

        movementScript = player.GetComponent<FirstPersonMovement>();
        lookScript = playerCamera.GetComponent<CinemaFirstPersonLook>(); // Changed to CinemaFirstPersonLook

        okButton.onClick.AddListener(StopPopup);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPopupOpen)
        {
            StartPopup();
        }
    }

    void StartPopup()
    {
        isPopupOpen = true;
        popupUI.SetActive(true);

        movementScript.enabled = false;
        if (lookScript != null) // Added null check for safety
        {
            lookScript.enabled = false;
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void StopPopup()
    {
        isPopupOpen = false;
        popupUI.SetActive(false);

        movementScript.enabled = true;
        if (lookScript != null) // Added null check for safety
        {
            lookScript.enabled = true;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        triggerCollider.enabled = false;
    }
}