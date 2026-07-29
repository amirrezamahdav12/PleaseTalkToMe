using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text notificationText;
    public CanvasGroup canvasGroup;

    public float showTime = 3f;
    public float fadeSpeed = 5f;


    private Coroutine currentRoutine;


    public void ShowNotification(string message)
    {
        if(currentRoutine != null)
            StopCoroutine(currentRoutine);


        notificationText.text = message;

        panel.SetActive(true);

        currentRoutine = StartCoroutine(NotificationRoutine());
    }


    IEnumerator NotificationRoutine()
    {
        // Fade In
        yield return StartCoroutine(
            Fade(0, 1)
        );


        // Wait
        yield return new WaitForSeconds(showTime);


        // Fade Out
        yield return StartCoroutine(
            Fade(1, 0)
        );


        panel.SetActive(false);
    }


    IEnumerator Fade(float start, float end)
    {
        float value = start;


        while(!Mathf.Approximately(value, end))
        {
            value = Mathf.MoveTowards(
                value,
                end,
                fadeSpeed * Time.deltaTime
            );


            canvasGroup.alpha = value;

            yield return null;
        }
    }
}