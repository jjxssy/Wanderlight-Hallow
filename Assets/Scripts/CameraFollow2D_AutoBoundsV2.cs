using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

/// <summary>
/// 2D camera follower with optional auto-detected world bounds and clamping.
/// Attach to any orthographic Camera (main or minimap). Each camera can
/// have its own settings (smoothness, size, padding, etc.).
/// - If <see cref="target"/> is not assigned and <see cref="autoFindTargetByTag"/> is true,
///   the script will try to find a Transform tagged "Player".
/// - Bounds are auto-merged from Tilemaps/Renderers/Colliders (or default rectangle).
/// </summary>
[DisallowMultipleComponent, RequireComponent(typeof(Camera))]
public class CameraFollow2D_AutoBoundsV2 : MonoBehaviour
{
    #region Target
    /// <summary>
    /// The Transform to follow (typically the player).
    /// </summary>
    [Header("Target")]
    public Transform target;

    /// <summary>
    /// If true and <see cref="target"/> is null, automatically finds the first object with <see cref="targetTag"/>.
    /// </summary>
    [Tooltip("If target is null, try to FindWithTag on Awake.")]
    public bool autoFindTargetByTag = true;

    /// <summary>
    /// Tag used to auto-find the target if <see cref="autoFindTargetByTag"/> is enabled.
    /// </summary>
    [Tooltip("Only used when autoFindTargetByTag is enabled.")]
    public string targetTag = "Player";
    #endregion

    #region Follow
    /// <summary>
    /// World-space offset added to the target position. Keep Z at -10 for 2D cameras.
    /// </summary>
    [Header("Follow")]
    public Vector3 offset = new Vector3(0, 0, -10);

    /// <summary>
    /// Smooth time (seconds) used when the camera is near the target.
    /// </summary>
    [Tooltip("Smooth time when close to the target.")]
    [Range(0.01f, 0.5f)] public float smoothTime = 0.12f;

    /// <summary>
    /// If planar distance to desired is greater than this, use fast catch-up.
    /// </summary>
    [Tooltip("If we're farther than this, use fast catch-up.")]
    public float fastDistance = 5f;

    /// <summary>
    /// Maximum speed (units/second) used during fast catch-up.
    /// </summary>
    [Tooltip("Max speed used during catch-up (units/second).")]
    public float maxSpeed = 40f;
    #endregion

    #region Bounds / Clamp
    /// <summary>
    /// If true, clamp the camera inside <see cref="worldBounds"/> (after padding).
    /// </summary>
    [Header("Bounds / Clamp")]
    public bool clampToBounds = true;

    /// <summary>
    /// If true, attempt to auto-detect world bounds from Tilemaps/Renderers/Colliders.
    /// </summary>
    public bool autoDetectBounds = true;

    /// <summary>
    /// Extra padding applied inside the computed bounds (world units).
    /// </summary>
    [Tooltip("Extra padding inside bounds (world units).")]
    public float boundsPadding = 0.5f;

    /// <summary>
    /// Optional root under which to search for Tilemaps/Renderers/Colliders (e.g. Grid).
    /// If null, the whole scene is searched.
    /// </summary>
    [Tooltip("Optional parent to search under (e.g., Grid). If null, searches whole scene.")]
    public Transform searchRoot;
    #endregion

    #region Manual Extras
    /// <summary>
    /// Optional extra Tilemaps to include in bounds.
    /// </summary>
    [Header("Manual Extras (optional)")]
    public List<Tilemap> extraTilemaps = new();

    /// <summary>
    /// Optional extra Renderers to include in bounds.
    /// </summary>
    public List<Renderer> extraRenderers = new();

    /// <summary>
    /// Optional extra 2D Colliders to include in bounds.
    /// </summary>
    public List<Collider2D> extraColliders = new();
    #endregion

    #region Debug
    /// <summary>
    /// The final world-space rectangle the camera is clamped within (computed if auto).
    /// </summary>
    [Header("Debug")]
    public Rect worldBounds;

    /// <summary>
    /// Gizmo color for drawing the bounds in the editor.
    /// </summary>
    public Color gizmoColor = new Color(0, 1, 0, 0.25f);
    #endregion

    private Camera cam;
    private Vector3 vel;

    /// <summary>
    /// Ensures orthographic camera, tries to find target if needed, and refreshes bounds if enabled.
    /// </summary>
    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;

        // Auto-find target if not assigned
        if (!target && autoFindTargetByTag && !string.IsNullOrEmpty(targetTag))
        {
            var go = GameObject.FindGameObjectWithTag(targetTag);
            if (go) target = go.transform;
        }

        if (autoDetectBounds) RefreshWorldBounds();
    }

#if UNITY_EDITOR
    /// <summary>
    /// Editor-time validation to keep camera orthographic and preview bounds.
    /// </summary>
    private void OnValidate()
    {
        if (!cam) cam = GetComponent<Camera>();
        if (cam) cam.orthographic = true;
        if (!Application.isPlaying && autoDetectBounds) RefreshWorldBounds();
    }
#endif

    /// <summary>
    /// Performs follow and clamping after all other updates have run.
    /// </summary>
    private void LateUpdate()
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

            // If the camera view is larger than the map on an axis, lock to center on that axis
            if (minX > maxX) desired.x = (pad.xMin + pad.xMax) * 0.5f;
            else desired.x = Mathf.Clamp(desired.x, minX, maxX);

            if (minY > maxY) desired.y = (pad.yMin + pad.yMax) * 0.5f;
            else desired.y = Mathf.Clamp(desired.y, minY, maxY);
        }

        // Distance on X/Y plane for deciding follow mode
        Vector2 d = (Vector2)(desired - transform.position);
        float planarDist = d.magnitude;

        Vector3 next = (planarDist > fastDistance)
            ? Vector3.MoveTowards(transform.position, desired, maxSpeed * Time.deltaTime)   // Fast catch-up
            : Vector3.SmoothDamp(transform.position, desired, ref vel, smoothTime);         // Smooth follow

        transform.position = next;
    }

    /// <summary>
    /// Scans the scene (or a root) for Tilemaps/Renderers/Colliders and merges their bounds.
    /// If nothing is found, uses a default rectangle.
    /// </summary>
    [ContextMenu("Refresh World Bounds")]
    public void RefreshWorldBounds()
    {
        Bounds? merged = null;

        // Build search roots
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
            // Sensible default area if nothing detected
            worldBounds = new Rect(-50f, -30f, 100f, 60f);
        }
        else
        {
            var b = merged.Value;
            worldBounds = Rect.MinMaxRect(b.min.x, b.min.y, b.max.x, b.max.y);
        }
    }

    /// <summary>
    /// Expands <paramref name="acc"/> to include <paramref name="add"/>.
    /// </summary>
    /// <param name="acc">Accumulator bounds (nullable).</param>
    /// <param name="add">Bounds to encapsulate.</param>
    private static void Merge(ref Bounds? acc, Bounds add)
    {
        if (acc == null) acc = add;
        else { var b = acc.Value; b.Encapsulate(add); acc = b; }
    }

    /// <summary>
    /// Draws the world bounds gizmo in the editor for convenience.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Vector3 c = new Vector3(worldBounds.center.x, worldBounds.center.y, 0f);
        Vector3 s = new Vector3(worldBounds.width, worldBounds.height, 0f);
        Gizmos.DrawCube(c, s);
    }
}
