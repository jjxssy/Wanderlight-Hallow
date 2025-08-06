using UnityEngine;

public class TestAchievements : MonoBehaviour
{ 
    private void Start()
    {
        //AchievementManager.Instance.ResetAllAchievements();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            AchievementManager.Instance.AddProgress("001", 1);
            AchievementManager.Instance.AddProgress("004", 1);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            AchievementManager.Instance.AddProgress("002", 1);
            AchievementManager.Instance.AddProgress("004", 1);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            AchievementManager.Instance.AddProgress("003", 1);
            AchievementManager.Instance.AddProgress("004", 1);
        }
    }

    // this script is just for testing
}
