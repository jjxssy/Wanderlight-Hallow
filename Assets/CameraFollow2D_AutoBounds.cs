using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public class CameraFollow2D_AutoBounds : MonoBehaviour
{
    [Header("Target")]
    public Transform target;                          // Drag your Player here

    [Header("Follow")]
    public Vector3 offset = new Vector3(0f, 0f, -10f); // Keep Z = -10 for 2D
    [Range(0.01f, 0.5f)] public float smoothTime = 0.12f;

    [Header("Clamp Inside Map")]
    public bool clampToBounds = true;
    [Tooltip("Auto: read bounds from Tilemaps/Colliders at runtime & in editor.")]
    public bool autoDetectBounds = true;

    [Tooltip("Optional parent to search under (e.g., your Grid). If empty, searches whole scene (active objects).")]
    public Transform searchRoot;

    [Tooltip("Include these Tilemaps when building bounds (auto). If empty, will find all Tilemaps under searchRoot or scene).")]
    public List<Tilemap> extraTilemaps = new List<Tilemap>();

    [Tooltip("Also include these Renderers (e.g., TilemapRenderer, SpriteRenderer) as part of world bounds.")]
    public List<Renderer> extraRenderers = new List<Renderer>();

    [Tooltip("Also include these 2D colliders' bounds (e.g., composite/world border).")]
    public List<Collider2D> extraColliders = new List<Collider2D>();

    [Header("Debug (read-only)")]
    public Rect worldBounds; // filled automatically if autoDetectBounds=true

    Camera cam;
    Vector3 vel;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;              // 2D camera
        if (autoDetectBounds) RefreshWorldBounds();
    }

#if UNITY_EDITOR
    // Update bounds live when tweaking in the editor
    void OnValidate()
    {
        if (!cam) cam = GetComponent<Camera>();
        if (cam) cam.orthographic = true;
        if (autoDetectBounds && Application.isEditor && !Application.isPlaying)
            RefreshWorldBounds();
    }
#endif

    void LateUpdate()
    {
        if (!target) return;

        Vector3 desired = target.position + offset;

        if (clampToBounds)
        {
            float halfH = cam.orthographicSize;
            float halfW = halfH * cam.aspect;

            float minX = worldBounds.xMin + halfW;
            float maxX = worldBounds.xMax - halfW;
            float minY = worldBounds.yMin + halfH;
            float maxY = worldBounds.yMax - halfH;

            // If the map is smaller than the camera, center-lock
            if (minX > maxX) { float cx = (worldBounds.xMin + worldBounds.xMax) * 0.5f; desired.x = cx; }
            else desired.x = Mathf.Clamp(desired.x, minX, maxX);

            if (minY > maxY) { float cy = (worldBounds.yMin + worldBounds.yMax) * 0.5f; desired.y = cy; }
            else desired.y = Mathf.Clamp(desired.y, minY, maxY);
        }

        transform.position = Vector3.SmoothDamp(transform.position, desired, ref vel, smoothTime);
    }

    /// <summary>
    /// Rebuild worldBounds from Tilemaps, Renderers, and Colliders.
    /// Call this if you change your level layout at runtime.
    /// </summary>
    [ContextMenu("Refresh World Bounds")]
    public void RefreshWorldBounds()
    {
        Bounds? merged = null;

        // 1) Gather candidates
        var roots = new List<Transform>();
        if (searchRoot) roots.Add(searchRoot);
        else
        {
            // No explicit root: search active scene
            foreach (var go in FindObjectsOfType<GameObject>())
                if (go.scene.IsValid() && go.activeInHierarchy) roots.Add(go.transform);
        }

        // a) Tilemaps (found + extra)
        var tilemaps = new HashSet<Tilemap>(extraTilemaps);
        foreach (var r in roots)
            foreach (var tm in r.GetComponentsInChildren<Tilemap>(true))
                tilemaps.Add(tm);

        foreach (var tm in tilemaps)
        {
            var rend = tm.GetComponent<Renderer>();
            if (rend) Merge(ref merged, rend.bounds);
            else
            {
                // Fallback: transform localBounds to world space
                Bounds lb = tm.localBounds;
                Merge(ref merged, TransformBounds(tm.transform.localToWorldMatrix, lb));
            }
        }

        // b) Renderers (explicit)
        foreach (var rd in extraRenderers)
            if (rd) Merge(ref merged, rd.bounds);

        // c) Colliders (explicit)
        foreach (var col in extraColliders)
            if (col) Merge(ref merged, col.bounds);

        // If nothing found, create a sane default around (0,0)
        if (merged == null)
        {
            worldBounds = new Rect(-50f, -30f, 100f, 60f);
        }
        else
        {
            var b = merged.Value;
            worldBounds = Rect.MinMaxRect(b.min.x, b.min.y, b.max.x, b.max.y);
        }
    }

    static void Merge(ref Bounds? acc, Bounds add)
    {
        if (acc == null) acc = add;
        else
        {
            var b = acc.Value;
            b.Encapsulate(add);
            acc = b;
        }
    }

    static Bounds TransformBounds(Matrix4x4 m, Bounds b)
    {
        // Transform an AABB by matrix by transforming its 8 corners and encapsulating
        var center = m.MultiplyPoint3x4(b.center);
        var ext = b.extents;
        Vector3[] axes = {
            m.MultiplyVector(new Vector3(ext.x, 0, 0)),
            m.MultiplyVector(new Vector3(0, ext.y, 0)),
            m.MultiplyVector(new Vector3(0, 0, ext.z))
        };
        var worldExtents = new Vector3(
            Mathf.Abs(axes[0].x) + Mathf.Abs(axes[1].x) + Mathf.Abs(axes[2].x),
            Mathf.Abs(axes[0].y) + Mathf.Abs(axes[1].y) + Mathf.Abs(axes[2].y),
            Mathf.Abs(axes[0].z) + Mathf.Abs(axes[1].z) + Mathf.Abs(axes[2].z));
        return new Bounds(center, worldExtents * 2f);
    }
}
