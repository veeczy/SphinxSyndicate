using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRange = 2f;
    public KeyCode interactKey = KeyCode.E;
    public LayerMask npcLayer;

    [Header("Dialogue UI")]
    public GameObject dialogueUI;
    public TMP_Text dialogueText;

    private NPC currentNPC = null;
    private NPC talkingToNPC = null;   
    private int dialogueIndex = 0;
    private bool isDialogueActive = false;

    void Update()
    {
        // Detect nearest NPC within range
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactRange, npcLayer);
        if (hit != null)
            currentNPC = hit.GetComponent<NPC>();
        else
            currentNPC = null;

        //  Auto-end dialogue feature
        if (isDialogueActive && talkingToNPC != null)
        {
            float distance = Vector2.Distance(transform.position, talkingToNPC.transform.position);
            if (distance > interactRange * 4f) //Range at which dialogue ends

            {
                EndDialogue();
                return;
            }
        }

        // Handle pressing E
        if (Input.GetButtonDown("Interact"))
        {
            if (currentNPC != null)
            {
                if (!isDialogueActive)
                    StartDialogue();
                else
                    NextDialogue();
            }
        }
    }

    void StartDialogue()
    {
        if (currentNPC == null || currentNPC.dialogueLines.Length == 0)
            return;

        isDialogueActive = true;
        dialogueIndex = 0;
        talkingToNPC = currentNPC;   
        dialogueUI.SetActive(true);
        dialogueText.text = currentNPC.dialogueLines[dialogueIndex];
    }

    void NextDialogue()
    {
        if (currentNPC == null)
            return;

        dialogueIndex++;
        if (dialogueIndex >= currentNPC.dialogueLines.Length)
        {
            EndDialogue();
        }
        else
        {
            dialogueText.text = currentNPC.dialogueLines[dialogueIndex];
        }
    }

    void EndDialogue()
    {
        dialogueUI.SetActive(false);
        isDialogueActive = false;
        dialogueIndex = 0;
        talkingToNPC = null;   
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
