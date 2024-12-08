using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class SitOnChair : MonoBehaviour
{
    public Transform sittingPosition;
    public GameObject player;
    public Transform lookAtTarget;
    public VideoPlayer videoPlayer;
    public GameObject nextLevelTrigger;
    public GameObject sitDownSign;
    public Transform playerCamera;
    public float transitionDuration = 0.5f;
    public float maxStepsPerFrame = 10f;

    private bool isNearChair = false;
    private bool isSitting = false;
    private bool isTransitioning = false;
    private FirstPersonMovement movementScript;
    private CinemaFirstPersonLook lookScript; // Changed to CinemaFirstPersonLook
    private Rigidbody playerRigidBody;
    private Vector3 originalCameraLocalPosition;
    private Quaternion originalCameraLocalRotation;

    void Start()
    {
        InitializeComponents();
    }

    void InitializeComponents()
    {
        movementScript = player.GetComponent<FirstPersonMovement>();
        // Changed to find CinemaFirstPersonLook on the camera
        lookScript = playerCamera.GetComponent<CinemaFirstPersonLook>();
        playerRigidBody = player.GetComponent<Rigidbody>();

        if (playerCamera != null)
        {
            originalCameraLocalPosition = playerCamera.localPosition;
            originalCameraLocalRotation = playerCamera.localRotation;
        }

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnMovieEnd;
        }

        if (nextLevelTrigger != null)
        {
            nextLevelTrigger.SetActive(false);
        }

        if (sitDownSign != null)
        {
            sitDownSign.SetActive(false);
        }
    }

    void Update()
    {
        if (isNearChair && Input.GetKeyDown(KeyCode.E) && !isSitting && !isTransitioning)
        {
            SitPlayer();
        }
    }

    void SitPlayer()
    {
        if (!isTransitioning)
        {
            DisablePlayerControls();
            StartCoroutine(SmoothTransition(true));
        }
    }

    void DisablePlayerControls()
    {
        if (movementScript != null)
        {
            movementScript.enabled = false;
        }

        if (lookScript != null)
        {
            lookScript.enabled = false;
        }

        if (playerRigidBody != null)
        {
            playerRigidBody.velocity = Vector3.zero;
            playerRigidBody.isKinematic = true;
        }

        if (sitDownSign != null)
        {
            sitDownSign.SetActive(false);
        }
    }

    // Rest of your methods remain the same...
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    void EnablePlayerControls()
    {
        if (movementScript != null)
        {
            movementScript.enabled = true;
        }

        if (lookScript != null)
        {
            lookScript.enabled = true;
        }

        if (playerRigidBody != null)
        {
            playerRigidBody.isKinematic = false;
        }

        this.isSitting = false;
    }

    // Your existing methods remain unchanged...
    IEnumerator SmoothTransition(bool isSitting)
    {
        isTransitioning = true;
        float elapsedTime = 0f;
        int steps = 0;

        Vector3 startPlayerPos = player.transform.position;
        Quaternion startPlayerRot = player.transform.rotation;
        Vector3 startCameraPos = playerCamera.position;
        Quaternion startCameraRot = playerCamera.rotation;

        Vector3 targetPlayerPos = isSitting ? sittingPosition.position : startPlayerPos;
        Vector3 directionToScreen = (lookAtTarget.position - sittingPosition.position).normalized;
        Quaternion targetPlayerRot = isSitting ?
            Quaternion.LookRotation(directionToScreen) :
            startPlayerRot;

        Vector3 targetCameraPos = isSitting ?
            sittingPosition.position + originalCameraLocalPosition :
            player.transform.TransformPoint(originalCameraLocalPosition);

        Quaternion targetCameraRot = isSitting ?
            Quaternion.LookRotation(lookAtTarget.position - targetCameraPos) :
            player.transform.rotation * originalCameraLocalRotation;

        while (elapsedTime < transitionDuration)
        {
            if (steps >= maxStepsPerFrame)
            {
                steps = 0;
                yield return new WaitForEndOfFrame();
            }

            float t = elapsedTime / transitionDuration;
            t = Mathf.SmoothStep(0, 1, t);

            player.transform.position = Vector3.Lerp(startPlayerPos, targetPlayerPos, t);
            player.transform.rotation = Quaternion.Lerp(startPlayerRot, targetPlayerRot, t);
            playerCamera.position = Vector3.Lerp(startCameraPos, targetCameraPos, t);
            playerCamera.rotation = Quaternion.Lerp(startCameraRot, targetCameraRot, t);

            elapsedTime += Time.deltaTime;
            steps++;

            yield return new WaitForSeconds(0.001f);
        }

        player.transform.position = targetPlayerPos;
        player.transform.rotation = targetPlayerRot;
        playerCamera.position = targetCameraPos;
        playerCamera.rotation = targetCameraRot;

        if (isSitting)
        {
            this.isSitting = true;
            StartMovie();
        }
        else
        {
            EnablePlayerControls();
        }

        isTransitioning = false;
    }

    void StandPlayer()
    {
        if (!isTransitioning)
        {
            StartCoroutine(SmoothTransition(false));
        }
    }

    void StartMovie()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Play();
            Debug.Log("Movie starting");
        }
    }

    void OnMovieEnd(VideoPlayer vp)
    {
        StandPlayer();
        ActivateNextLevelTrigger();
    }

    void ActivateNextLevelTrigger()
    {
        if (nextLevelTrigger != null)
        {
            nextLevelTrigger.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            isNearChair = true;
            if (!isSitting && sitDownSign != null)
            {
                sitDownSign.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            isNearChair = false;
            if (sitDownSign != null)
            {
                sitDownSign.SetActive(false);
            }
        }
    }
}