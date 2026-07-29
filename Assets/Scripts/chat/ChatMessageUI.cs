using TMPro;
using UnityEngine;


public class ChatMessageUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text usernameText;

    [SerializeField]
    private TMP_Text messageText;


    public void Setup(ChatMessageData data)
    {
        usernameText.text = data.username;
        messageText.text = data.message;
    }
}