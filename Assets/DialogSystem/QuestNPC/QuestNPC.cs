using System.Collections;
using UnityEngine;
using TMPro;

public class QuestNPC : MonoBehaviour
{
    public enum QuestState { QUEST_NOT_STARTED, QUEST_ACTIVE, QUEST_COMPLETED }
    [SerializeField] private QuestState currentQuestState;

    [Header("Dialogue Settings")]
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private DialogManager dialogManager;

    [Header("Dialogue Content")]
    [TextArea(3, 10)]
    [SerializeField] private string[] initialDialogue;
    [TextArea(3, 10)]
    [SerializeField] private string[] activeDialogue;
    [TextArea(3, 10)]
    [SerializeField] private string[] completionDialogue;

    [Header("Quest Settings")]
    [SerializeField] private Item reward;

    private bool playerIsNearby = false;

    void Start()
    {
        interactionPrompt.SetActive(false);
    }

    void Update()
    {
        if (playerIsNearby)
        {
            if(Input.GetKeyDown(KeyCode.E) && !dialogManager.IsDialogueActive()) StartDialogue();
            else if(!dialogManager.IsDialogueActive()) interactionPrompt.SetActive(true);
        }
        else interactionPrompt.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearby = false;
            dialogManager.StopDialog();
        }
    }

    private void StartDialogue()
    {
        interactionPrompt.SetActive(false);

        string[] currentDialogue;

        switch (currentQuestState)
        {
            case QuestState.QUEST_NOT_STARTED:
                currentDialogue = initialDialogue;
                break;
            case QuestState.QUEST_ACTIVE:
                currentDialogue = activeDialogue;
                break;
            case QuestState.QUEST_COMPLETED:
                currentDialogue = completionDialogue;
                break;
            default:
                currentDialogue = null;
                break;
        }

        if (currentDialogue != null)
        {
            dialogManager.ShowDialog(currentDialogue);
        }

        if (currentQuestState == QuestState.QUEST_NOT_STARTED)
        {
            currentQuestState = QuestState.QUEST_ACTIVE;
        }
        if (currentQuestState == QuestState.QUEST_COMPLETED)
        {
            GiveReward();
        }
    }
    public void OnBossDefeated()
    {
        currentQuestState = QuestState.QUEST_COMPLETED;
        Debug.Log("Quest completed! The NPC is now waiting to reward the player.");
    }

    private void GiveReward()
    {
        if (reward != null)
        {
            InventoryManager.Instance.AddItem(reward);
            Debug.Log("Reward given to player!");
        }
        else
        {
            Debug.LogWarning("No reward prefab assigned to the NPC!");
        }
    }

}

