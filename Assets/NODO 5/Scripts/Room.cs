using UnityEngine;

public class Room : MonoBehaviour
{
    public int roomNumber;
    private bool objectEntered = false;
    private GrabbableObject currentObject = null;
    private bool isWaitingForSettling = false;
    private float settlingTime = 1f; // Time to wait for the object to settle
    private float currentWaitTime = 0f;

    private void OnTriggerEnter(Collider other)
    {
        GrabbableObject grabbable = other.GetComponent<GrabbableObject>();
        if (grabbable != null && !grabbable.isCorrectlyPlaced)
        {
            if (grabbable.destinationRoomNumber == roomNumber)
            {
                objectEntered = true;
                currentObject = grabbable;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GrabbableObject grabbable = other.GetComponent<GrabbableObject>();
        if (grabbable == currentObject)
        {
            objectEntered = false;
            currentObject = null;
            isWaitingForSettling = false;
            currentWaitTime = 0f;
        }
    }

    private void Update()
    {
        if (objectEntered && currentObject != null)
        {
            ObjectGrabber grabber = FindObjectOfType<ObjectGrabber>();
            if (grabber != null && !grabber.IsHoldingObject(currentObject.gameObject))
            {
                if (!isWaitingForSettling)
                {
                    isWaitingForSettling = true;
                    currentWaitTime = 0f;
                }
                else
                {
                    currentWaitTime += Time.deltaTime;
                    if (currentWaitTime >= settlingTime)
                    {
                        currentObject.LockInPlace();
                        PopupSequenceManager.Instance.StartSequence(currentObject.objectID);

                        objectEntered = false;
                        currentObject = null;
                        isWaitingForSettling = false;
                        currentWaitTime = 0f;
                    }
                }
            }
            else
            {
                isWaitingForSettling = false;
                currentWaitTime = 0f;
            }
        }
    }
}