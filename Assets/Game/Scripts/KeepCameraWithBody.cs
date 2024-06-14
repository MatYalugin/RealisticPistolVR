using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepCameraWithBody : MonoBehaviour
{
    public Transform playerCamera;
    public float yOffset;
    private Vector3 initialRotation;
    public Collider playerCollider;
    void Start()
    {
        initialRotation = gameObject.transform.rotation.eulerAngles;
    }
    void Update()
    {
        if (playerCamera != null)
        {
            // Получение позиции целевого объекта
            Vector3 targetPosition = playerCamera.position;

            // Установка позиции текущего объекта ниже целевого объекта с учетом yOffset
            transform.position = new Vector3(targetPosition.x, targetPosition.y + yOffset, targetPosition.z);

            Vector3 currentRotation = gameObject.transform.rotation.eulerAngles;

            gameObject.transform.rotation = Quaternion.Euler(playerCamera.transform.rotation.eulerAngles.x, playerCamera.transform.rotation.eulerAngles.y, playerCamera.transform.rotation.eulerAngles.z);

            gameObject.transform.rotation = Quaternion.Euler(initialRotation.x, gameObject.transform.rotation.eulerAngles.y, initialRotation.z);
        }
    }
}
