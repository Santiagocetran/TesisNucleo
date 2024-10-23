using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemaDoorController : MonoBehaviour
{
    public Animator doorAnimator1;
    public Animator doorAnimator2;

    private bool isDoorOpened = false;

    private TicketBoothInteraction ticketBoothInteraction;

    // Start is called before the first frame update
    void Start()
    {
        ticketBoothInteraction = FindObjectOfType<TicketBoothInteraction>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ticketBoothInteraction.counter == 4 && !isDoorOpened)
        {
            CheckForDoorInteraction();
        }
    }

    void CheckForDoorInteraction()
    {
        if (Vector3.Distance(transform.position, ticketBoothInteraction.playerCamera.position) < ticketBoothInteraction.interactionDistance)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                OpenDoor();
            }
        }
    }

    void OpenDoor()
    {
        Debug.Log("OpenDoor method is called");

        ticketBoothInteraction.doorPopupPanel.SetActive(false);

        doorAnimator1.SetTrigger("OpenDoor");
        doorAnimator2.SetTrigger("OpenDoor");

        isDoorOpened = true;

        Debug.Log("Door is opened");
    }
}
