using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;

    private AudioSource audioSource;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
    }


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
}