using UnityEngine;

public class ThirdPerson2DCamera : MonoBehaviour
{
    public Transform target; // El objeto que la c�mara seguir�
    public float smoothSpeed = 0.125f; // Velocidad de suavizado del movimiento
    public Vector3 offset; // Distancia entre la c�mara y el objetivo

    void LateUpdate()
    {
        if (target == null)
            return;

        // Calcula la posici�n deseada de la c�mara
        Vector3 desiredPosition = target.position + offset;

        // Suaviza el movimiento de la c�mara
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Actualiza la posici�n de la c�mara
        transform.position = smoothedPosition;

        // Opcional: Hacer que la c�mara mire siempre al objetivo
        // transform.LookAt(target);
    }
}