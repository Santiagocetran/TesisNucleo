using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableObject : MonoBehaviour
{
    public int destinationRoomNumber;
    public bool isCorrectlyPlaced = false; // New variable to track placement status

    // Add this method to make the object unmovable
    public void LockInPlace()
    {
        isCorrectlyPlaced = true;

        // Disable the grabbable layer
        gameObject.layer = LayerMask.NameToLayer("Default");

        // Make the rigidbody kinematic and unmovable
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }
}