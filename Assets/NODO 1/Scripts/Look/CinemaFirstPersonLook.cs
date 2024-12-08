using UnityEngine;

public class CinemaFirstPersonLook : MonoBehaviour
{
    [SerializeField]
    Transform character;
    public float sensitivityX = 2;
    public float sensitivityY = 2;

    private float rotationX = 0;
    private float rotationY = 0;
    private Quaternion lastCameraRotation;
    private Quaternion lastCharacterRotation;

    private TicketBoothInteraction ticketBooth;

    void Reset()
    {
        character = GetComponentInParent<FirstPersonMovement>().transform;
    }

    void Start()
    {
        rotationX = character.localEulerAngles.y;
        rotationY = transform.localEulerAngles.x;

        lastCameraRotation = transform.localRotation;
        lastCharacterRotation = character.localRotation;

        ticketBooth = FindObjectOfType<TicketBoothInteraction>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (ticketBooth != null && ticketBooth.enabled && ticketBooth.IsInteracting())
        {
            transform.localRotation = lastCameraRotation;
            character.localRotation = lastCharacterRotation;
            return;
        }

        // Get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivityX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivityY;

        // Update rotations
        rotationX += mouseX;
        rotationY -= mouseY; // Inverted for proper camera movement
        rotationY = Mathf.Clamp(rotationY, -90f, 90f);

        // Apply rotations directly
        character.localRotation = Quaternion.Euler(0, rotationX, 0);
        transform.localRotation = Quaternion.Euler(rotationY, 0, 0);

        // Store current rotations
        lastCameraRotation = transform.localRotation;
        lastCharacterRotation = character.localRotation;
    }
}