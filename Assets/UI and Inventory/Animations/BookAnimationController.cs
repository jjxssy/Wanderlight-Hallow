using System.Collections;
using UnityEngine;

public class BookSequenceController : MonoBehaviour
{
    public Animator bookAnimator;     // Animation A
    public GameObject tabsObject;     // Tabs object (Animation B)
    public Animator tabsAnimator;     // Tabs animation controller

    public void StartSequence()
    {
        bookAnimator.Play("BookOpen", 0, 0f);
        StartCoroutine(WaitAndPlayTabs());
    }

    private IEnumerator WaitAndPlayTabs()
    {
        // Wait for the book animation to finish
        yield return new WaitForSeconds(bookAnimator.GetCurrentAnimatorStateInfo(0).length);

        tabsObject.SetActive(true);
        tabsAnimator.Play("TabsOpen", 0, 0f);
    }
}
