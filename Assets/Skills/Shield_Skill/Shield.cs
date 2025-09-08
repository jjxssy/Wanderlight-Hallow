using UnityEngine;

/// <summary>
/// Simple shield that destroys any incoming object whose layer matches the configured block mask.
/// Attach to a GameObject with a 2D trigger collider.
/// </summary>
public class Shield : MonoBehaviour
{
    /// <summary>
    /// Layers that this shield will block (and destroy) on trigger contact.
    /// </summary>
    [SerializeField] private LayerMask blockLayer;

    /// <summary>
    /// When another collider enters the shield, destroy it if its layer is in <see cref="blockLayer"/>.
    /// </summary>
    /// <param name="other">The collider that entered the shield trigger.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((blockLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            Destroy(other.gameObject);
        }
    }
}
