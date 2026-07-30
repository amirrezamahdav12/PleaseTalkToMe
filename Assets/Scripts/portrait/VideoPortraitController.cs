using UnityEngine;
using UnityEngine.Video;

public class VideoPortraitController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PortraitMoodLibrary moodLibrary;

    [Header("Transition")]
    [SerializeField] private VideoTransitionManager transitionManager;

    private OmidMood currentMood = OmidMood.Natural;

    private void Start()
    {
        VideoClip clip = moodLibrary.GetClip(currentMood);

        transitionManager.Initialize(clip);
    }

    public void SetMood(OmidMood mood)
    {
        Debug.Log($"SetMood => {mood}");
        

        if (mood == currentMood)
            return;

        VideoClip clip = moodLibrary.GetClip(mood);

        Debug.Log($"Clip => {clip}");

        if (clip == null)
        {
            Debug.LogWarning($"No clip registered for mood: {mood}");
            return;
        }

        currentMood = mood;

        transitionManager.Play(clip);
    }
}