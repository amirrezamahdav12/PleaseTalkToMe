using UnityEngine;
using TMPro;

public class ViewerManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text viewerText;


    [SerializeField]
    private int currentViewers = 12;


    void Start()
    {
        UpdateUI();
        AddViewer(5);
    }


    public void AddViewer(int amount)
    {
        currentViewers += amount;

        UpdateUI();
    }


    public void RemoveViewer(int amount)
    {
        currentViewers -= amount;

        if (currentViewers < 0)
            currentViewers = 0;

        UpdateUI();
    }


    void UpdateUI()
    {
        viewerText.text = currentViewers + " Viewers";
    }
}