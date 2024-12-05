using System.Collections;
using System.Collections.Generic;
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

    [Header("Camera Settings")]
    public Transform playerCamera; // Reference to the actual camera transform
    public float cameraTransitionSpeed = 2f; // Speed at which the camera rotates

    private bool isNearChair = false;
    private bool isSitting = false;
    private FirstPersonMovement movementScript;
    private FirstPersonLook lookScript;
    private Rigidbody playerRigidBody;
    private Vector3 originalCameraLocalPosition;
    private Quaternion originalCameraLocalRotation;

    void Start()
    {
        movementScript = player.GetComponent<FirstPersonMovement>();
        lookScript = player.GetComponentInChildren<FirstPersonLook>();
        playerRigidBody = player.GetComponent<Rigidbody>();

        // Store original camera transform relative to player
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
        if (isNearChair && Input.GetKeyDown(KeyCode.E) && !isSitting)
        {
            SitPlayer();
        }
    }

    void SitPlayer()
    {
        movementScript.enabled = false;
        lookScript.enabled = false;

        if (playerRigidBody != null)
        {
            playerRigidBody.velocity = Vector3.zero;
            playerRigidBody.isKinematic = true;
        }

        if (sitDownSign != null)
        {
            sitDownSign.SetActive(false);
        }

        StartCoroutine(SmoothSitWithCamera());
    }

    IEnumerator SmoothSitWithCamera()
    {
        float duration = 0.5f;
        float elapsedTime = 0f;

        // Starting positions and rotations
        Vector3 playerStartPos = player.transform.position;
        Quaternion playerStartRot = player.transform.rotation;
        Vector3 cameraStartPos = playerCamera.position;
        Quaternion cameraStartRot = playerCamera.rotation;

        // Calculate target rotations
        Vector3 directionToScreen = (lookAtTarget.position - sittingPosition.position).normalized;
        Quaternion targetPlayerRotation = Quaternion.LookRotation(directionToScreen);

        // Calculate the target camera position and rotation
        Vector3 targetCameraPosition = sittingPosition.position + originalCameraLocalPosition;
        Quaternion targetCameraRotation = Quaternion.LookRotation(lookAtTarget.position - targetCameraPosition);

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            // Smooth interpolation for player position and rotation
            player.transform.position = Vector3.Lerp(playerStartPos, sittingPosition.position, t);
            player.transform.rotation = Quaternion.Lerp(playerStartRot, targetPlayerRotation, t);

            // Smooth interpolation for camera
            playerCamera.position = Vector3.Lerp(cameraStartPos, targetCameraPosition, t);
            playerCamera.rotation = Quaternion.Lerp(cameraStartRot, targetCameraRotation, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final positions and rotations are exact
        player.transform.position = sittingPosition.position;
        player.transform.rotation = targetPlayerRotation;
        playerCamera.position = targetCameraPosition;
        playerCamera.rotation = targetCameraRotation;

        isSitting = true;
        StartMovie();
    }

    void StandPlayer()
    {
        StartCoroutine(SmoothStandWithCamera());
    }

    IEnumerator SmoothStandWithCamera()
    {
        float duration = 0.5f;
        float elapsedTime = 0f;

        Vector3 startCameraPos = playerCamera.position;
        Quaternion startCameraRot = playerCamera.rotation;

        // Reset to original camera local transform
        Vector3 targetCameraPosition = player.transform.TransformPoint(originalCameraLocalPosition);
        Quaternion targetCameraRotation = player.transform.rotation * originalCameraLocalRotation;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            // Smooth interpolation for camera return
            playerCamera.position = Vector3.Lerp(startCameraPos, targetCameraPosition, t);
            playerCamera.rotation = Quaternion.Lerp(startCameraRot, targetCameraRotation, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Re-enable player control
        movementScript.enabled = true;
        lookScript.enabled = true;

        if (playerRigidBody != null)
        {
            playerRigidBody.isKinematic = false;
        }

        isSitting = false;
        Debug.Log("Player stood up");
    }

    // Rest of the methods remain the same
    void StartMovie()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Play();
            Debug.Log("Movie starting");
        }
        else
        {
            Debug.LogWarning("No movie assigned");
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
            Debug.Log("Next level trigger activated");
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