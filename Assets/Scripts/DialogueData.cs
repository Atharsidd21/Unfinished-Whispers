using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Single Dialogue")]
public class DialogueData : ScriptableObject
{
    public string speakerName;
    public Sprite speakerPortrait;
    [TextArea(2, 4)]
    public string[] lines;
}