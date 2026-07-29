using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public DialogueDatabase database;

    public DialogueNode currentNode;

    public DialogueUI dialogueUI;

    public PlayerStats playerStats;

    [SerializeField]
    private EndGameUI endGameUI;

    [SerializeField]
    private ChatManager chatManager;


    void Start()
    {
        chatManager.ClearChat();

        StartDialogue("intro_001");
    }

    public void StartDialogue(string nodeID)
    {
        currentNode = GetNode(nodeID);

        if (currentNode == null)
        {
            Debug.LogError("Node not found: " + nodeID);
            return;
        }


        ShowCurrentNode();
    }


    public void SelectChoice(int index)
    {
        ChoiceData choice = currentNode.choices[index];


        playerStats.ChangeStats(
            choice.hopeChange,
            choice.stressChange,
            choice.trustChange
        );


        StartDialogue(choice.nextNodeID);
    }


    DialogueNode GetNode(string id)
    {
        foreach (DialogueNode node in database.nodes)
        {
            if (node.nodeID == id)
                return node;
        }

        return null;
    }


    void ShowCurrentNode()
    {
        dialogueUI.DisplayNode(
            currentNode,
            this
        );

        chatManager.DisplayChat(currentNode);
    }
}