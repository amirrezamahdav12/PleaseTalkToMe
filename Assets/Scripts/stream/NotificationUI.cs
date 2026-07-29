using TMPro;
using UnityEngine;

public class NotificationUI : MonoBehaviour
{
    [SerializeField]
    private GameObject panel;

    [SerializeField]
    private TMP_Text titleText;

    [SerializeField]
    private TMP_Text messageText;


    public void Show(
        string title,
        string message
    )
    {
        panel.SetActive(true);

        titleText.text = title;
        messageText.text = message;
    }


    public void Hide()
    {
        panel.SetActive(false);
    }
}