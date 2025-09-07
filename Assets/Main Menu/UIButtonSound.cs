using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Plays a click sound whenever the attached <see cref="Button"/> is pressed.
/// </summary>
[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    /// <summary>
    /// The <see cref="AudioSource"/> used to play the click sound.
    /// </summary>
    [SerializeField] private AudioSource uiAudioSource;

    /// <summary>
    /// The audio clip to play when the button is clicked.
    /// </summary>
    [SerializeField] private AudioClip clickSound;

    /// <summary>
    /// Cached reference to the <see cref="Button"/> component attached to this GameObject.
    /// </summary>
    private Button button;

    /// <summary>
    /// Gets or sets the <see cref="AudioSource"/> for playing UI sounds.
    /// </summary>
    public AudioSource UiAudioSource
    {
        get => uiAudioSource;
        set => uiAudioSource = value;
    }

    /// <summary>
    /// Gets or sets the audio clip played when the button is clicked.
    /// </summary>
    public AudioClip ClickSound
    {
        get => clickSound;
        set => clickSound = value;
    }

    /// <summary>
    /// Initializes the <see cref="Button"/> reference and registers
    /// <see cref="PlayClickSound"/> as a listener to the button’s <c>onClick</c> event.
    /// </summary>
    private void Awake()
    {
        button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(PlayClickSound);
        }
    }

    /// <summary>
    /// Plays the click sound when the button is pressed.
    /// </summary>
    private void PlayClickSound()
    {
        if (uiAudioSource && clickSound)
        {
            uiAudioSource.PlayOneShot(clickSound);
        }
    }
}
