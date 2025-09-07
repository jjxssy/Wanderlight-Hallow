using UnityEngine;

/// <summary>
/// Trigger placed in an area that changes the background music
/// when the player enters its collider.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class AreaMusicTrigger : MonoBehaviour
{
    [Tooltip("The background music to play when the player enters this area.")]
    [SerializeField] private AudioClip areaMusic;

    [Tooltip("How long the crossfade between tracks should take.")]
    [SerializeField] private float fadeDuration = 1.5f;

    /// <summary>
    /// Gets or sets the background music clip for this area.
    /// </summary>
    public AudioClip AreaMusic
    {
        get => areaMusic;
        set => areaMusic = value;
    }

    /// <summary>
    /// Gets or sets the crossfade duration (in seconds).
    /// </summary>
    public float FadeDuration
    {
        get => fadeDuration;
        set => fadeDuration = value;
    }

    /// <summary>
    /// Ensures the collider is always set as a trigger.
    /// </summary>
    private void Start()
    {
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    /// <summary>
    /// Called when another collider enters this trigger.
    /// If the collider belongs to the player, music is changed.
    /// </summary>
    /// <param name="other">The collider entering the trigger.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && AudioManager.Instance != null)
        {
            AudioManager.Instance.ChangeMusic(areaMusic, fadeDuration);
        }
    }
}
