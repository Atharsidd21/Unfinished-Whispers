using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    [Header("Dialogue")]
    public DialogueData simpleDialogue;         // for basic NPCs
    public ConversationData conversation;       // for important characters

    public void Interact()
    {
        if (conversation != null)
            DialogueManager.Instance.StartConversation(conversation);
        else if (simpleDialogue != null)
            DialogueManager.Instance.StartDialogue(simpleDialogue);
    }
}