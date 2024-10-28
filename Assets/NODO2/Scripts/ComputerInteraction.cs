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

    private bool isSitting = false;

    void Start()
    {
        pressEText.SetActive(false); // Hide "Press E" prompt initially
        popupMessage.SetActive(false); // Hide popup message initially

        closeButton.onClick.AddListener(ClosePopup);
        LockCursor();
    }

    void Update()
    {
        if (!isSitting) // Only allow interaction when not sitting
        {
            RaycastForComputer();
            HandleComputerInteraction();
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
}
