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
    public GameObject popupPanel;
    public Button continueButton;
    

    private FirstPersonMovement playerController;
    private FirstPersonLook cameraLook;

    public TextMeshProUGUI counterText;
    private int counter = 0;

    void Start()
    {
        // Get the camera from the First Person Controller
        playerCamera = Camera.main.transform;
        
        interactionText.gameObject.SetActive(false);
        popupPanel.SetActive(false);

        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(ClosePopup);

        playerController = GetComponent<FirstPersonMovement>();
        cameraLook = playerCamera.GetComponent<FirstPersonLook>();

        UpdateCounterText();
    }

    void Update()
    {
        CheckForBoothInteraction();
    }

    public void OpenPopup()
    {
        popupPanel.SetActive(true);
        interactionText.gameObject.SetActive(false);

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        if (cameraLook != null)
        {
            cameraLook.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void CheckForBoothInteraction()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        // Check if the ray hits a Ticket Booth within the interaction distance
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.CompareTag("TicketBooth") && !popupPanel.activeSelf)
            {

                // Show prompt (you can customize this)
                interactionText.text = "Press E to interact";
                interactionText.gameObject.SetActive(true);

                // Check if the player presses E
                if (Input.GetKeyDown(KeyCode.E))
                {
                    OpenPopup();
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

    public void ClosePopup()
    {
        Debug.Log("close popup");
        popupPanel.SetActive(false);

        counter++;
        UpdateCounterText();

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

    void UpdateCounterText()
    {
       counterText.text = "Tickets completed: " + counter + "/4";
    }
}
