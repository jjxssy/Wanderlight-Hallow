using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Teleports the user to the mouse position if the target cell/point is not blocked
/// by a solid collider or a solid tile. Spawns optional VFX before/after teleport.
/// Uses private serialized fields with Java-style getters/setters.
/// </summary>
[CreateAssetMenu(fileName = "New Teleport Skill", menuName = "Skills/Teleport Skill")]
public class TeleportSkill : Skill
{
    [Header("Teleport Effects")]
    [SerializeField] private GameObject teleportOutVFX;
    [SerializeField] private GameObject teleportInVFX;

    // Cached reference to a solid/collision tilemap (tag your Tilemap as "CollisionTilemap")
    private static Tilemap collisionTilemap;

    // --- Java-style getters/setters (no public properties) ---
    public GameObject GetTeleportOutVFX() { return teleportOutVFX; }
    public void SetTeleportOutVFX(GameObject value) { teleportOutVFX = value; }

    public GameObject GetTeleportInVFX() { return teleportInVFX; }
    public void SetTeleportInVFX(GameObject value) { teleportInVFX = value; }

    /// <summary>
    /// Teleport to mouse world position if not overlapping a solid collider or solid tile.
    /// Spawns out/in VFX if assigned.
    /// </summary>
    public override void Activate(GameObject user)
    {
        if (collisionTilemap == null)
        {
            GameObject tilemapObject = GameObject.FindGameObjectWithTag("CollisionTilemap");
            if (tilemapObject != null)
            {
                collisionTilemap = tilemapObject.GetComponent<Tilemap>();
            }
        }

        Vector3 destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        destination.z = user.transform.position.z;

        // Block teleport if landing on any non-trigger collider
        Collider2D hitCollider = Physics2D.OverlapPoint(destination);
        if (hitCollider != null && !hitCollider.isTrigger)
        {
            Debug.Log("Cannot teleport into a solid object: " + hitCollider.name);
            return;
        }

        // Block teleport if landing on a solid tile in the collision tilemap
        if (collisionTilemap != null)
        {
            Vector3Int cell = collisionTilemap.WorldToCell(destination);
            if (collisionTilemap.HasTile(cell))
            {
                Debug.Log("Cannot teleport onto a solid tile!");
                return;
            }
        }

        // VFX out -> teleport -> VFX in
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
