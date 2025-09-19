using UnityEngine;
using System.Collections;

/// <summary>
/// Central audio manager that persists between scenes.
/// Handles background music playback and smooth crossfades between tracks.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the <see cref="AudioManager"/>.
    /// </summary>
    public static AudioManager Instance { get; private set; }

    /// <summary>
    /// The audio source used for background music playback.
    /// </summary>
    private AudioSource audioSource;

    /// <summary>
    /// Reference to the currently running fade coroutine (if any).
    /// </summary>
    private Coroutine fadeCoroutine;

    /// <summary>
    /// Ensures a single persistent instance of the <see cref="AudioManager"/>.
    /// Initializes the <see cref="AudioSource"/>.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        //DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
    }

    /// <summary>
    /// Initiates a music change with a crossfade to the new clip.
    /// </summary>
    /// <param name="newClip">The new <see cref="AudioClip"/> to play.</param>
    /// <param name="fadeDuration">The duration of the crossfade in seconds.</param>
    public void ChangeMusic(AudioClip newClip, float fadeDuration = 1.0f)
    {
        if (newClip != null && audioSource.clip != newClip)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(FadeAndSwitchMusic(newClip, fadeDuration));
        }
    }

    /// <summary>
    /// Coroutine that handles fading out the current track,
    /// switching to the new clip, and fading it in.
    /// </summary>
    /// <param name="newClip">The new <see cref="AudioClip"/> to switch to.</param>
    /// <param name="duration">Total fade duration in seconds.</param>
    private IEnumerator FadeAndSwitchMusic(AudioClip newClip, float duration)
    {
        float startVolume = audioSource.volume;
        float timer = 0f;

        // --- Fade Out ---
        while (timer < duration / 2)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / (duration / 2));
            timer += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = 0f;

        // --- Switch Clip ---
        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.Play();

        // --- Fade In ---
        timer = 0f;
        while (timer < duration / 2)
        {
            audioSource.volume = Mathf.Lerp(0f, startVolume, timer / (duration / 2));
            timer += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = startVolume;

        fadeCoroutine = null;
    }

    /// <summary>
    /// Gets or sets the current background music volume (0.0 – 1.0).
    /// </summary>
    public float Volume
    {
        get => audioSource != null ? audioSource.volume : 0f;
        set
        {
            if (audioSource != null)
                audioSource.volume = Mathf.Clamp01(value);
        }
    }

    /// <summary>
    /// Gets the current <see cref="AudioClip"/> being played.
    /// </summary>
    public AudioClip CurrentClip => audioSource != null ? audioSource.clip : null;
}
