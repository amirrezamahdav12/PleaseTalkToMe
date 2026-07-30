using TMPro;
using UnityEngine;

public class EndGameUI : MonoBehaviour
{
    public GameObject panel;

    public TMP_Text titleText;
    public TMP_Text descriptionText;


    public void ShowEnding(
        string title,
        string description
    )
    {
        panel.SetActive(true);

        titleText.text = title;
        descriptionText.text = description;
    }
}