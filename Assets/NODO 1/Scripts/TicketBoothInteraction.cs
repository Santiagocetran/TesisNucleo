using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicketBoothInteraction : MonoBehaviour
{
    public float interactionDistance = 3f; // Distance to interact
    private Transform playerCamera;  // Reference to the camera for raycasting

    void Start()
    {
        // Get the camera from the First Person Controller
        playerCamera = Camera.main.transform;
    }

    void Update()
    {
        CheckForBoothInteraction();
    }

    void CheckForBoothInteraction()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        // Check if the ray hits a Ticket Booth within the interaction distance
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.CompareTag("TicketBooth"))
            {
                // Show prompt (you can customize this)
                Debug.Log("Press E to interact with the ticket booth");

                // Check if the player presses E
                if (Input.GetKeyDown(KeyCode.E))
                {
                    // Perform interaction logic here
                    Debug.Log("Interacting with Ticket Booth...");
                    // Placeholder for popup or next action
                }
            }
        }
    }
}
