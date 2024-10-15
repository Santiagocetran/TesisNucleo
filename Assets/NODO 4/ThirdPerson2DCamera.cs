using UnityEngine;

public class ThirdPerson2DCamera : MonoBehaviour
{
    public Transform target; // El objeto que la cámara seguirá
    public float smoothSpeed = 0.125f; // Velocidad de suavizado del movimiento
    public Vector3 offset; // Distancia entre la cámara y el objetivo

    void LateUpdate()
    {
        if (target == null)
            return;

        // Calcula la posición deseada de la cámara
        Vector3 desiredPosition = target.position + offset;

        // Suaviza el movimiento de la cámara
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Actualiza la posición de la cámara
        transform.position = smoothedPosition;

        // Opcional: Hacer que la cámara mire siempre al objetivo
        // transform.LookAt(target);
    }
}