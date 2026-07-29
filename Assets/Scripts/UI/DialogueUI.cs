using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DialogueUI : MonoBehaviour
{

    public TMP_Text speakerText;

    public TMP_Text dialogueText;

    public Image portraitImage;


    public ChoiceButtonUI[] choiceButtons;



    public void DisplayNode(
        DialogueNode node,
        DialogueManager manager
    )
    {

        speakerText.text = node.speaker;

        dialogueText.text = node.dialogue;


        portraitImage.sprite = node.portrait;



        for(int i=0;i<choiceButtons.Length;i++)
        {

            if(i < node.choices.Count)
            {

                choiceButtons[i].gameObject.SetActive(true);


                choiceButtons[i].Initialize(
                    manager,
                    i,
                    node.choices[i].text
                );

            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }

    }

}