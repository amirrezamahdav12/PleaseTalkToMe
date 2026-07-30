using UnityEngine;

public class EndingManager : MonoBehaviour
{
    public PlayerStats playerStats;

    public EndGameUI endingUI;


    public void CheckEnding()
    {
        float hope = playerStats.hope;
        float stress = playerStats.stress;
        float trust = playerStats.trust;


        if (hope >= 70 && trust >= 60)
        {
            ShowEnding(
                "Someone Stayed",
                "Sometimes one person staying is enough."
            );
        }

        else if (hope <= 20 || stress >= 80)
        {
            ShowEnding(
                "Nobody Answered",
                "Some signals disappear quietly."
            );
        }

        else
        {
            ShowEnding(
                "The Stream Continues",
                "Not every story ends today."
            );
        }
    }


    void ShowEnding(string title, string description)
    {
        endingUI.ShowEnding(title, description);
    }
}