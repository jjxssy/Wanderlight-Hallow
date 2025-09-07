using UnityEngine;
/// <summary>
/// Adjusts the SpriteRenderer's sorting order based on the object's Y position.
/// Objects lower on the screen will appear in front of objects higher up,
/// creating a top-down depth illusion.
/// </summary>
public class YSorter : MonoBehaviour
{    /// <summary>
    /// Cached reference to the attached SpriteRenderer.
    /// </summary>
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// Cache the SpriteRenderer on Awake.
    /// </summary>
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    /// <summary>
    /// Updates sorting order every frame so it follows the object's Y position.
    /// Lower Y → higher sorting order (drawn in front).
    /// </summary>
    void LateUpdate()
    {
        spriteRenderer.sortingOrder = (int)(-transform.position.y * 100f);
    }
}