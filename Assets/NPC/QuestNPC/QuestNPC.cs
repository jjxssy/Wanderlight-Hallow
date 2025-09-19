using System.Collections;
using UnityEngine;
using TMPro; // Make sure you have TextMeshPro installed in your project

public class QuestNPC : MonoBehaviour
{
    // The three states of our quest
    public enum QuestState { QUEST_NOT_STARTED, QUEST_ACTIVE, QUEST_COMPLETED }
    [SerializeField] private QuestState currentQuestState;

    [Header("Dialogue Settings")]
    [SerializeField] private TextMeshProUGUI dialogueText; 
    [SerializeField] private GameObject dialoguePanel; // The UI panel that holds the dialogue text
    [SerializeField] private GameObject interactionPrompt; // The "Press E to talk" UI prompt

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
        // Hide the dialogue and prompt at the start
        dialoguePanel.SetActive(false);
        interactionPrompt.SetActive(false);
    }

    void Update()
    {
        if (playerIsNearby && Input.GetKeyDown(KeyCode.E) && !dialoguePanel.activeSelf)
        {
            StartCoroutine(StartDialogue());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearby = true;
            interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearby = false;
            interactionPrompt.SetActive(false);
            StopAllCoroutines();
            dialoguePanel.SetActive(false);
        }
    }

    private IEnumerator StartDialogue()
    {
        dialoguePanel.SetActive(true);
        interactionPrompt.SetActive(false);

        string[] currentDialogue;

        // Determine which dialogue to show based on the quest state
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

        // Display the sentences one by one
        if (currentDialogue != null)
        {
            foreach (string sentence in currentDialogue)
            {
                dialogueText.text = "";
                foreach (char letter in sentence.ToCharArray())
                {
                    dialogueText.text += letter;
                    yield return new WaitForSeconds(0.05f); // Typing speed
                }
                yield return new WaitForSeconds(1.5f); // Pause between sentences
            }
        }

        // After the dialogue is finished
        dialoguePanel.SetActive(false);

        if (playerIsNearby)
        {
            interactionPrompt.SetActive(true);
        }

        // Update the quest state after the first interaction
        if (currentQuestState == QuestState.QUEST_NOT_STARTED)
        {
            currentQuestState = QuestState.QUEST_ACTIVE;
        }

        // If the quest is completed, give the reward
        if (currentQuestState == QuestState.QUEST_COMPLETED)
        {
            GiveReward();
        }
    }

    // This public method will be called by the boss's script
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
            Debug.Log("Reward given!");
        }
        else
        {
            Debug.LogWarning("No reward prefab assigned to the NPC!");
        }
    }
}