using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float smoothSpeed = 0.125f;

    public Transform spider; // Assign the spider GameObject's Transform here
    public Vector3 offset = new Vector3(5, 5, 10); // Offset for the camera position

    void LateUpdate()
{
    if (spider != null)
    {
        Vector3 desiredPosition = spider.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Optional: Make the camera look at the spider
        transform.LookAt(spider);
    }
}
}
