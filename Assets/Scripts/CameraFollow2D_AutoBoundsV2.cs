using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 2D camera follower with optional auto-detected world bounds and clamping.
/// Attach to any orthographic Camera (main or minimap). Each camera can
/// have its own settings (smoothness, size, padding, etc.).
/// </summary>
[DisallowMultipleComponent, RequireComponent(typeof(Camera))]
public class CameraFollow2D_AutoBoundsV2 : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField, Tooltip("If target is null, try to FindWithTag on Awake.")]
    private bool autoFindTargetByTag = true;
    [SerializeField, Tooltip("Only used when autoFindTargetByTag is enabled.")]
    private string targetTag = "Player";

    [Header("Follow")]
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    [SerializeField, Tooltip("Smooth time when close to the target."), Range(0.01f, 0.5f)]
    private float smoothTime = 0.12f;
    [SerializeField, Tooltip("If we're farther than this, use fast catch-up.")]
    private float fastDistance = 5f;
    [SerializeField, Tooltip("Max speed used during catch-up (units/second).")]
    private float maxSpeed = 40f;

    [Header("Bounds / Clamp")]
    [SerializeField] private bool clampToBounds = true;
    [SerializeField] private bool autoDetectBounds = true;
    [SerializeField, Tooltip("Extra padding inside bounds (world units).")]
    private float boundsPadding = 0.5f;
    [SerializeField, Tooltip("Optional parent to search under (e.g., Grid). If null, searches whole scene.")]
    private Transform searchRoot;

    [Header("Manual Extras (optional)")]
    [SerializeField] private List<Tilemap> extraTilemaps = new();
    [SerializeField] private List<Renderer> extraRenderers = new();
    [SerializeField] private List<Collider2D> extraColliders = new();

    [Header("Debug")]
    [SerializeField] private Rect worldBounds;
    [SerializeField] private Color gizmoColor = new Color(0, 1, 0, 0.25f);

    private Camera cam;
    private Vector3 vel;

    public Transform GetTarget() { return target; }
    public void SetTarget(Transform t) { target = t; }

    public bool GetAutoFindTargetByTag() { return autoFindTargetByTag; }
    public void SetAutoFindTargetByTag(bool v) { autoFindTargetByTag = v; }

    public string GetTargetTag() { return targetTag; }
    public void SetTargetTag(string tag) { targetTag = tag; }

    public Vector3 GetOffset() { return offset; }
    public void SetOffset(Vector3 v) { offset = v; }

    public float GetSmoothTime() { return smoothTime; }
    public void SetSmoothTime(float v) { smoothTime = Mathf.Max(0.01f, v); }

    public float GetFastDistance() { return fastDistance; }
    public void SetFastDistance(float v) { fastDistance = v; }

    public float GetMaxSpeed() { return maxSpeed; }
    public void SetMaxSpeed(float v) { maxSpeed = v; }

    public bool GetClampToBounds() { return clampToBounds; }
    public void SetClampToBounds(bool v) { clampToBounds = v; }

    public bool GetAutoDetectBounds() { return autoDetectBounds; }
    public void SetAutoDetectBounds(bool v) { autoDetectBounds = v; }

    public float GetBoundsPadding() { return boundsPadding; }
    public void SetBoundsPadding(float v) { boundsPadding = v; }

    public Transform GetSearchRoot() { return searchRoot; }
    public void SetSearchRoot(Transform t) { searchRoot = t; }

    public List<Tilemap> GetExtraTilemaps() { return extraTilemaps; }
    public List<Renderer> GetExtraRenderers() { return extraRenderers; }
    public List<Collider2D> GetExtraColliders() { return extraColliders; }

    public Rect GetWorldBounds() { return worldBounds; }
    public void SetWorldBounds(Rect r) { worldBounds = r; }

    public Color GetGizmoColor() { return gizmoColor; }
    public void SetGizmoColor(Color c) { gizmoColor = c; }

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;

        if (!target && autoFindTargetByTag && !string.IsNullOrEmpty(targetTag))
        {
            var go = GameObject.FindGameObjectWithTag(targetTag);
            if (go) target = go.transform;
        }

        if (autoDetectBounds) RefreshWorldBounds();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!cam) cam = GetComponent<Camera>();
        if (cam) cam.orthographic = true;
        if (!Application.isPlaying && autoDetectBounds) RefreshWorldBounds();
    }
#endif

    private void LateUpdate()
    {
        if (!target) return;

        Vector3 desired = target.position + offset;

        if (clampToBounds)
        {
            float halfH = cam.orthographicSize;
            float halfW = halfH * cam.aspect;

            Rect pad = new Rect(
                worldBounds.x + boundsPadding,
                worldBounds.y + boundsPadding,
                worldBounds.width - 2f * boundsPadding,
                worldBounds.height - 2f * boundsPadding
            );

            float minX = pad.xMin + halfW;
            float maxX = pad.xMax - halfW;
            float minY = pad.yMin + halfH;
            float maxY = pad.yMax - halfH;

            if (minX > maxX) desired.x = (pad.xMin + pad.xMax) * 0.5f;
            else desired.x = Mathf.Clamp(desired.x, minX, maxX);

            if (minY > maxY) desired.y = (pad.yMin + pad.yMax) * 0.5f;
            else desired.y = Mathf.Clamp(desired.y, minY, maxY);
        }

        Vector2 d = (Vector2)(desired - transform.position);
        float planarDist = d.magnitude;

        Vector3 next = (planarDist > fastDistance)
            ? Vector3.MoveTowards(transform.position, desired, maxSpeed * Time.deltaTime)
            : Vector3.SmoothDamp(transform.position, desired, ref vel, smoothTime);

        transform.position = next;
    }

    [ContextMenu("Refresh World Bounds")]
    public void RefreshWorldBounds()
    {
        Bounds? merged = null;

        var roots = new List<Transform>();
        if (searchRoot) roots.Add(searchRoot);
        else
        {
            foreach (var go in FindObjectsOfType<GameObject>())
                if (go.scene.IsValid() && go.activeInHierarchy) roots.Add(go.transform);
        }

        var seenTms = new HashSet<Tilemap>(extraTilemaps);
        foreach (var r in roots)
            foreach (var tm in r.GetComponentsInChildren<Tilemap>(true))
                seenTms.Add(tm);

        foreach (var tm in seenTms)
        {
            var rend = tm.GetComponent<Renderer>();
            if (rend) Merge(ref merged, rend.bounds);
            else Merge(ref merged, tm.localBounds);
        }

        foreach (var rd in extraRenderers)
            if (rd) Merge(ref merged, rd.bounds);

        foreach (var col in extraColliders)
            if (col) Merge(ref merged, col.bounds);

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

    private static void Merge(ref Bounds? acc, Bounds add)
    {
        if (acc == null) acc = add;
        else { var b = acc.Value; b.Encapsulate(add); acc = b; }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Vector3 c = new Vector3(worldBounds.center.x, worldBounds.center.y, 0f);
        Vector3 s = new Vector3(worldBounds.width, worldBounds.height, 0f);
        Gizmos.DrawCube(c, s);
    }
}
