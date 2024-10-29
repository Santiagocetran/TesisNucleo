using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ComputerInteraction : MonoBehaviour
{
    public Camera playerCamera;
    public float rayDistance = 5f;
    public LayerMask computerLayer;

    public GameObject pressEText; // UI Text for "Press E to interact"
    public GameObject popupMessage; // Popup panel with a close button
    public Button closeButton; // Button to close the popup

    private GameObject currentComputer;

    public CardHighlight cardHighlight;

    // Player movement and look scripts
    public FirstPersonMovement playerMovement;
    public FirstPersonLook playerLook;
    public Collider playerCollider; // Reference to the player's collider
    public Rigidbody playerRigidBody; // Reference to player's Rigidbody

    // Chair position and rotation
    public Transform chairPosition; // Assign the chair’s Transform in the Inspector
    public Transform screenCenterPoint; // A Transform positioned at the center of the screen for precise alignment

    // Video Player
    public VideoPlayer computerVideoPlayer; // Attach the Video Player component here
    public VideoPlayer secondVideoPlayer;

    private bool isSitting = false;

    // Define the time (in seconds) where you want to pause the video
    public double pauseTimestamp = 44.9; // Adjust to your target time
    private bool hasPausedAtTimestamp = false;
    private bool isVideoResumed = false;

    public GameObject crosshair;

    public GameObject hiddenObject;

    void Start()
    {
        pressEText.SetActive(false); // Hide "Press E" prompt initially
        popupMessage.SetActive(false); // Hide popup message initially

        closeButton.onClick.AddListener(ClosePopup);
        LockCursor();

        // Subscribe to the end of the first video to start the second video
        if (computerVideoPlayer != null && secondVideoPlayer != null)
        {
            computerVideoPlayer.loopPointReached += OnFirstVideoFinished;
            secondVideoPlayer.enabled = false; // Disable second video initially

            // Subscribe to the end of the second video to make the player stand up
            secondVideoPlayer.loopPointReached += OnSecondVideoFinished;
        }
    }

    void Update()
    {
        if (!isSitting) // Only allow interaction when not sitting
        {
            RaycastForComputer();
            HandleComputerInteraction();
        }
        else
        {
            // Check if we should pause the video
            if (computerVideoPlayer.isPlaying && !hasPausedAtTimestamp && !isVideoResumed)
            {
                CheckForPausePoint();
            }

            // Check for "V" key press to resume the video if it has already paused at the pauseTimestamp
            if (hasPausedAtTimestamp)
            {
                Debug.Log("Waiting for V key press to resume video...");
                if (Input.GetKeyDown(KeyCode.V))
                {
                    Debug.Log("V key detected - attempting to resume video.");
                    ResumeVideo();
                }
            }
        }
    }

    void RaycastForComputer()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance, computerLayer))
        {
            GameObject hitComputer = hit.collider.gameObject;

            if (hitComputer != currentComputer)
            {
                currentComputer = hitComputer;
                ShowPressEText(true);
            }
        }
        else
        {
            ShowPressEText(false);
            currentComputer = null;
        }
    }

    void HandleComputerInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentComputer != null)
        {
            if (cardHighlight.hasCard)
            {
                SitAndInteractWithComputer();
            }
            else
            {
                ShowCluePopup();
            }
        }
    }

    void ShowPressEText(bool show)
    {
        pressEText.SetActive(show);
    }

    void SitAndInteractWithComputer()
    {
        Debug.Log("Player is interacting with the computer.");
        ShowPressEText(false);

        // Disable the crosshair
        if (crosshair != null)
        {
            crosshair.SetActive(false);
        }

        // Disable movement, camera look, and collider for smooth sitting
        LockPlayerControls();
        playerCollider.enabled = false;
        if (playerRigidBody != null)
        {
            playerRigidBody.velocity = Vector3.zero;
            playerRigidBody.isKinematic = true;
        }

        // Start smooth transition to sitting position
        StartCoroutine(SmoothSit());
    }

    IEnumerator SmoothSit()
    {
        isSitting = true;
        float duration = 0.5f;
        Vector3 startPos = playerMovement.transform.position;
        Quaternion startRot = playerMovement.transform.rotation;
        Vector3 endPos = chairPosition.position;

        // Rotate the player to face the screenCenterPoint to ensure it's centered in view
        Quaternion endRot = Quaternion.LookRotation(screenCenterPoint.position - chairPosition.position);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            playerMovement.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            playerMovement.transform.rotation = Quaternion.Lerp(startRot, endRot, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the player is exactly in place and facing the screen center
        playerMovement.transform.position = endPos;
        playerMovement.transform.rotation = endRot;

        // Start the video once seated
        StartVideo();
    }

    void OnFirstVideoFinished(VideoPlayer vp)
    {
        // Hide the first video and start the second video
        Debug.Log("First video finished. Starting second video.");

        // Disable the first VideoPlayer and enable the second one
        computerVideoPlayer.enabled = false;
        secondVideoPlayer.enabled = true;
        secondVideoPlayer.Play();
    }


    void StartVideo()
    {
        if (computerVideoPlayer != null)
        {
            computerVideoPlayer.Play();
            Debug.Log("Video starting");
        }
        else
        {
            Debug.LogWarning("No video assigned to the computer screen.");
        }
    }

    void ShowCluePopup()
    {
        popupMessage.SetActive(true);
        LockPlayerControls();
        UnlockCursor();
    }

    void ClosePopup()
    {
        popupMessage.SetActive(false);
        UnlockPlayerControls();
        LockCursor();
    }

    void LockPlayerControls()
    {
        playerMovement.enabled = false;
        playerLook.enabled = false;
    }

    void UnlockPlayerControls()
    {
        playerMovement.enabled = true;
        playerLook.enabled = true;

        // Reactivate player's collider and Rigidbody
        playerCollider.enabled = true;
        if (playerRigidBody != null)
        {
            playerRigidBody.isKinematic = false;
        }

        isSitting = false;
        Debug.Log("Player stood up");
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void CheckResumeInput()
    {
        if (hasPausedAtTimestamp)
        {
            Debug.Log("Waiting for V key press to resume video...");
            if (Input.GetKeyDown(KeyCode.V))
            {
                Debug.Log("V key detected - attempting to resume video.");
                ResumeVideo();
            }
        }
    }

    void CheckForPausePoint()
    {
        if (computerVideoPlayer.time >= pauseTimestamp)
        {
            PauseVideo();
        }
    }

    void PauseVideo()
    {
        computerVideoPlayer.Pause();
        hasPausedAtTimestamp = true;
        isVideoResumed = false; // Reset resume flag for next pause-resume cycle
        Debug.Log("Video paused. Press 'V' to continue.");
    }

    void ResumeVideo()
    {
        computerVideoPlayer.Play();
        hasPausedAtTimestamp = false; // Reset pause flag
        isVideoResumed = true; // Set resume flag to bypass future pause checks
        Debug.Log("Video resumed.");
    }

    void OnSecondVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Second video finished. Player will stand up.");
        StandUp();
    }

    void StandUp()
    {
        // Enable movement and look controls
        UnlockPlayerControls();

        // Show the crosshair again
        if (crosshair != null)
        {
            crosshair.SetActive(true);
        }

        // Activate the hidden object
        if (hiddenObject != null)
        {
            hiddenObject.SetActive(true);
            Debug.Log("Hidden object is now visible.");
        }

        UnlockCursor();
        Debug.Log("Player has stood up and can move again.");
    }
}
