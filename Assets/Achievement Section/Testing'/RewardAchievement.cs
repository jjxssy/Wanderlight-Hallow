using System.Collections;
using TMPro;
using UnityEngine;

public class RewardAchievement : MonoBehaviour
{
    [SerializeField] GameObject popUpImage;
    [SerializeField] TextMeshProUGUI popUpText;

    [SerializeField] Item healthPotion;
    private void OnEnable()
    {
        AchievementManager.OnAchievementUnlocked += HandleAchievementCompleted;
    }

    private void OnDisable()
    {
        AchievementManager.OnAchievementUnlocked -= HandleAchievementCompleted;
    }
    private void HandleAchievementCompleted(Achievement completedAchievement)
    {
        Debug.Log("Broadcast Received! An achievement was completed: " + completedAchievement.title);

        if (completedAchievement.id == "001")
        {
            StartCoroutine(AchievementPopUp("Pressed X 5 times"));
            InventoryManager.Instance.AddItem(healthPotion);
        }
        else if (completedAchievement.id == "002")
        {
            StartCoroutine(AchievementPopUp("Pressed Y 10 times"));
        }
        //add switch or just a generic function...
    }
    private IEnumerator AchievementPopUp(string text)
    {
        if (popUpImage != null && popUpText != null)
        {
            popUpImage.SetActive(true);
            popUpText.text = text;
            yield return new WaitForSeconds(1.5f);
            popUpImage.SetActive(false);

            // just use a float timer in Update instead of coroutine, cause when one achievement is popped and you complete another achievement, the second achievement popup doesnt last full time, when the first achievement time ends
            //the second achievement pop is disabled too, but adding a timer will reset the timer to full when a 2nd achievement is obtained during the popup of 1st achievement.

            //this script is just for testing
        }
    }
}