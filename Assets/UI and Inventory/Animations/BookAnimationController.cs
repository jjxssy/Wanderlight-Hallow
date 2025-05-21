using UnityEngine;

public class BookAnimationController : MonoBehaviour
{
    public GameObject bookOpening;
    public GameObject finalSprite;
    public Animator bookAnimator;

    private bool isOpen = false;

    public void OnBookButtonPressed()
    {
        if (isOpen) return;

        isOpen = true;

        bookOpening.SetActive(true);
        finalSprite.SetActive(false);
        bookAnimator.Play("BookOpen");

        StartCoroutine(WaitForAnimationToEnd("BookOpen", () =>
        {
            bookOpening.SetActive(false);
            finalSprite.SetActive(true);
        }));
    }

    public void OnCloseButtonPressed()
    {
        if (!isOpen) return;

        isOpen = false;

        finalSprite.SetActive(false);
        bookOpening.SetActive(true);
        bookAnimator.Play("BookClose");

        StartCoroutine(WaitForAnimationToEnd("BookClose", () =>
        {
            bookOpening.SetActive(false);
        }));
    }

    private System.Collections.IEnumerator WaitForAnimationToEnd(string animationName, System.Action onComplete)
    {
        // Wait until the animator starts playing the given animation
        while (!bookAnimator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
            yield return null;

        // Wait until the animation has finished
        while (bookAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            yield return null;

        onComplete?.Invoke();
    }
}
