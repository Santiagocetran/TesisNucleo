using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PopupSequenceManager : MonoBehaviour
{
    public static PopupSequenceManager Instance { get; private set; }

    [System.Serializable]
    public class PopupSequence
    {
        public int objectID; // Matches with the object that triggers this sequence
        public Canvas[] popupCanvases; // Array of 4 canvases for this sequence
        public Button[] nextButtons; // Array of 3 "next" buttons
        public Button closeButton; // Close button for the final popup
    }

    public PopupSequence[] sequences;
    private int currentSequenceIndex = -1;
    private int currentPopupIndex = -1;

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
        // Set up button listeners for all sequences
        foreach (var sequence in sequences)
        {
            // Set up "Next" buttons
            for (int i = 0; i < sequence.nextButtons.Length; i++)
            {
                int sequenceIndex = System.Array.IndexOf(sequences, sequence);
                int popupIndex = i;
                sequence.nextButtons[i].onClick.AddListener(() => ShowNextInSequence(sequenceIndex, popupIndex));
            }

            // Set up close button
            sequence.closeButton.onClick.AddListener(() => EndSequence(System.Array.IndexOf(sequences, sequence)));

            // Ensure all popups start hidden
            foreach (var canvas in sequence.popupCanvases)
            {
                canvas.gameObject.SetActive(false);
            }
        }
    }

    public void StartSequence(int objectID)
    {
        // Find the sequence for this object
        int sequenceIndex = System.Array.FindIndex(sequences, s => s.objectID == objectID);
        if (sequenceIndex == -1)
        {
            Debug.LogWarning($"No popup sequence found for object ID: {objectID}");
            return;
        }

        currentSequenceIndex = sequenceIndex;
        currentPopupIndex = 0;

        // Show first popup
        ShowPopup(sequenceIndex, 0);
    }

    private void ShowNextInSequence(int sequenceIndex, int currentIndex)
    {
        // Hide current popup
        sequences[sequenceIndex].popupCanvases[currentIndex].gameObject.SetActive(false);

        // Show next popup
        ShowPopup(sequenceIndex, currentIndex + 1);
    }

    private void ShowPopup(int sequenceIndex, int popupIndex)
    {
        currentPopupIndex = popupIndex;

        // Enable the popup
        sequences[sequenceIndex].popupCanvases[popupIndex].gameObject.SetActive(true);

        // Ensure player movement is locked
        PopupManager.Instance.LockPlayerMovement();

        // Show cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void EndSequence(int sequenceIndex)
    {
        // Hide the final popup
        sequences[sequenceIndex].popupCanvases[3].gameObject.SetActive(false);

        // Reset indices
        currentSequenceIndex = -1;
        currentPopupIndex = -1;

        // Unlock player movement and hide cursor
        PopupManager.Instance.UnlockPlayerMovement();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}