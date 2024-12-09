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

    public AudioSource AudioSource;

    private bool hasCompletedVideos = false;

    public Transform screenTransform; // Reference to the computer screen's transform
    [Header("Camera View Settings")]
    public float sittingFOV = 60f; // FOV when sitting at computer
    public float defaultFOV; // Will store the original camera FOV
    public Vector3 cameraPositionOffset = new Vector3(0, 0, 0); // Fine-tune camera position when sitting
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;

    [Header("Screen Adjustment")]
    public float viewportPercentage = 0.9f; // How much of the viewport the screen should occupy
    public bool adjustForResolution = true; // Toggle resolution adjustment


    void Start()
    {
        pressEText.SetActive(false); // Hide "Press E" prompt initially
        popupMessage.SetActive(false); // Hide popup message initially

        hasCompletedVideos = false; // Initialize the flag

        closeButton.onClick.AddListener(ClosePopup);
        LockCursor();

        // Subscribe to the end of the first video to start the second video
        if (computerVideoPlayer != null && secondVideoPlayer != null)
        {
            computerVideoPlayer.loopPointReached += OnFirstVideoFinished;
            secondVideoPlayer.enabled = false; // Disable second video initially

            // Subscribe to the end of the second video to make the player stand up
            secondVideoPlayer.loopPointReached += OnSecondVideoFinished;

            if (playerCamera != null)
            {
                defaultFOV = playerCamera.fieldOfView;
                originalCameraPosition = playerCamera.transform.localPosition;
                originalCameraRotation = playerCamera.transform.localRotation;
            }
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

            // Only show the "Press E" text if videos haven't been completed
            if (hitComputer != currentComputer && !hasCompletedVideos)
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
        // Only allow interaction if videos haven't been completed
        if (Input.GetKeyDown(KeyCode.E) && currentComputer != null && !hasCompletedVideos)
        {
            if (cardHighlight.hasCard)
            {
                SitAndInteractWithComputer();
                if (AudioSource != null && AudioSource.isPlaying)
                {
                    AudioSource.Stop();
                }
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

        // Calculate the optimal position to view the screen
        Vector3 screenCenter = screenCenterPoint.position;
        Vector3 directionToScreen = (screenCenter - chairPosition.position).normalized;
        Quaternion endRot = Quaternion.LookRotation(directionToScreen);

        // Store camera start positions
        Vector3 cameraStartPos = playerCamera.transform.localPosition;
        Quaternion cameraStartRot = playerCamera.transform.localRotation;
        float startFOV = playerCamera.fieldOfView;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            // Move player
            playerMovement.transform.position = Vector3.Lerp(startPos, endPos, t);
            playerMovement.transform.rotation = Quaternion.Lerp(startRot, endRot, t);

            // Adjust camera
            playerCamera.transform.localPosition = Vector3.Lerp(cameraStartPos, cameraPositionOffset, t);
            playerCamera.transform.localRotation = Quaternion.Lerp(cameraStartRot, Quaternion.identity, t);
            playerCamera.fieldOfView = Mathf.Lerp(startFOV, sittingFOV, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final position is exact
        playerMovement.transform.position = endPos;
        playerMovement.transform.rotation = endRot;
        playerCamera.transform.localPosition = cameraPositionOffset;
        playerCamera.transform.localRotation = Quaternion.identity;
        playerCamera.fieldOfView = sittingFOV;

        if (adjustForResolution)
        {
            AdjustViewForScreenResolution();
        }

        // Start the video once seated
        StartVideo();
    }

    void AdjustViewForScreenResolution()
    {
        if (screenTransform == null || playerCamera == null) return;

        // Get screen bounds in world space
        Bounds screenBounds = new Bounds();
        if (screenTransform.TryGetComponent<Renderer>(out var renderer))
        {
            screenBounds = renderer.bounds;
        }
        else if (screenTransform.TryGetComponent<RectTransform>(out var rectTransform))
        {
            // Handle UI elements
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            screenBounds = new Bounds(rectTransform.position, Vector3.zero);
            foreach (Vector3 corner in corners)
            {
                screenBounds.Encapsulate(corner);
            }
        }

        // Calculate the optimal FOV to fit the screen
        float screenWidth = screenBounds.size.x;
        float screenHeight = screenBounds.size.y;
        float screenAspect = screenWidth / screenHeight;
        float distanceToScreen = Vector3.Distance(playerCamera.transform.position, screenBounds.center);

        // Adjust FOV based on screen aspect ratio and desired viewport coverage
        float targetFOV = 2.0f * Mathf.Atan((screenHeight * 0.5f) / distanceToScreen) * Mathf.Rad2Deg;
        targetFOV /= viewportPercentage; // Adjust for desired viewport coverage

        // Apply the calculated FOV
        playerCamera.fieldOfView = targetFOV;
    }

    IEnumerator StandUpSmoothly()
    {
        float duration = 0.5f;
        float startFOV = playerCamera.fieldOfView;
        Vector3 cameraStartPos = playerCamera.transform.localPosition;
        Quaternion cameraStartRot = playerCamera.transform.localRotation;

        // Calculate a safe standing position slightly in front of the chair
        Vector3 standingPosition = chairPosition.position + chairPosition.forward * -0.5f; // Step back from chair
        standingPosition.y = chairPosition.position.y; // Maintain the same height as chair

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            // Reset camera
            playerCamera.transform.localPosition = Vector3.Lerp(cameraStartPos, originalCameraPosition, t);
            playerCamera.transform.localRotation = Quaternion.Lerp(cameraStartRot, originalCameraRotation, t);
            playerCamera.fieldOfView = Mathf.Lerp(startFOV, defaultFOV, t);

            // Move player to standing position
            playerMovement.transform.position = Vector3.Lerp(playerMovement.transform.position, standingPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final position is exact
        playerCamera.transform.localPosition = originalCameraPosition;
        playerCamera.transform.localRotation = originalCameraRotation;
        playerCamera.fieldOfView = defaultFOV;
        playerMovement.transform.position = standingPosition;

        // Enable physics after positioning is complete
        UnlockPlayerControls();
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
        // First enable the collider so the ground check works
        playerCollider.enabled = true;

        // Reset and enable rigidbody
        if (playerRigidBody != null)
        {
            playerRigidBody.isKinematic = false;
            playerRigidBody.velocity = Vector3.zero; // Reset any velocity
        }

        // Do a ground check
        bool isGrounded = Physics.Raycast(
            playerMovement.transform.position + Vector3.up * 0.1f,
            Vector3.down,
            0.2f
        );

        if (!isGrounded)
        {
            // If not grounded, adjust Y position to ground
            RaycastHit hit;
            if (Physics.Raycast(playerMovement.transform.position + Vector3.up, Vector3.down, out hit))
            {
                Vector3 newPos = playerMovement.transform.position;
                newPos.y = hit.point.y;
                playerMovement.transform.position = newPos;
            }
        }

        // Now enable movement scripts
        playerMovement.enabled = true;
        playerLook.enabled = true;

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
        hasCompletedVideos = true; // Set the flag when videos are completed
        StandUp();
    }

    void StandUp()
    {
        StartCoroutine(StandUpSmoothly());

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
