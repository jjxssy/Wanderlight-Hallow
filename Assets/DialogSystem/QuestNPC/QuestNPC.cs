using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// NPC quest controller that:
/// 1) Shows an interaction prompt when the player enters range,
/// 2) Starts context-appropriate dialogue based on the quest state,
/// 3) Advances the quest from NotStarted → Active, and
/// 4) Grants a reward when the quest is completed.
/// </summary>
public class QuestNPC : MonoBehaviour
{
    /// <summary>
    /// High-level quest phases for this NPC.
    /// </summary>
    public enum QuestState { QUEST_NOT_STARTED, QUEST_ACTIVE, QUEST_COMPLETED }
    /// <summary>Current quest state for this NPC.</summary>
    [SerializeField] private QuestState currentQuestState;

    [Header("Dialogue Settings")]
    /// <summary>UI object displayed to hint the player to interact (e.g., "Press E").</summary>
    [SerializeField] private GameObject interactionPrompt;
    /// <summary>Reference to the dialog manager that renders lines with a typewriter effect.</summary>
    [SerializeField] private DialogManager dialogManager;

    [Header("Dialogue Content")]
    /// <summary>Lines shown before the quest is accepted.</summary>
    [TextArea(3, 10)]
    [SerializeField] private string[] initialDialogue;
    /// <summary>Lines shown while the quest is ongoing.</summary>
    [TextArea(3, 10)]
    [SerializeField] private string[] activeDialogue;
    /// <summary>Lines shown after the quest is completed.</summary>
    [TextArea(3, 10)]
    [SerializeField] private string[] completionDialogue;

    [Header("Quest Settings")]
    /// <summary>Item rewarded to the player upon completing the quest.</summary>
    [SerializeField] private Item reward;

    /// <summary>True while the player is within the NPC's trigger.</summary>
    private bool playerIsNearby = false;

    /// <summary>
    /// Hides the interaction prompt on startup.
    /// </summary>
    void Start()
    {
        interactionPrompt.SetActive(false);
    }

    /// <summary>
    /// Handles interaction input and prompt visibility while the player remains nearby.
    /// </summary>
    void Update()
    {
        if (playerIsNearby)
        {
            if(Input.GetKeyDown(KeyCode.E) && !dialogManager.IsDialogueActive()) StartDialogue();
            else if(!dialogManager.IsDialogueActive()) interactionPrompt.SetActive(true);
        }
        else interactionPrompt.SetActive(false);
    }

    /// <summary>
    /// Marks the player as nearby when entering the trigger zone.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearby = true;
        }
    }

    /// <summary>
    /// Clears nearby state and stops any active dialogue when leaving the trigger zone.
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearby = false;
            dialogManager.StopDialog();
        }
    }

    /// <summary>
    /// Chooses the correct dialogue based on <see cref="currentQuestState"/>,
    /// shows it, advances state from NotStarted to Active, and grants reward if already Completed.
    /// </summary>
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

    /// <summary>
    /// External hook to mark this quest as completed (e.g., boss defeated).
    /// </summary>
    public void OnBossDefeated()
    {
        currentQuestState = QuestState.QUEST_COMPLETED;
        Debug.Log("Quest completed! The NPC is now waiting to reward the player.");
    }

    /// <summary>
    /// Attempts to give the configured <see cref="reward"/> to the player inventory.
    /// </summary>
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

