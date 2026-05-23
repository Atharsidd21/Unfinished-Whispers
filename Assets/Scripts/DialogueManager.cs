using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI References")]
    public GameObject dialogueBox;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public UnityEngine.UI.Image portrait;
    private bool ignoreNextInput;
    private string[] currentLines;
    private int currentLineIndex;
    private bool isActive = false;



    // For conversations
    private ConversationData currentConversation;
    private int currentEntryIndex;

    // Public property so Player.cs can check if dialogue is open
    public bool IsActive => isActive;

    void Awake()
    {
        Instance = this;
        dialogueBox.SetActive(false);
    }

    // ─────────────────────────────────────────
    //  SIMPLE DIALOGUE
    // ─────────────────────────────────────────
    public void StartDialogue(DialogueData data)
    {
        isActive = true;
        currentLines = data.lines;
        currentLineIndex = 0;

        nameText.text = data.speakerName;
        if (portrait != null && data.speakerPortrait != null)
            portrait.sprite = data.speakerPortrait;

        dialogueBox.SetActive(true);

        ignoreNextInput = true;

        ShowLine();

    }

    // ─────────────────────────────────────────
    //  CONVERSATION
    // ─────────────────────────────────────────
    public void StartConversation(ConversationData data)
    {
        currentConversation = data;
        currentEntryIndex = 0;
        LoadCurrentEntry();
    }

    void LoadCurrentEntry()
    {
        DialogueEntry entry = currentConversation.entries[currentEntryIndex];

        isActive = true;
        currentLines = entry.lines;
        currentLineIndex = 0;

        nameText.text = entry.speakerName;
        if (portrait != null && entry.speakerPortrait != null)
            portrait.sprite = entry.speakerPortrait;

        dialogueBox.SetActive(true);
        ignoreNextInput = true;
        ShowLine();

    }

    // ─────────────────────────────────────────
    //  SHARED LOGIC
    // ─────────────────────────────────────────
    void ShowLine()
    {
        dialogueText.text = currentLines[currentLineIndex];
        //   Debug.Log("Showing: " + currentLines[currentLineIndex]); // temp debug

    }

    void Update()
    {
        if (!isActive) return;


        if (Input.GetKeyUp(KeyCode.Z) || Input.GetKeyUp(KeyCode.Return))
        {
            // Ignore the release from the interaction press
            if (ignoreNextInput)
            {
                ignoreNextInput = false;
                return;
            }

            AdvanceDialogue();
        }
    }

    void AdvanceDialogue()
    {
        currentLineIndex++;

        // More lines in current entry?
        if (currentLineIndex < currentLines.Length)
        {
            ShowLine();
            return;
        }

        // In a conversation, more entries?
        if (currentConversation != null)
        {
            currentEntryIndex++;
            if (currentEntryIndex < currentConversation.entries.Length)
            {
                LoadCurrentEntry();
                return;
            }
        }

        // Nothing left — close
        EndDialogue();
    }

    void EndDialogue()
    {
        isActive = false;


        currentConversation = null;
        dialogueBox.SetActive(false);
    }
}