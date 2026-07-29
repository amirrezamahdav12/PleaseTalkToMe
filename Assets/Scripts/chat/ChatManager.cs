using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChatManager : MonoBehaviour
{
    [SerializeField]
    private ChatUI chatUI;


    private Coroutine chatRoutine;



    public void DisplayChat(DialogueNode node)
    {
        if (chatRoutine != null)
        {
            StopCoroutine(chatRoutine);
        }


        chatRoutine = StartCoroutine(
            DisplayMessages(node.chatMessages)
        );
    }



public IEnumerator DisplayChoiceChat(List<ChatMessageData> messages)
{
    if(messages == null || messages.Count == 0)
        yield break;


    foreach(ChatMessageData message in messages)
    {
        yield return new WaitForSeconds(message.delay);

        chatUI.AddMessage(message);
    }
}



    private IEnumerator DisplayMessages(List<ChatMessageData> messages)
    {
        foreach (ChatMessageData message in messages)
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