using UnityEngine;

[System.Serializable]
public class DialogueEntry
{
    public string speakerName;
    public Sprite speakerPortrait;
    [TextArea(2, 4)]
    public string[] lines;
}

[CreateAssetMenu(fileName = "NewConversation", menuName = "Dialogue/Conversation")]
public class ConversationData : ScriptableObject
{
    public DialogueEntry[] entries;
}