using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int hope = 50;
    public int stress = 50;
    public int trust = 30;


    public void ChangeStats(int hopeValue, int stressValue, int trustValue)
    {
        hope += hopeValue;
        stress += stressValue;
        trust += trustValue;

        hope = Mathf.Clamp(hope,0,100);
        stress = Mathf.Clamp(stress,0,100);
        trust = Mathf.Clamp(trust,0,100);
    }
}