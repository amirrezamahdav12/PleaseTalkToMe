using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(
    fileName = "Portrait Mood Library",
    menuName = "SwitchPrime/Portrait/Mood Library")]
public class PortraitMoodLibrary : ScriptableObject
{
    [SerializeField]
    private List<PortraitMoodEntry> moods = new();

    private Dictionary<OmidMood, VideoClip> cache;

    private void BuildCache()
    {
        if (cache != null)
            return;

        cache = new Dictionary<OmidMood, VideoClip>();

        foreach (PortraitMoodEntry mood in moods)
        {
            if (!cache.ContainsKey(mood.mood))
            {
                cache.Add(mood.mood, mood.clip);
            }
        }
    }

    public VideoClip GetClip(OmidMood mood)
    {
        BuildCache();

        cache.TryGetValue(mood, out VideoClip clip);

        return clip;
    }
}