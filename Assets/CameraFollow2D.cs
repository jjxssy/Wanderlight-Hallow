using UnityEngine;

[DisallowMultipleComponent]
public class CameraFollow2D : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // Drag your Player here in the Inspector

    [Header("Follow Settings")]
    public Vector3 offset = new Vector3(0f, 0f, -10f); // Keep Z = -10 for 2D
    public float smoothTime = 0.12f; // Lower = snappier, higher = smoother

    [Header("Clamp Settings")]
    public bool clampToBounds = true;
    // World bounds in world units: bottom-left (x,y) and size (width, height)
    public Rect worldBounds = new Rect(-50f, -30f, 100f, 60f);

    private Vector3 velocity = Vector3.zero;
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam != null) cam.orthographic = true; // Ensure orthographic for 2D
    }

    void LateUpdate()
    {
        if (!target) return;

        // Desired position based on player + offset
        Vector3 targetPosition = target.position + offset;

        if (clampToBounds && cam != null)
        {
            // Calculate half camera size in world units
            float camHalfHeight = cam.orthographicSize;
            float camHalfWidth = camHalfHeight * cam.aspect;

            // Clamp X and Y so camera stays inside worldBounds
            float minX = worldBounds.xMin + camHalfWidth;
            float maxX = worldBounds.xMax - camHalfWidth;
            float minY = worldBounds.yMin + camHalfHeight;
            float maxY = worldBounds.yMax - camHalfHeight;

            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }

        // Smoothly move the camera
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
