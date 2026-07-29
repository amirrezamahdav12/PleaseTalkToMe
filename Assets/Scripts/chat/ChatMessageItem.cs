using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatMessageItem : MonoBehaviour
{
    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Image avatar;

    public void Initialize(ChatMessage message)
    {
        usernameText.text = message.username;
        messageText.text = message.message;
    }
}