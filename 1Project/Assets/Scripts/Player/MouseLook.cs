using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    private float xRotation = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        // ѕоворот камеры (вертикаль)
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // ѕоворот тела игрока (горизонталь) - только если тело существует
        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }

    public float GetXRotation() => xRotation;
    public void SetXRotation(float value)
    {
        xRotation = value;
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}