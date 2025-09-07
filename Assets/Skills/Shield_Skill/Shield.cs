using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] private LayerMask blockLayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((blockLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            Destroy(other.gameObject);
        }
    }
}