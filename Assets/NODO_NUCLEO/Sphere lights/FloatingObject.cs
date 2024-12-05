using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    [Header("Float Settings")]
    [SerializeField] private float amplitude = 0.2f;    // How high it floats
    [SerializeField] private float frequency = 0.5f;    // How fast it floats
    [SerializeField] private float returnSpeed = 2f;    // How fast it returns to floating position

    private Vector3 startPosition;
    private Vector3 floatingPosition;
    private float randomOffset;
    private Rigidbody rb;
    private bool isHit = false;
    private float hitRecoveryTimer = 0f;
    private float recoveryDelay = 2f;  // Time before object starts returning after being hit

    private void Start()
    {
        startPosition = transform.position;
        randomOffset = Random.Range(0f, 2f * Mathf.PI);
        rb = GetComponent<Rigidbody>();

        // Configure Rigidbody for physics interaction
        rb.useGravity = false;
        rb.drag = 1f;          // Air resistance
        rb.angularDrag = 1f;   // Rotational resistance
        rb.mass = 1f;          // Adjust mass to change how easily it's pushed
    }

    private void FixedUpdate()
    {
        if (isHit)
        {
            hitRecoveryTimer += Time.fixedDeltaTime;

            // Start returning to floating position after delay
            if (hitRecoveryTimer >= recoveryDelay)
            {
                Vector3 targetPosition = GetFloatingPosition();
                Vector3 returnDirection = (targetPosition - rb.position);

                // If close enough to floating position, resume normal floating
                if (returnDirection.magnitude < 0.1f)
                {
                    isHit = false;
                    hitRecoveryTimer = 0f;
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
                else
                {
                    // Gradually move back to floating position
                    rb.velocity = Vector3.Lerp(rb.velocity, returnDirection * returnSpeed, Time.fixedDeltaTime);
                }
            }
        }
        else
        {
            // Normal floating behavior
            float floatEffect = amplitude * Mathf.Sin(Time.time * frequency + randomOffset);
            Vector3 targetPosition = new Vector3(
                startPosition.x,
                startPosition.y + floatEffect,
                startPosition.z
            );

            rb.MovePosition(targetPosition);
        }
    }

    private Vector3 GetFloatingPosition()
    {
        float floatEffect = amplitude * Mathf.Sin(Time.time * frequency + randomOffset);
        return new Vector3(
            startPosition.x,
            startPosition.y + floatEffect,
            startPosition.z
        );
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if hit by player or other objects
        if (collision.gameObject.CompareTag("Player") || collision.relativeVelocity.magnitude > 1f)
        {
            isHit = true;
            hitRecoveryTimer = 0f;
        }
    }
}