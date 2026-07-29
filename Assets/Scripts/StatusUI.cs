using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    public PlayerStats stats;
    public Slider hopeSlider;
    public Slider stressSlider;
    public Slider trustSlider;

    void Update()
    {
        hopeSlider.value = stats.hope;
        stressSlider.value = stats.stress;
        trustSlider.value = stats.trust;

    }
}