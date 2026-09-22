using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;          // Drag your Witch/Necromancer model here
    public float distance = 3.5f;     // Distance behind the character
    public float height = 1.5f;       // Height above the character feet
    public float sensitivity = 3.0f;  // Orbit sensitivity

    private float currentX = 0.0f;
    private float currentY = 15.0f;   // Slight upward angle

    void Update()
    {
        // Orbit camera when holding Right Mouse Button
        if (Input.GetMouseButton(1))
        {
            currentX += Input.GetAxis("Mouse X") * sensitivity;
            currentY -= Input.GetAxis("Mouse Y") * sensitivity;
            currentY = Mathf.Clamp(currentY, -10f, 60f); // Prevents camera from flipping
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate position behind the character based on current rotation angle
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 targetPosition = target.position + Vector3.up * height;
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        
        // Update camera position to follow character continuously
        transform.position = targetPosition + (rotation * negDistance);
        transform.rotation = rotation;
    }
}