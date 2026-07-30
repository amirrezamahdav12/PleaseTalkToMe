using System;
using UnityEngine;

public class EndingManager : MonoBehaviour
{
    public PlayerStats playerStats;

    public EndGameUI endingUI;

    public event Action<string, string> OnEndingDetermined;


    public void CheckEnding()
    {
        float hope = playerStats.hope;
        float stress = playerStats.stress;
        float trust = playerStats.trust;


        string title, description;

        if (hope >= 70 && trust >= 60)
        {
            title = "Someone Stayed";
            description = "Sometimes one person staying is enough.";
        }
        else if (hope <= 20 || stress >= 80)
        {
            title = "Nobody Answered";
            description = "Some signals disappear quietly.";
        }
        else
        {
            title = "The Stream Continues";
            description = "Not every story ends today.";
        }

        OnEndingDetermined?.Invoke(title, description);

        if (endingUI != null)
            endingUI.ShowEnding(title, description);
    }
}