using UnityEngine;
using System.Collections;

public class BookAnimationController : MonoBehaviour
{
    [SerializeField] private GameObject bookOpening;     // Opening animation (book and tabs)
    [SerializeField] private GameObject finalSprite;     // Open Book static image + overlays
    [SerializeField] private GameObject tabsClose;       // Closing animation (tabs + book)
    [SerializeField] private Animator tabsCloseAnimator;
    [SerializeField] private string tabsCloseAnimName = "TabsClose";
    [SerializeField] private float tabsCloseDuration = 1.0f;

    private bool isOpen = false;

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

    public void OnCloseButtonPressed()
    {
        if (!isOpen) return;
        isOpen = false;

        finalSprite.SetActive(false);
        tabsClose.SetActive(true);
        tabsCloseAnimator.Play(tabsCloseAnimName);

        StartCoroutine(WaitForAnimationToEnd(tabsCloseAnimator, tabsCloseAnimName, () =>
{
    // Wait an extra 1.5 seconds after animation ends
    StartCoroutine(WaitForSeconds(1.0f, () =>
    {
        tabsClose.SetActive(false);
    }));
}));

    }

    private IEnumerator WaitForSeconds(float seconds, System.Action callback)
    {
        yield return new WaitForSeconds(seconds);
        callback?.Invoke();
    }

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
