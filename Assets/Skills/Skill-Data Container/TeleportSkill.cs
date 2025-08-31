using UnityEngine;
using UnityEngine.Tilemaps; // <-- NEW: Add this line to work with Tilemaps

[CreateAssetMenu(fileName = "New Teleport Skill", menuName = "Skills/Teleport Skill")]
public class TeleportSkill : Skill
{
    [Header("Teleport Effects")]
    public GameObject teleportOutVFX;
    public GameObject teleportInVFX;

    // This will hold a reference to our solid tilemap
    private static Tilemap collisionTilemap;

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

        Collider2D hitCollider = Physics2D.OverlapPoint(destination);
        if (hitCollider != null && !hitCollider.isTrigger)
        {
            Debug.Log("Cannot teleport into a solid object: " + hitCollider.name);
            return;
        }

        // --- CHECK 2: Tilemap Collider ---
        if (collisionTilemap != null)
        {
            // Convert the world position to a grid cell position
            Vector3Int cellPosition = collisionTilemap.WorldToCell(destination);
            // Check if there is a tile at that cell AND if that tile has a collider
            if (collisionTilemap.HasTile(cellPosition))
            {
                Debug.Log("Cannot teleport onto a solid tile!");
                return;
            }
        }

        // If all checks pass, proceed with the teleport
        if (teleportOutVFX != null)
        {
            Instantiate(teleportOutVFX, user.transform.position, Quaternion.identity);
        }

        user.transform.position = destination;

        if (teleportInVFX != null)
        {
            Instantiate(teleportInVFX, user.transform.position, Quaternion.identity);
        }
    }
}