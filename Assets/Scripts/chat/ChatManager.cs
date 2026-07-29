using System.Collections;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    [SerializeField]
    private ChatUI chatUI;


    private Coroutine chatRoutine;


    public void DisplayChat(DialogueNode node)
    {
        if(chatRoutine != null)
        {
            StopCoroutine(chatRoutine);
        }

        chatRoutine = StartCoroutine(DisplayMessages(node));
    }


    IEnumerator DisplayMessages(DialogueNode node)
    {
        foreach(ChatMessageData message in node.chatMessages)
        {
            yield return new WaitForSeconds(message.delay);

            chatUI.AddMessage(message);
        }
    }


    public void ClearChat()
    {
        chatUI.Clear();
    }
}