using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ChoiceButtonUI : MonoBehaviour
{

    public TMP_Text label;


    DialogueManager manager;

    int index;



    public void Initialize(
        DialogueManager dialogueManager,
        int choiceIndex,
        string text
    )
    {

        manager = dialogueManager;

        index = choiceIndex;


        label.text = text;


        Button button = GetComponent<Button>();

        button.onClick.RemoveAllListeners();

        button.onClick.AddListener(OnClick);

    }



    void OnClick()
    {
        manager.SelectChoice(index);
    }

}