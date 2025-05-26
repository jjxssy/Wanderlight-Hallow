using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the animations for opening and closing a book UI element in the game.
/// </summary>
public class BookAnimationController : MonoBehaviour
{
    /// <summary>
    /// GameObject representing the book opening animation (including tabs).
    /// </summary>
    [SerializeField] private GameObject bookOpening;

    /// <summary>
    /// GameObject representing the final open book with static overlays.
    /// </summary>
    [SerializeField] private GameObject finalSprite;

    /// <summary>
    /// GameObject representing the closing animation for the book and tabs.
    /// </summary>
    [SerializeField] private GameObject tabsClose;

    /// <summary>
    /// Animator controlling the closing tabs animation.
    /// </summary>
    [SerializeField] private Animator tabsCloseAnimator;

    /// <summary>
    /// The name of the closing animation to play.
    /// </summary>
    [SerializeField] private string tabsCloseAnimName = "TabsClose";

    /// <summary>
    /// Duration to wait after the tabs closing animation finishes.
    /// </summary>
    [SerializeField] private float tabsCloseDuration = 1.0f;

    /// <summary>
    /// Indicates whether the book is currently open.
    /// </summary>
    private bool isOpen = false;

    /// <summary>
    /// Called when the book open button is pressed.
    /// Initiates the book opening animation and displays the final book UI.
    /// </summary>
    public void OnBookButtonPressed()
    {
        if (isOpen) return;
        isOpen = true;

        bookOpening.SetActive(true);
        finalSprite.SetActive(false);
        tabsClose.SetActive(false);

        StartCoroutine(WaitForSeconds(1.5f, () =>
        {
            bookOpening.SetActive(false);
            finalSprite.SetActive(true);
        }));
    }

    /// <summary>
    /// Called when the close button is pressed.
    /// Starts the tabs closing animation and hides the final book UI.
    /// </summary>
    public void OnCloseButtonPressed()
    {
        if (!isOpen) return;
        isOpen = false;

        finalSprite.SetActive(false);
        tabsClose.SetActive(true);
        tabsCloseAnimator.Play(tabsCloseAnimName);

        StartCoroutine(WaitForAnimationToEnd(tabsCloseAnimator, tabsCloseAnimName, () =>
        {
            // Wait an extra 1.0 second after animation ends before hiding tabsClose
            StartCoroutine(WaitForSeconds(1.0f, () =>
            {
                tabsClose.SetActive(false);
            }));
        }));
    }

    /// <summary>
    /// Coroutine that waits for a specified number of seconds before invoking a callback.
    /// </summary>
    /// <param name="seconds">Time to wait in seconds.</param>
    /// <param name="callback">Callback to invoke after the wait.</param>
    /// <returns>IEnumerator for coroutine.</returns>
    private IEnumerator WaitForSeconds(float seconds, System.Action callback)
    {
        yield return new WaitForSeconds(seconds);
        callback?.Invoke();
    }

    /// <summary>
    /// Coroutine that waits until a specific animation has started and finished playing,
    /// then calls the provided completion callback.
    /// </summary>
    /// <param name="animator">Animator playing the animation.</param>
    /// <param name="animationName">Name of the animation to wait for.</param>
    /// <param name="onComplete">Callback to invoke when the animation ends.</param>
    /// <returns>IEnumerator for coroutine.</returns>
    private IEnumerator WaitForAnimationToEnd(Animator animator, string animationName, System.Action onComplete)
    {
        // Wait until the animation starts
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
            yield return null;

        // Wait until the animation ends
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            yield return null;

        onComplete?.Invoke();
    }
}