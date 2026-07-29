using UnityEngine;
using TMPro;
using System;


public class GameTimer : MonoBehaviour
{
    [SerializeField]
    private float timeRemaining = 300f;


    [SerializeField]
    private TMP_Text timerText;


    private bool isRunning = true;


    public event Action OnTimeFinished;


    void Start()
    {
        Invoke(nameof(TestEvent), 5);
    }


    void TestEvent()
    {
        AddTime(30);
    }

    void Update()
    {
        if (!isRunning)
            return;


        timeRemaining -= Time.deltaTime;


        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            isRunning = false;

            OnTimeFinished?.Invoke();
        }


        UpdateUI();
    }



    public void AddTime(float amount)
    {
        timeRemaining += amount;
    }



    public void RemoveTime(float amount)
    {
        timeRemaining -= amount;


        if (timeRemaining < 0)
            timeRemaining = 0;
    }



    void UpdateUI()
    {
        int minutes =
            Mathf.FloorToInt(timeRemaining / 60);


        int seconds =
            Mathf.FloorToInt(timeRemaining % 60);



        timerText.text =
            minutes.ToString("00")
            + ":"
            +
            seconds.ToString("00");
    }
}