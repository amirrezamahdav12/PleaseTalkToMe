using UnityEngine;
using System.Collections;

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

    [SerializeField]
    private GameTimer gameTimer;

    [SerializeField]
    private float choiceChatDelay = 3f;

    [SerializeField]
    private NotificationManager notificationManager;

    void Start()
    {
        chatManager.ClearChat();

        StartDialogue("intro_001");

        notificationManager.ShowNotification(
    "Unknown98 followed you"
);
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


        gameTimer.AddTime(
            choice.timeEffect
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
        dialogueUI.DisplayNode(currentNode, this);


        StartCoroutine(
            ShowNodeChatDelayed()
        );

        if (!string.IsNullOrEmpty(currentNode.notificationText))
        {
            notificationManager.ShowNotification(
                currentNode.notificationText
            );
        }
    }

    public void PlayNodeChat()
    {
        chatManager.DisplayChat(currentNode);
    }

    private IEnumerator ExecuteChoice(ChoiceData choice)
    {
        playerStats.ChangeStats(
            choice.hopeChange,
            choice.stressChange,
            choice.trustChange
        );


        gameTimer.AddTime(
            choice.timeEffect
        );


        yield return StartCoroutine(
            chatManager.DisplayChoiceChat(
                choice.responseChat
            )
        );


        StartDialogue(
            choice.nextNodeID
        );
    }

    private IEnumerator ShowNodeChatDelayed()
    {
        yield return new WaitForSeconds(2f);

        chatManager.DisplayChat(currentNode);
    }
}