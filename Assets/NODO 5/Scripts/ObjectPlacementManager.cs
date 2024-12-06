using UnityEngine;

public class ObjectPlacementManager : MonoBehaviour
{
    public static ObjectPlacementManager Instance { get; private set; }

    [Tooltip("References to all door Animator components")]
    public Animator[] doorAnimators;

    [Tooltip("The number of objects that need to be placed correctly")]
    public int requiredObjects = 2;

    // Add single audio source for door opening sound
    [Tooltip("Audio source for door opening sound")]
    public AudioSource doorSound;

    private int correctlyPlacedObjects = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Verify all door animators are assigned
        if (doorAnimators == null || doorAnimators.Length == 0)
        {
            Debug.LogWarning("No door animators assigned to ObjectPlacementManager!");
        }
    }

    public void ObjectCorrectlyPlaced()
    {
        correctlyPlacedObjects++;
        Debug.Log($"Object placed correctly. {correctlyPlacedObjects}/{requiredObjects} objects placed.");

        if (correctlyPlacedObjects >= requiredObjects)
        {
            OpenAllDoors();
        }
    }

    private void OpenAllDoors()
    {
        if (doorAnimators != null)
        {
            foreach (Animator doorAnimator in doorAnimators)
            {
                if (doorAnimator != null)
                {
                    Debug.Log($"Opening door: {doorAnimator.gameObject.name}");
                    doorAnimator.SetTrigger("Open");
                }
                else
                {
                    Debug.LogError("One of the door animators is null!");
                }
            }

            // Play the door sound once
            if (doorSound != null)
            {
                doorSound.Play();
            }
        }
        else
        {
            Debug.LogError("Door animators array not assigned!");
        }
    }
}