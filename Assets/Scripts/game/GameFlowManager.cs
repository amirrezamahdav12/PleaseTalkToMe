using System;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    [Header("Panels (assigned in Hierarchy)")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject summaryPanel;

    [Header("Login Fields")]
    [SerializeField] private TMPro.TMP_InputField usernameInput;
    [SerializeField] private TMPro.TMP_InputField passwordInput;

    [Header("Summary Fields")]
    [SerializeField] private TMPro.TMP_Text summaryTitle;
    [SerializeField] private TMPro.TMP_Text summaryDesc;
    [SerializeField] private TMPro.TMP_Text summaryHope;
    [SerializeField] private TMPro.TMP_Text summaryStress;
    [SerializeField] private TMPro.TMP_Text summaryTrust;
    [SerializeField] private TMPro.TMP_Text summaryTip;

    [Header("Gameplay")]
    [SerializeField] private GameObject gameplayCanvas;

    [Header("Managers")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private EndingManager endingManager;

    void Awake()
    {
        if (gameplayCanvas == null)
            gameplayCanvas = GameObject.Find("gameplay");
        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();
        if (gameTimer == null)
            gameTimer = FindObjectOfType<GameTimer>();
        if (dialogueManager == null)
            dialogueManager = FindObjectOfType<DialogueManager>();
        if (endingManager == null)
            endingManager = FindObjectOfType<EndingManager>();

        if (endingManager != null)
            endingManager.OnEndingDetermined += OnGameEnded;

        ShowPanel(mainMenuPanel);
    }

    void OnDestroy()
    {
        if (endingManager != null)
            endingManager.OnEndingDetermined -= OnGameEnded;
    }

    private void ShowPanel(GameObject panel)
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(panel == mainMenuPanel);
        if (loginPanel != null) loginPanel.SetActive(panel == loginPanel);
        if (summaryPanel != null) summaryPanel.SetActive(panel == summaryPanel);
        if (gameplayCanvas != null) gameplayCanvas.SetActive(panel == gameplayCanvas);
    }

    public void OnNewGame()
    {
        ShowPanel(loginPanel);
    }

    public void OnLogin()
    {
        if (usernameInput != null && string.IsNullOrEmpty(usernameInput.text))
            return;

        StartGameplay(usernameInput != null ? usernameInput.text : "Player");
    }

    public void OnBackToMenu()
    {
        ShowPanel(mainMenuPanel);
    }

    public void OnQuit()
    {
        Application.Quit();
    }

    private void StartGameplay(string username)
    {
        ShowPanel(gameplayCanvas);

        if (playerStats != null)
        {
            playerStats.hope = 50;
            playerStats.stress = 50;
            playerStats.trust = 50;
        }

        if (gameTimer != null)
            gameTimer.ResetTime(60f);

        var chatManager = FindObjectOfType<ChatManager>();
        if (chatManager != null) chatManager.ClearChat();

        var endGameUI = FindObjectOfType<EndGameUI>();
        if (endGameUI != null && endGameUI.panel != null)
            endGameUI.panel.SetActive(false);

        var dialogueUI = FindObjectOfType<DialogueUI>();
        if (dialogueUI != null)
        {
            dialogueUI.StopAllCoroutines();
            dialogueUI.HideChoices();
        }

        if (dialogueManager != null)
            dialogueManager.StartDialogue("intro_001");
    }

    private void OnGameEnded(string title, string description)
    {
        ShowPanel(summaryPanel);

        if (summaryTitle != null) summaryTitle.text = title;
        if (summaryDesc != null) summaryDesc.text = description;

        float h = playerStats != null ? playerStats.hope : 0;
        float s = playerStats != null ? playerStats.stress : 0;
        float t = playerStats != null ? playerStats.trust : 0;

        if (summaryHope != null) summaryHope.text = $"Hope:   {h:F0}/100";
        if (summaryStress != null) summaryStress.text = $"Stress: {s:F0}/100";
        if (summaryTrust != null) summaryTrust.text = $"Trust:  {t:F0}/100";

        string tip = h >= 70 && t >= 60 ? "You made a real connection." :
                     h <= 20 || s >= 80 ? "Some nights are harder than others." :
                     "The stream goes on. Tomorrow is another day.";
        if (summaryTip != null) summaryTip.text = tip;
    }
}
