using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[DisallowMultipleComponent, RequireComponent(typeof(Camera))]
public class CameraFollow2D_AutoBoundsV2 : MonoBehaviour
{
    [Header("Target")]
    public Transform target;                           // Drag the Player here

    [Header("Follow")]
    public Vector3 offset = new Vector3(0, 0, -10);    // Keep Z = -10 for 2D
    [Tooltip("Smooth time when close to the target.")]
    [Range(0.01f, 0.5f)] public float smoothTime = 0.12f;
    [Tooltip("If we're farther than this, use fast catch-up.")]
    public float fastDistance = 5f;
    [Tooltip("Max speed used during catch-up (units/second).")]
    public float maxSpeed = 40f;

    [Header("Bounds / Clamp")]
    public bool clampToBounds = true;
    public bool autoDetectBounds = true;
    [Tooltip("Extra padding inside bounds (world units).")]
    public float boundsPadding = 0.5f;
    [Tooltip("Optional parent to search under (e.g., Grid). If null, searches whole scene.")]
    public Transform searchRoot;

    [Header("Manual Extras (optional)")]
    public List<Tilemap> extraTilemaps = new();
    public List<Renderer> extraRenderers = new();
    public List<Collider2D> extraColliders = new();

    [Header("Debug")]
    public Rect worldBounds; // computed if auto
    public Color gizmoColor = new Color(0, 1, 0, 0.25f);

    Camera cam;
    Vector3 vel;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true; // 2D
        if (autoDetectBounds) RefreshWorldBounds();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!cam) cam = GetComponent<Camera>();
        if (cam) cam.orthographic = true;
        if (!Application.isPlaying && autoDetectBounds) RefreshWorldBounds();
    }
#endif

    void LateUpdate()
    {
        if (!target) return;

        // Desired position before clamp
        Vector3 desired = target.position + offset;

        if (clampToBounds)
        {
            float halfH = cam.orthographicSize;
            float halfW = halfH * cam.aspect;

            // Apply padding to bounds
            Rect pad = new Rect(
                worldBounds.x + boundsPadding,
                worldBounds.y + boundsPadding,
                worldBounds.width  - 2f * boundsPadding,
                worldBounds.height - 2f * boundsPadding
            );

            float minX = pad.xMin + halfW;
            float maxX = pad.xMax - halfW;
            float minY = pad.yMin + halfH;
            float maxY = pad.yMax - halfH;

            // If the camera is larger than the map on an axis, lock to center on that axis
            if (minX > maxX) desired.x = (pad.xMin + pad.xMax) * 0.5f;
            else desired.x = Mathf.Clamp(desired.x, minX, maxX);

            if (minY > maxY) desired.y = (pad.yMin + pad.yMax) * 0.5f;
            else desired.y = Mathf.Clamp(desired.y, minY, maxY);
        }

        // Distance on X/Y plane for deciding follow mode
        Vector2 d = (Vector2)(desired - transform.position);
        float planarDist = d.magnitude;

        Vector3 next;
        if (planarDist > fastDistance)
        {
            // FAST catch-up when far
            next = Vector3.MoveTowards(transform.position, desired, maxSpeed * Time.deltaTime);
        }
        else
        {
            // Smooth and pretty when close
            next = Vector3.SmoothDamp(transform.position, desired, ref vel, smoothTime);
        }

        transform.position = next;
    }

    [ContextMenu("Refresh World Bounds")]
    public void RefreshWorldBounds()
    {
        Bounds? merged = null;

        // Search roots
        var roots = new List<Transform>();
        if (searchRoot) roots.Add(searchRoot);
        else
        {
            foreach (var go in FindObjectsOfType<GameObject>())
                if (go.scene.IsValid() && go.activeInHierarchy) roots.Add(go.transform);
        }

        // Tilemaps
        var seenTms = new HashSet<Tilemap>(extraTilemaps);
        foreach (var r in roots)
            foreach (var tm in r.GetComponentsInChildren<Tilemap>(true))
                seenTms.Add(tm);

        foreach (var tm in seenTms)
        {
            var rend = tm.GetComponent<Renderer>();
            if (rend) Merge(ref merged, rend.bounds);
            else Merge(ref merged, tm.localBounds); // local fallback
        }

        // Renderers
        foreach (var rd in extraRenderers)
            if (rd) Merge(ref merged, rd.bounds);

        // Colliders
        foreach (var col in extraColliders)
            if (col) Merge(ref merged, col.bounds);

        if (merged == null)
        {
            worldBounds = new Rect(-50f, -30f, 100f, 60f);
        }
        else
        {
            var b = merged.Value;
            // If any localBounds slipped in, they won’t be world; but renderers/colliders cover most cases.
            worldBounds = Rect.MinMaxRect(b.min.x, b.min.y, b.max.x, b.max.y);
        }
    }

    static void Merge(ref Bounds? acc, Bounds add)
    {
        if (acc == null) acc = add;
        else { var b = acc.Value; b.Encapsulate(add); acc = b; }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Vector3 c = new Vector3(worldBounds.center.x, worldBounds.center.y, 0f);
        Vector3 s = new Vector3(worldBounds.width, worldBounds.height, 0f);
        Gizmos.DrawCube(c, s);
    }
}
