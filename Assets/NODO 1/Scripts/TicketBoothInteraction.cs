using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TicketBoothInteraction : MonoBehaviour
{
    public float interactionDistance = 0.5f; // Distance to interact
    private Transform playerCamera;  // Reference to the camera for raycasting
    public TextMeshProUGUI interactionText;
    public Button continueButton;

    public GameObject popupPanel1;
    public GameObject popupPanel2;
    public GameObject popupPanel3;
    public GameObject popupPanel4;
    public GameObject doorPupupPanel;

    public Button doorPopupCloseButton;

    private FirstPersonMovement playerController;
    private FirstPersonLook cameraLook;

    public TextMeshProUGUI counterText;
    private int counter = 0;
    private bool isInteracting = false;

    private GameObject activePopup;

    void Start()
    {
        // Get the camera from the First Person Controller
        playerCamera = Camera.main.transform;
        
        interactionText.gameObject.SetActive(false);
        DeactivateAllPopups();

        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(ClosePopup);

        doorPopupCloseButton.onClick.AddListener(ClosePopupDoor);

        playerController = GetComponent<FirstPersonMovement>();
        cameraLook = playerCamera.GetComponent<FirstPersonLook>();

        UpdateCounterText();
    }

    void Update()
    {
        CheckForBoothInteraction();
    }

    void DeactivateAllPopups()
    {
        popupPanel1.SetActive(false);
        popupPanel2.SetActive(false);
        popupPanel3.SetActive(false);
        popupPanel4.SetActive(false);
        doorPupupPanel.SetActive(false);
    }

    public void OpenPopup(GameObject popup)
    {
        activePopup = popup;
        activePopup.SetActive(true);

        interactionText.gameObject.SetActive(false);
        isInteracting = true;

        if (playerController != null)
        {
            playerController.enabled = false;
            Debug.Log("player movement disabled");
        }

        if (cameraLook != null)
        {
            cameraLook.enabled = false;
            Debug.Log("Camera look disabled");
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ClosePopup()
    {
        if (activePopup == null) return;

        activePopup.SetActive(false);
        activePopup = null;

        counter++;
        UpdateCounterText();

        isInteracting = false;

        if (playerController != null)
        {
            playerController.enabled = true;
        }

        if (cameraLook != null)
        {
            cameraLook.enabled = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ClosePopupDoor()
    {
        if (activePopup == null) return;

        activePopup.SetActive(false);
        activePopup = null;

        isInteracting = false;

        if (playerController != null)
        {
            playerController.enabled = true;
        }

        if (cameraLook != null)
        {
            cameraLook.enabled = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void CheckForBoothInteraction()
    {
        if (isInteracting) return;

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        // Check if the ray hits a Ticket Booth within the interaction distance
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.CompareTag("TicketBooth"))
            {

                // Show prompt (you can customize this)
                interactionText.text = "Press E to interact";
                interactionText.gameObject.SetActive(true);

                // Check if the player presses E
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (hit.collider.name == "Booth1")
                    {
                        OpenPopup(popupPanel1);
                    }
                    else if (hit.collider.name == "Booth2")
                    {
                        OpenPopup(popupPanel2);
                    }
                    else if (hit.collider.name == "Booth3")
                    {
                        OpenPopup(popupPanel3);
                    }
                    else if (hit.collider.name == "Booth4")
                    {
                        OpenPopup(popupPanel4);
                    }
                }
            }
            else if (hit.collider.CompareTag("Door"))
            {
                interactionText.text = "Press [E] to open door";
                interactionText.gameObject.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    OpenPopup(doorPupupPanel);
                }
            }
            else
            {
                interactionText.gameObject.SetActive(false);
            }
        }
        else
        {
            interactionText.gameObject.SetActive(false);
        }
    }


    void UpdateCounterText()
    {
       counterText.text = "Tickets completed: " + counter + "/4";
    }
}
