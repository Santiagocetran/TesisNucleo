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
    public GameObject nextLevelTrigger; // GameObject that sends the player to the next level
    public GameObject sitDownSign; // UI sign for "Press E to sit down"

    private bool isNearChair = false;
    private bool isSitting = false;
    private FirstPersonMovement movementScript;
    private FirstPersonLook lookScript;
    private Rigidbody playerRigidBody;

    // Start is called before the first frame update
    void Start()
    {
        movementScript = player.GetComponent<FirstPersonMovement>();
        lookScript = player.GetComponentInChildren<FirstPersonLook>();
        playerRigidBody = player.GetComponent<Rigidbody>();

        // Subscribe to the movie end event
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnMovieEnd;
        }

        // Ensure the next level trigger and sitDownSign are disabled at the start
        if (nextLevelTrigger != null)
        {
            nextLevelTrigger.SetActive(false);
        }

        if (sitDownSign != null)
        {
            sitDownSign.SetActive(false); // Ensure the sign is hidden at the start
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is near the chair and presses E
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

        // Hide the "Press E to sit down" sign when the player sits
        if (sitDownSign != null)
        {
            sitDownSign.SetActive(false);
        }

        StartCoroutine(SmoothSit());
    }

    IEnumerator SmoothSit()
    {
        float duration = 0.5f;
        Vector3 startPos = player.transform.position;
        Quaternion startRot = player.transform.rotation;
        Vector3 endPos = sittingPosition.position;
        Quaternion endRot = Quaternion.LookRotation(lookAtTarget.position - sittingPosition.position);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            player.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            player.transform.rotation = Quaternion.Lerp(startRot, endRot, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        player.transform.position = endPos;
        player.transform.rotation = endRot;

        isSitting = true;
        StartMovie();
    }

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

    void StandPlayer()
    {
        movementScript.enabled = true;
        lookScript.enabled = true;

        if (playerRigidBody != null)
        {
            playerRigidBody.isKinematic = false;
        }

        isSitting = false;
        Debug.Log("Player stood up");
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

            // Show the "Press E to sit down" sign when the player is near the chair
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

            // Hide the sign when the player moves away from the chair
            if (sitDownSign != null)
            {
                sitDownSign.SetActive(false);
            }
        }
    }
}
