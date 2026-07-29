using UnityEngine;


public class PlayerStats : MonoBehaviour
{
    public float hope = 50;

    public float stress = 50;

    public float trust = 50;



    public void ChangeStats(
        float hopeAmount,
        float stressAmount,
        float trustAmount
    )
    {
        hope += hopeAmount;

        stress += stressAmount;

        trust += trustAmount;



        hope = Mathf.Clamp(hope, 0, 100);

        stress = Mathf.Clamp(stress, 0, 100);

        trust = Mathf.Clamp(trust, 0, 100);



        Debug.Log(
            $"Hope:{hope} Stress:{stress} Trust:{trust}"
        );
    }
}