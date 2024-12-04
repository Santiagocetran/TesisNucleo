using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PopupSequenceManager : MonoBehaviour
{
    public static PopupSequenceManager Instance { get; private set; }

    [System.Serializable]
    public class PopupSequence
    {
        public int objectID;
        public Canvas[] popupCanvases = new Canvas[3]; // Changed to 3 popups
        public Button[] nextButtons = new Button[2];   // Changed to 2 "next" buttons (first two popups only)
        public Button closeButton;  // For the final (third) popup
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
        if (sequences == null || sequences.Length == 0)
        {
            Debug.LogError("No sequences configured!");
            return;
        }

        // Set up button listeners for all sequences
        for (int sequenceIndex = 0; sequenceIndex < sequences.Length; sequenceIndex++)
        {
            var sequence = sequences[sequenceIndex];

            // Verify sequence setup
            if (sequence.popupCanvases == null || sequence.popupCanvases.Length != 3)
            {
                Debug.LogError($"Sequence {sequenceIndex} doesn't have exactly 3 canvases!");
                continue;
            }

            if (sequence.nextButtons == null || sequence.nextButtons.Length != 2)
            {
                Debug.LogError($"Sequence {sequenceIndex} doesn't have exactly 2 next buttons!");
                continue;
            }

            // Set up "Next" buttons
            for (int i = 0; i < sequence.nextButtons.Length; i++)
            {
                int currentSequenceIndex = sequenceIndex;
                int popupIndex = i;

                if (sequence.nextButtons[i] != null)
                {
                    sequence.nextButtons[i].onClick.RemoveAllListeners();
                    sequence.nextButtons[i].onClick.AddListener(() => ShowNextInSequence(currentSequenceIndex, popupIndex));
                }
                else
                {
                    Debug.LogError($"Next button {i} not assigned for sequence {sequenceIndex}");
                }
            }

            // Set up close button
            if (sequence.closeButton != null)
            {
                int currentSequenceIndex = sequenceIndex;
                sequence.closeButton.onClick.RemoveAllListeners();
                sequence.closeButton.onClick.AddListener(() => {
                    Debug.Log("Close button clicked for sequence " + currentSequenceIndex);
                    EndSequence(currentSequenceIndex);
                });
            }
            else
            {
                Debug.LogError($"Close button not assigned for sequence {sequenceIndex}");
            }

            // Hide all popups initially
            foreach (var canvas in sequence.popupCanvases)
            {
                if (canvas != null)
                {
                    canvas.gameObject.SetActive(false);
                }
                else
                {
                    Debug.LogError($"Missing canvas in sequence {sequenceIndex}");
                }
            }
        }
    }

    public void StartSequence(int objectID)
    {
        Debug.Log($"Starting sequence for object {objectID}");

        int sequenceIndex = System.Array.FindIndex(sequences, s => s.objectID == objectID);
        if (sequenceIndex == -1 || sequenceIndex >= sequences.Length)
        {
            Debug.LogWarning($"No popup sequence found for object ID: {objectID}");
            return;
        }

        currentSequenceIndex = sequenceIndex;
        currentPopupIndex = 0;

        ShowPopup(sequenceIndex, 0);
    }

    private void ShowNextInSequence(int sequenceIndex, int currentIndex)
    {
        if (sequenceIndex >= sequences.Length || currentIndex >= sequences[sequenceIndex].popupCanvases.Length - 1)
        {
            Debug.LogError($"Invalid sequence or popup index: {sequenceIndex}, {currentIndex}");
            return;
        }

        Debug.Log($"Moving to next popup. Sequence: {sequenceIndex}, Current Index: {currentIndex}");

        if (sequences[sequenceIndex].popupCanvases[currentIndex] != null)
        {
            sequences[sequenceIndex].popupCanvases[currentIndex].gameObject.SetActive(false);
        }

        ShowPopup(sequenceIndex, currentIndex + 1);
    }

    private void ShowPopup(int sequenceIndex, int popupIndex)
    {
        if (sequenceIndex >= sequences.Length || popupIndex >= sequences[sequenceIndex].popupCanvases.Length)
        {
            Debug.LogError($"Invalid sequence or popup index: {sequenceIndex}, {popupIndex}");
            return;
        }

        Debug.Log($"Showing popup. Sequence: {sequenceIndex}, Popup Index: {popupIndex}");

        currentPopupIndex = popupIndex;

        var canvas = sequences[sequenceIndex].popupCanvases[popupIndex];
        if (canvas != null)
        {
            canvas.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError($"Canvas is null for sequence {sequenceIndex}, popup {popupIndex}");
            return;
        }

        PopupManager.Instance.LockPlayerMovement();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void EndSequence(int sequenceIndex)
    {
        Debug.Log($"Ending sequence {sequenceIndex}");

        if (sequenceIndex >= sequences.Length)
        {
            Debug.LogError($"Invalid sequence index: {sequenceIndex}");
            return;
        }

        // Hide all popups in the sequence
        foreach (var canvas in sequences[sequenceIndex].popupCanvases)
        {
            if (canvas != null)
            {
                canvas.gameObject.SetActive(false);
            }
        }

        currentSequenceIndex = -1;
        currentPopupIndex = -1;

        PopupManager.Instance.UnlockPlayerMovement();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}