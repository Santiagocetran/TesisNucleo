using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SceneTransitionPopup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private Button goBackButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private GameObject sceneTrigger;
    [SerializeField] private GameObject sceneButton;

    [Header("Interaction Settings")]
    [SerializeField] private GameObject interactText;
    [SerializeField] private float maxInteractDistance = 3f; // Maximum distance for interaction

    [Header("Optional References")]
    [SerializeField] private MonoBehaviour playerMovementScript;
    [SerializeField] private MonoBehaviour playerCameraScript;

    private bool isPopupActive = false;
    private bool isLookingAtButton = false;

    public static bool IsPopupOpen { get; private set; }

    private void Awake()
    {
        popupPanel.SetActive(false);
        interactText.SetActive(false);
        IsPopupOpen = false;

        if (sceneTrigger != null)
            sceneTrigger.SetActive(false);
    }

    private void Start()
    {
        if (goBackButton != null)
            goBackButton.onClick.AddListener(ClosePopup);

        if (continueButton != null)
            continueButton.onClick.AddListener(ContinueToNextScene);
    }

    private void Update()
    {
        if (!IsPopupOpen)
        {
            CheckButtonRaycast();

            if (isLookingAtButton && Input.GetKeyDown(KeyCode.E) && !isPopupActive)
            {
                ShowPopup();
            }
        }
    }

    private void CheckButtonRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Check if we hit the button AND we're within the interaction distance
            if (hit.collider.gameObject == sceneButton && hit.distance <= maxInteractDistance)
            {
                if (!isLookingAtButton)
                {
                    isLookingAtButton = true;
                    interactText.SetActive(true);
                }
            }
            else
            {
                if (isLookingAtButton)
                {
                    isLookingAtButton = false;
                    interactText.SetActive(false);
                }
            }
        }
        else
        {
            if (isLookingAtButton)
            {
                isLookingAtButton = false;
                interactText.SetActive(false);
            }
        }
    }

    private void ShowPopup()
    {
        isPopupActive = true;
        IsPopupOpen = true;
        popupPanel.SetActive(true);
        interactText.SetActive(false);

        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        if (playerCameraScript != null)
            playerCameraScript.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ClosePopup()
    {
        isPopupActive = false;
        IsPopupOpen = false;
        popupPanel.SetActive(false);

        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        if (playerCameraScript != null)
            playerCameraScript.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void ContinueToNextScene()
    {
        if (sceneTrigger != null)
            sceneTrigger.SetActive(true);

        ClosePopup();
    }

    private void OnDestroy()
    {
        if (goBackButton != null)
            goBackButton.onClick.RemoveListener(ClosePopup);

        if (continueButton != null)
            continueButton.onClick.RemoveListener(ContinueToNextScene);

        IsPopupOpen = false;
    }
}