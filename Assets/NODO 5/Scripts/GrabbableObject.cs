using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableObject : MonoBehaviour
{
    public int destinationRoomNumber;
    public bool isCorrectlyPlaced = false; // New variable to track placement status
    private ParticleSystem particleEffect; // Reference to the particle system

    public int popupID;
    public int objectID;
    private void Start()
    {
        // Get the ParticleSystem component (either on this object or in children)
        particleEffect = GetComponentInChildren<ParticleSystem>();
    }

    // Method to control particle effect
    public void SetParticleEffect(bool active)
    {
        if (particleEffect != null)
        {
            if (active)
                particleEffect.Play();
            else
                particleEffect.Stop();
        }
    }

    // Add this method to make the object unmovable
    public void LockInPlace()
    {
        if (!isCorrectlyPlaced) // Only count it if it wasn't already placed
        {
            isCorrectlyPlaced = true;
            SetParticleEffect(false);

            // Disable the grabbable layer
            gameObject.layer = LayerMask.NameToLayer("Default");

            // Make the rigidbody kinematic and unmovable
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            // Notify the manager that this object is correctly placed
            if (ObjectPlacementManager.Instance != null)
            {
                ObjectPlacementManager.Instance.ObjectCorrectlyPlaced();
            }
        }
    }

    public void ShowObjectPopup()
    {
        PopupManager.Instance.ShowPopup(popupID);
    }
}