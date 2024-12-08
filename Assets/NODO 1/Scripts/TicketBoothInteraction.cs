using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TicketBoothInteraction : MonoBehaviour
{
    public float interactionDistance = 0.5f;
    public Transform playerCamera;
    public TextMeshProUGUI interactionText;

    // Separate continue buttons for each popup
    public Button continueButton1;
    public Button continueButton2;
    public Button continueButton3;
    public Button continueButton4;

    public GameObject popupPanel1;
    public GameObject popupPanel2;
    public GameObject popupPanel3;
    public GameObject popupPanel4;

    public GameObject doorPopupPanel;
    public Button doorPopupCloseButton;

    public GameObject infoPanel1;
    public GameObject infoPanel2;
    public GameObject infoPanel3;
    public GameObject infoPanel4;

    public Button okButton1;
    public Button okButton2;
    public Button okButton3;
    public Button okButton4;

    private FirstPersonMovement playerController;
    private FirstPersonLook cameraLook;

    public TextMeshProUGUI counterText;
    public int counter = 0;
    private bool isInteracting = false;
    private bool hasAllTickets = false;

    private GameObject activePopup;

    public Animator leftDoorAnimation;
    public Animator rightDoorAnimation;

    public AudioSource doorAudioSource;

    // Add the new method here, before Start()
    public bool IsInteracting()
    {
        return isInteracting;
    }
    void Start()
    {
        playerCamera = Camera.main.transform;
        interactionText.gameObject.SetActive(false);
        DeactivateAllPopups();

        // Assign individual continue button listeners
        continueButton1.onClick.AddListener(() => CloseMovieSelectionPopup(1));
        continueButton2.onClick.AddListener(() => CloseMovieSelectionPopup(2));
        continueButton3.onClick.AddListener(() => CloseMovieSelectionPopup(3));
        continueButton4.onClick.AddListener(() => CloseMovieSelectionPopup(4));

        okButton1.onClick.AddListener(() => CloseInfoPanel(infoPanel1));
        okButton2.onClick.AddListener(() => CloseInfoPanel(infoPanel2));
        okButton3.onClick.AddListener(() => CloseInfoPanel(infoPanel3));
        okButton4.onClick.AddListener(() => CloseInfoPanel(infoPanel4));

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
        infoPanel1.SetActive(false);
        infoPanel2.SetActive(false);
        infoPanel3.SetActive(false);
        infoPanel4.SetActive(false);
        doorPopupPanel.SetActive(false);
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
        }

        if (cameraLook != null)
        {
            cameraLook.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseMovieSelectionPopup(int boothNumber)
    {
        if (activePopup == null) return;

        activePopup.SetActive(false);
        activePopup = null;

        isInteracting = false;

        IncrementCounter();

        // Open the corresponding movie info panel after closing the movie selection popup
        if (boothNumber == 1) OpenPopup(infoPanel1);
        else if (boothNumber == 2) OpenPopup(infoPanel2);
        else if (boothNumber == 3) OpenPopup(infoPanel3);
        else if (boothNumber == 4) OpenPopup(infoPanel4);
    }

    public void CloseInfoPanel(GameObject infoPanel)
    {
        infoPanel.SetActive(false);
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

    void IncrementCounter()
    {
        counter++;
        UpdateCounterText();

        if (counter >= 4)
        {
            hasAllTickets = true;
            OpenCinemaDoors();
        }
    }

    void OpenCinemaDoors()
    {
        leftDoorAnimation.SetTrigger("OpenDoor");
        rightDoorAnimation.SetTrigger("OpenDoor");
        Debug.Log("doors are opening");

        // Stop the audio when the door opens
        if (doorAudioSource != null && doorAudioSource.isPlaying)
        {
            doorAudioSource.Stop();
        }
    }

    void CheckForBoothInteraction()
    {
        if (isInteracting) return;

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.CompareTag("TicketBooth"))
            {
                interactionText.text = "Presiona E para interactuar";
                interactionText.gameObject.SetActive(true);

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
                if (!hasAllTickets)
                {
                    interactionText.text = "Presiona E para abrir la puerta";
                    interactionText.gameObject.SetActive(true);

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        OpenPopup(doorPopupPanel);
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
        else
        {
            interactionText.gameObject.SetActive(false);
        }
    }

    void UpdateCounterText()
    {
        counterText.text = counter + "/4";
        Debug.Log("Counter value: " + counter);
    }
}
