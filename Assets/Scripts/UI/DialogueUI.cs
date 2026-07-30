using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text speakerText;
    public TMP_Text dialogueText;
    public Image portraitImage;

    [Header("Choices")]
    public ChoiceButtonUI[] choiceButtons;

    [Header("Typewriter")]
    public float typingSpeed = 0.03f;

    private Coroutine typingCoroutine;
    private bool isTyping;
    private string currentDialogue;

    private DialogueNode currentNode;
    private DialogueManager currentManager;

    // Input System
    private InputSystem_Actions input;

    private void Awake()
    {
        input = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Update()
    {
        if (!isTyping)
            return;

        if (input.UI.SkipDialogue.WasPressedThisFrame())
        {
            SkipTyping();
        }
    }

    public void DisplayNode(DialogueNode node, DialogueManager manager)
    {
        currentNode = node;
        currentManager = manager;

        speakerText.text = node.speaker;
        portraitImage.sprite = node.portrait;

        HideChoices();

        StartTyping(node.dialogue);
    }

    private void StartTyping(string text)
    {
        currentDialogue = text;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeWriter());
    }

    private IEnumerator TypeWriter()
    {
        isTyping = true;

        dialogueText.text = currentDialogue;

        dialogueText.ForceMeshUpdate();

        dialogueText.maxVisibleCharacters = 0;

        int totalCharacters = dialogueText.textInfo.characterCount;

        for (int i = 0; i <= totalCharacters; i++)
        {
            dialogueText.maxVisibleCharacters = i;

            yield return new WaitForSeconds(typingSpeed);
        }

        dialogueText.maxVisibleCharacters = totalCharacters;

        isTyping = false;

        ShowChoices();
    }

    private void SkipTyping()
    {
        if (!isTyping)
            return;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.maxVisibleCharacters =
            dialogueText.textInfo.characterCount;

        isTyping = false;

        ShowChoices();
    }

    private void HideChoices()
    {
        foreach (ChoiceButtonUI button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    private void ShowChoices()
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < currentNode.choices.Count)
            {
                choiceButtons[i].gameObject.SetActive(true);

                choiceButtons[i].Initialize(
                    currentManager,
                    i,
                    currentNode.choices[i].text
                );
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }
}