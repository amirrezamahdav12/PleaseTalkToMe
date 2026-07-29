using UnityEngine;


public class ChatUI : MonoBehaviour
{

    [SerializeField]
    private Transform content;


    [SerializeField]
    private ChatMessageUI messagePrefab;



    public void AddMessage(ChatMessageData data)
    {
        Debug.Log("Creating message: " + data.username);

        ChatMessageUI item =
            Instantiate(messagePrefab, content);

        item.Setup(data);
    }



    public void Clear()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }

}