using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AreaMusicTrigger : MonoBehaviour
{
    [Tooltip("The background music to play when the player enters this area.")]
    public AudioClip areaMusic;

    [Tooltip("How long the crossfade between tracks should take.")]
    public float fadeDuration = 1.5f;

    private void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (AudioManager.instance != null)
            {
                AudioManager.instance.ChangeMusic(areaMusic, fadeDuration);
            }
        }
    }
}