using UnityEngine;

/// <summary>
/// Minimal follower for a top-down minimap camera:
/// - Locks to target XY every LateUpdate
/// - Keeps a fixed Z (e.g., -10)
/// - Optional fixed rotation
/// </summary>
[DisallowMultipleComponent, RequireComponent(typeof(Camera))]
public sealed class MinimapFollower : MonoBehaviour
{
    /// <summary>
    /// The Transform to follow (usually the Player).
    /// </summary>
    [SerializeField] private Transform target;

    /// <summary>
    /// World-space offset (keep Z at -10 for 2D).
    /// </summary>
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    /// <summary>
    /// If true and <see cref="target"/> is null, find first object tagged "Player".
    /// </summary>
    [SerializeField] private bool autoFindTargetByTag = true;

    /// <summary>
    /// Keep a fixed rotation for the minimap camera regardless of target rotation.
    /// </summary>
    [SerializeField] private bool lockRotation = true;

    /// <summary>
    /// Rotation used when <see cref="lockRotation"/> is true.
    /// </summary>
    [SerializeField] private Vector3 lockedEuler = new Vector3(0f, 0f, 0f);

    private void Awake()
    {
        if (!target && autoFindTargetByTag)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go) target = go.transform;
        }

        var cam = GetComponent<Camera>();
        cam.orthographic = true;
    }

    private void LateUpdate()
    {
        if (!target) return;

        // Lock position to target XY + offset; keep Z from offset
        Vector3 p = target.position + offset;
        transform.position = p;

        if (lockRotation)
            transform.rotation = Quaternion.Euler(lockedEuler);
    }
}
