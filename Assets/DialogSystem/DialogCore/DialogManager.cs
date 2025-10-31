using System.Collections;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    private bool isDialogueActive = false;
    private bool isFastDialogueActive = false;
    private Coroutine activeCoroutine;
    private string[] currentDialogue;
    void Start()
    {
        dialoguePanel.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) isFastDialogueActive = true;
        if (Input.GetKeyUp(KeyCode.E)) isFastDialogueActive = false;
    }

    public void ShowDialog(string[] dialog)
    {
        StopDialog();
        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        currentDialogue = dialog;
        activeCoroutine = StartCoroutine(DialogCouroutine());
    }
    public void StopDialog()
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            isDialogueActive = false;
            dialoguePanel.SetActive(false);
            dialogueText.text = "";
        }
    }
    private IEnumerator DialogCouroutine()
    {
        yield return null;
        if (currentDialogue != null)
        {
            foreach (string sentence in currentDialogue)
            {
                dialogueText.text = "";
                foreach (char letter in sentence.ToCharArray())
                {
                    dialogueText.text += letter;
                    if (isFastDialogueActive)
                        yield return new WaitForSeconds(0.005f);
                    else
                        yield return new WaitForSeconds(0.05f);
                }
                yield return null;
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
                yield return null;
            }
        }
        dialoguePanel.SetActive(false);
        isDialogueActive = false;
    }
    public bool IsDialogueActive() { return isDialogueActive; }
}
