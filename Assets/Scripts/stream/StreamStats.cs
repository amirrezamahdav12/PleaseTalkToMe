using UnityEngine;
using TMPro;

public class StreamStats : MonoBehaviour
{
    [SerializeField]
    private TMP_Text viewerText;

    [SerializeField]
    private TMP_Text followerText;


    private int viewers = 1;
    private int followers = 0;


    void Start()
    {
        UpdateUI();
    }


    public void AddViewer(int amount)
    {
        viewers += amount;
        UpdateUI();
    }


    public void RemoveViewer(int amount)
    {
        viewers -= amount;

        if (viewers < 0)
            viewers = 0;

        UpdateUI();
    }


    public void AddFollower(int amount)
    {
        followers += amount;
        UpdateUI();
    }


    void UpdateUI()
    {
        viewerText.text = viewers + " Viewers";
        followerText.text = followers + " Followers";
    }
}