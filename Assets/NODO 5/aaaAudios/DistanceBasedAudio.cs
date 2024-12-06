using UnityEngine;

public class DistanceBasedAudio : MonoBehaviour
{
    public float maxDistance = 10f;
    public float minDistance = 1f;
    public float volumeMultiplier = 0.1f;    // Add this to control overall volume
    private AudioSource audioSource;
    private Transform playerTransform;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.maxDistance = maxDistance;
        audioSource.minDistance = minDistance;

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        float volume = (1f - Mathf.Clamp01(distance / maxDistance)) * volumeMultiplier;
        audioSource.volume = volume;
    }
}