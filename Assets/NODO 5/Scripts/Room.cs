using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int roomNumber;

    private void OnTriggerEnter(Collider other)
    {
        GrabbableObject grabbable = other.GetComponent<GrabbableObject>();
        if (grabbable != null && !grabbable.isCorrectlyPlaced)
        {
            if (grabbable.destinationRoomNumber == roomNumber)
            {
                Debug.Log($"Object placed in correct room! Room {roomNumber}");
                grabbable.LockInPlace();
                PopupSequenceManager.Instance.StartSequence(grabbable.objectID);
            }
        }
    }
}