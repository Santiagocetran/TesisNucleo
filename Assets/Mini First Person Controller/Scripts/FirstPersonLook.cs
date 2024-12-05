using UnityEngine;

public class FirstPersonLook : MonoBehaviour
{
    [SerializeField]
    Transform character;
    public float sensitivity = 2;
    public float smoothing = 1.5f;
    Vector2 velocity;
    Vector2 frameVelocity;

    // Store initial rotations
    private Vector3 initialCameraRotation;
    private Vector3 initialCharacterRotation;

    // Store last known rotation
    private Quaternion lastCameraRotation;
    private Quaternion lastCharacterRotation;

    void Reset()
    {
        character = GetComponentInParent<FirstPersonMovement>().transform;
    }

    void Start()
    {
        // Store initial rotations as euler angles
        initialCameraRotation = transform.localEulerAngles;
        initialCharacterRotation = character.localEulerAngles;

        // Set initial velocity based on the starting rotation
        velocity.x = initialCharacterRotation.y;
        velocity.y = -initialCameraRotation.x;

        // Initialize last known rotations
        lastCameraRotation = transform.localRotation;
        lastCharacterRotation = character.localRotation;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (PopupManager.IsPopupOpen)
        {
            // Force rotations to stay at their last position
            transform.localRotation = lastCameraRotation;
            character.localRotation = lastCharacterRotation;
            return;
        }

        // Get smooth velocity.
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        velocity += frameVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -90, 90);

        // Rotate camera up-down and controller left-right from velocity.
        transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        character.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);

        // Store current rotations
        lastCameraRotation = transform.localRotation;
        lastCharacterRotation = character.localRotation;
    }
}