using UnityEngine;

public class DistanceBasedAudio : MonoBehaviour
{
    public float maxDistance = 10f;
    public float minDistance = 1f;
    public float volumeMultiplier = 0.1f;    // Add this to control overall volume
    private AudioSource audioSource;
    private Transform playerTransform;

    private GrabbableObject grabbableObject; // Reference to the GrabbableObject component

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.maxDistance = maxDistance;
        audioSource.minDistance = minDistance;

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // Get reference to the GrabbableObject component
        grabbableObject = GetComponent<GrabbableObject>();
        if (grabbableObject == null)
        {
            Debug.LogWarning("No GrabbableObject component found on object with DistanceBasedAudio!");
        }
    }

    void Update()
    {
        // Check if the object is correctly placed
        if (grabbableObject != null && grabbableObject.isCorrectlyPlaced)
        {
            // If object is placed, ensure audio is stopped
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            return; // Exit early
        }

        // Normal audio behavior if object isn't placed
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        float volume = (1f - Mathf.Clamp01(distance / maxDistance)) * volumeMultiplier;
        audioSource.volume = volume;
    }
}