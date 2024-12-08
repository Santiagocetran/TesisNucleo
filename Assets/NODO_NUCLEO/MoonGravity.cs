using UnityEngine;

public class MoonGravity : MonoBehaviour
{
    private void Start()
    {
        // This will change gravity for ALL objects in the scene
        // Moon's gravity is approximately 1.62 m/s²
        Physics.gravity = new Vector3(0, -1.62f, 0);

        // You can adjust this value:
        // Higher value (like -3) = stronger gravity
        // Lower value (like -1) = weaker gravity
    }
}