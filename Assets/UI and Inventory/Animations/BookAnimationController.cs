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

        // Step 1: Play book opening animation
        bookOpening.SetActive(true);
        finalSprite.SetActive(false);
        tabsClose.SetActive(false); // ⛔ Ensure closing book is hidden

        // Wait for BookOpen animation to finish (assume it's 1.5s here)
        StartCoroutine(WaitForSeconds(1.5f, () =>
        {
            bookOpening.SetActive(false);    // Hide animated opener
            finalSprite.SetActive(true);     // Show static open book with tabs & X button
        }));
    }

    public void OnCloseButtonPressed()
    {
        if (!isOpen) return;
        isOpen = false;

        finalSprite.SetActive(false);        // Hide overlays and static book
        tabsClose.SetActive(true);           // Show closing animation
        tabsCloseAnimator.Play(tabsCloseAnimName);

        StartCoroutine(WaitForSeconds(tabsCloseDuration, () =>
        {
            tabsClose.SetActive(false);      // Hide once animation ends
        }));
    }

    private IEnumerator WaitForSeconds(float seconds, System.Action callback)
    {
        yield return new WaitForSeconds(seconds);
        callback?.Invoke();
    }
}
