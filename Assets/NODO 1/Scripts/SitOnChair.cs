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
    }

    // Update is called once per frame
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
            Debug.Log("movie starting");
        }
       else
        {
            Debug.LogWarning("no movie");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            isNearChair = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            isNearChair = false;
        }
    }
}
