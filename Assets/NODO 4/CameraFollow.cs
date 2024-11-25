using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // El personaje a seguir
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 0, 0); // Adjust as needed



    void LateUpdate()
    {
        // Calcula la nueva posición de la cámara
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}

