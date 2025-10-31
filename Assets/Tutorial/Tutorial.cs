using System.Collections;
using System.Linq;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private DialogManager dialogManager;
    [TextArea(3,10)]
    [SerializeField] private string[] MovenentDialogues;
    [TextArea(3, 10)]
    [SerializeField] private string[] MovementCompleteDialogues;
    [SerializeField] private ItemWorld testItem;
    [TextArea(3, 10)]
    [SerializeField] private string[] InventoryDialogues;
    [SerializeField] private WorldSkill testSkill;
    [TextArea(3, 10)]
    [SerializeField] private string[] skillDialogues;


    private KeyCode[] movementKeys = {KeyCode.W,KeyCode.S,KeyCode.A,KeyCode.D};
    private int dialogState = 0;
    
    private bool firstTextShown = false;
    void Start()
    {
        StartCoroutine(StartDialogue());
        //PlayerPrefs.GetInt("DialogueState", 0);
    }

    void Update()
    {
        if (firstTextShown && movementKeys.Any(Input.GetKeyDown) && dialogState == 1) 
        {
            dialogManager.ShowDialog(MovementCompleteDialogues);
            dialogState = 2;
           // PlayerPrefs.SetInt("DialogueState", 2);
        }
        if (dialogState == 2 && testItem == null)
        {
            dialogManager.ShowDialog(InventoryDialogues);
            dialogState = 3;
            //PlayerPrefs.SetInt("DialogueState", 3);
        }
        if (dialogState == 3 && testSkill == null)
        {
            dialogManager.ShowDialog(skillDialogues);
            dialogState = 4;
           // PlayerPrefs.SetInt("DialogueState", 4);
        }
    }
    private IEnumerator StartDialogue()
    {
        yield return new WaitForSeconds(0.5f);
        dialogManager.ShowDialog(MovenentDialogues);
        yield return new WaitForSeconds(1f);
        firstTextShown = true;
        dialogState = 1;
        //PlayerPrefs.SetInt("DialogueState", 1);
    }
}
