using UnityEngine;

/// <summary>
/// Teleports the user to the mouse position.
/// 1. Prevents teleporting INTO solid objects (Walls, Obstacles).
/// 2. Prevents teleporting OUTSIDE the "LevelBounds" PolygonCollider.
/// </summary>
[CreateAssetMenu(fileName = "New Teleport Skill", menuName = "Skills/Teleport Skill")]
public class TeleportSkill : Skill
{
    [Header("Teleport Effects")]
    [SerializeField] private GameObject teleportOutVFX;
    [SerializeField] private GameObject teleportInVFX;

    // Cached reference to the Play Area boundaries
    // Make sure your boundary object has a PolygonCollider2D and is tagged "LevelBounds"
    private static PolygonCollider2D levelBounds;

    // --- Java-style getters/setters ---
    public GameObject GetTeleportOutVFX() { return teleportOutVFX; }
    public void SetTeleportOutVFX(GameObject value) { teleportOutVFX = value; }

    public GameObject GetTeleportInVFX() { return teleportInVFX; }
    public void SetTeleportInVFX(GameObject value) { teleportInVFX = value; }

    public override void Activate(GameObject user)
    {
        // 1. Find and Cache the Level Bounds if missing
        if (levelBounds == null)
        {
            GameObject boundsObj = GameObject.FindGameObjectWithTag("LevelBounds");
            if (boundsObj != null)
            {
                levelBounds = boundsObj.GetComponent<PolygonCollider2D>();
            }
            else
            {
                Debug.LogWarning("No object tagged 'LevelBounds' found. 'Outside' check will be skipped.");
            }
        }

        Vector3 destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        destination.z = user.transform.position.z;

        // 2. CHECK OBSTACLES: Block teleport if landing ON a solid object
        // We look for any collider at the point.
        Collider2D hitCollider = Physics2D.OverlapPoint(destination);

        if (hitCollider != null)
        {
            // We ignore Triggers and we ignore the LevelBounds itself (because we are supposed to be inside it)
            bool isLevelBounds = (levelBounds != null && hitCollider == levelBounds);

            if (!hitCollider.isTrigger && !isLevelBounds)
            {
                Debug.Log("Cannot teleport into a solid object: " + hitCollider.name);
                return;
            }
        }

        // 3. CHECK BOUNDS: Block teleport if landing OUTSIDE the polygon
        if (levelBounds != null)
        {
            // OverlapPoint returns true if the point is INSIDE the polygon
            if (levelBounds.OverlapPoint(destination))
            {
                Debug.Log("Cannot teleport outside the Level Bounds!");
                return;
            }
        }

        // 4. EXECUTE TELEPORT
        if (teleportOutVFX != null)
        {
            Object.Instantiate(teleportOutVFX, user.transform.position, Quaternion.identity);
        }

        user.transform.position = destination;

        if (teleportInVFX != null)
        {
            Object.Instantiate(teleportInVFX, user.transform.position, Quaternion.identity);
        }
    }
}