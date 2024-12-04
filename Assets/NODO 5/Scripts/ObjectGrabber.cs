using UnityEngine;

public class ObjectGrabber : MonoBehaviour
{
    [Header("Grab Settings")]
    public float grabDistance = 3f;
    public LayerMask grabbableLayer;
    public LayerMask collisionLayer; // Layer mask for walls/floors
    public float holdDistance = 2f;
    public float slideSpeed = 5f;

    [Header("Highlight Settings")]
    public Color highlightColor = Color.yellow;
    public float highlightIntensity = 0.5f;

    private Camera playerCamera;
    private GameObject heldObject;
    private Rigidbody heldRigidbody;
    private GameObject lastHighlightedObject;
    private Material[] originalMaterials;
    private Vector3 lastValidPosition;
    private bool isSliding;

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera == null)
            Debug.LogError("No camera found on player!");
    }

    void Update()
    {
        CheckForHighlight();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null)
                TryGrabObject();
            else
                DropObject();
        }

        if (heldObject != null)
        {
            UpdateHeldObjectPosition();
        }
    }

    void CheckForHighlight()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, grabDistance, grabbableLayer))
        {
            GameObject targetObject = GetGrabbableParent(hit.collider.gameObject);

            if (lastHighlightedObject != targetObject)
            {
                RemoveHighlight();
                lastHighlightedObject = targetObject;
                AddHighlight(lastHighlightedObject);
            }
        }
        else
        {
            RemoveHighlight();
        }
    }

    void UpdateHeldObjectPosition()
    {
        Vector3 targetPosition = CalculateTargetPosition();
        Vector3 currentPosition = heldObject.transform.position;

        // Check if moving to target would cause collision
        Vector3 moveDirection = (targetPosition - currentPosition).normalized;
        float moveDistance = Vector3.Distance(currentPosition, targetPosition);

        // Cast a ray from current position toward target
        RaycastHit[] hits = Physics.RaycastAll(
            currentPosition,
            moveDirection,
            moveDistance,
            collisionLayer
        );

        if (hits.Length > 0)
        {
            // Sort hits by distance
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (RaycastHit hit in hits)
            {
                // Ignore collisions with held object
                if (hit.collider.gameObject != heldObject)
                {
                    // Calculate slide direction along the wall
                    Vector3 slideDirection = Vector3.ProjectOnPlane(moveDirection, hit.normal).normalized;

                    // If we can slide
                    if (slideDirection.magnitude > 0.01f)
                    {
                        // Try to slide along the wall
                        Vector3 slideTarget = hit.point + (slideDirection * moveDistance * 0.5f) + (hit.normal * 0.1f);

                        // Check if sliding position is clear
                        if (!Physics.Raycast(hit.point, slideDirection, moveDistance * 0.5f, collisionLayer))
                        {
                            targetPosition = slideTarget;
                            isSliding = true;
                        }
                        else
                        {
                            // Can't slide, stay at last position
                            targetPosition = lastValidPosition;
                        }
                    }
                    else
                    {
                        // Can't slide, stay at last position
                        targetPosition = lastValidPosition;
                    }
                    break;
                }
            }
        }
        else
        {
            isSliding = false;
            // Store position as valid if no collisions
            lastValidPosition = targetPosition;
        }

        // Move object smoothly to target position
        float speed = isSliding ? slideSpeed : 10f;
        heldObject.transform.position = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime * speed);

        // Check if object got too far (through a wall)
        float distanceToPlayer = Vector3.Distance(playerCamera.transform.position, heldObject.transform.position);
        if (distanceToPlayer > holdDistance * 1.5f)
        {
            // Force object to last valid position and drop it
            heldObject.transform.position = lastValidPosition;
            DropObject();
        }
    }

    Vector3 CalculateTargetPosition()
    {
        // Calculate desired hold position in front of camera
        Vector3 holdPosition = playerCamera.transform.position + playerCamera.transform.forward * holdDistance;

        // Check for obstacles between camera and hold position
        RaycastHit hit;
        if (Physics.Linecast(playerCamera.transform.position, holdPosition, out hit, collisionLayer))
        {
            // If there's an obstacle, place the object just in front of it
            return hit.point - (playerCamera.transform.forward * 0.2f);
        }

        return holdPosition;
    }

    GameObject GetGrabbableParent(GameObject hitObject)
    {
        Transform current = hitObject.transform;

        // Keep going up the hierarchy until we find an object with a Rigidbody
        // or until we reach the root
        while (current.parent != null)
        {
            if (current.GetComponent<Rigidbody>() != null)
                return current.gameObject;
            current = current.parent;
        }

        // If we didn't find a parent with Rigidbody, return the top-most parent
        return current.gameObject;
    }

    void AddHighlight(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].material;
            Material highlightMaterial = new Material(originalMaterials[i]);
            highlightMaterial.EnableKeyword("_EMISSION");
            highlightMaterial.SetColor("_EmissionColor", highlightColor * highlightIntensity);
            renderers[i].material = highlightMaterial;
        }
    }

    void RemoveHighlight()
    {
        if (lastHighlightedObject != null && originalMaterials != null)
        {
            Renderer[] renderers = lastHighlightedObject.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                if (i < originalMaterials.Length)
                    renderers[i].material = originalMaterials[i];
            }
        }
        lastHighlightedObject = null;
        originalMaterials = null;
    }

    void TryGrabObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, grabDistance, grabbableLayer))
        {
            GameObject targetObject = GetGrabbableParent(hit.collider.gameObject);

            GrabbableObject grabbable = targetObject.GetComponent<GrabbableObject>();
            if (grabbable != null)
            {
                if (grabbable.isCorrectlyPlaced)
                    return;

                // Show popup when object is grabbed
                grabbable.ShowObjectPopup();

                // Deactivate particles when grabbed
                grabbable.SetParticleEffect(false);
            }

            heldObject = targetObject;
            heldRigidbody = heldObject.GetComponent<Rigidbody>();
            lastValidPosition = heldObject.transform.position;

            if (heldRigidbody != null)
            {
                heldRigidbody.useGravity = false;
                heldRigidbody.isKinematic = true;
                heldRigidbody.detectCollisions = true;
            }
        }
    }

    void DropObject()
    {
        if (heldObject != null)
        {
            // Reactivate particles when dropped (only if not correctly placed)
            GrabbableObject grabbable = heldObject.GetComponent<GrabbableObject>();
            if (grabbable != null && !grabbable.isCorrectlyPlaced)
            {
                grabbable.SetParticleEffect(true);
            }

            // Rest of your existing drop code...
            if (heldRigidbody != null)
            {
                heldObject.transform.position = lastValidPosition;
                heldRigidbody.useGravity = true;
                heldRigidbody.isKinematic = false;

                Collider objectCollider = heldObject.GetComponent<Collider>();
                if (objectCollider != null)
                {
                    Collider[] overlapping = Physics.OverlapBox(
                        objectCollider.bounds.center,
                        objectCollider.bounds.extents,
                        heldObject.transform.rotation
                    );

                    if (overlapping.Length > 1)
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(heldObject.transform.position + Vector3.up * 2f, Vector3.down, out hit))
                        {
                            heldObject.transform.position = hit.point + Vector3.up * 0.1f;
                        }
                    }
                }
            }

            heldObject = null;
            heldRigidbody = null;
        }
    }
}