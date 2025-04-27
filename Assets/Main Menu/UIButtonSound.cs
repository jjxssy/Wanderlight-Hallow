using UnityEngine;
using UnityEngine.UI;

// This script plays a click sound every time a UI Button is pressed
public class UIButtonSound : MonoBehaviour
{
    public AudioSource uiAudioSource; // The AudioSource that will play the click sound
    public AudioClip clickSound;      // The sound clip to play when the button is clicked

    private Button button; // Reference to the Button component on this GameObject

    private void Awake()
    {
        // Get the Button component attached to this GameObject
        button = GetComponent<Button>();

        // If a Button component is found, add PlayClickSound as a listener to the onClick event
        if (button != null)
        {
            button.onClick.AddListener(PlayClickSound);
        }
    }

    // This function plays the click sound when the button is pressed
    private void PlayClickSound()
    {
        // Check if both AudioSource and ClickSound are assigned
        if (uiAudioSource && clickSound)
        {
            // Play the click sound once without interrupting other sounds
            uiAudioSource.PlayOneShot(clickSound);
        }
    }
}
