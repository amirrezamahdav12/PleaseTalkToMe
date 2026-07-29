using TMPro;
using UnityEngine;

public class EndGameUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text summaryText;

    public void Show(PlayerStats stats, string message)
    {
        panel.SetActive(true);

        summaryText.text =
            message +
            "\n\nHope : " + stats.hope +
            "\nStress : " + stats.stress +
            "\nTrust : " + stats.trust;
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}