using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class VideoTransitionManager : MonoBehaviour
{
    [Header("Displays")]
    [SerializeField] private PortraitDisplay displayA;
    [SerializeField] private PortraitDisplay displayB;

    [Header("Transition")]
    [SerializeField] private float fadeDuration = 0.25f;

    private PortraitDisplay activeDisplay;
    private PortraitDisplay inactiveDisplay;

    private bool isTransitioning;

    private void Awake()
    {
        activeDisplay = displayA;
        inactiveDisplay = displayB;

        activeDisplay.image.color = Color.white;
        inactiveDisplay.image.color = new Color(1f, 1f, 1f, 0f);

        activeDisplay.player.isLooping = true;
        inactiveDisplay.player.isLooping = true;
    }

    public void Initialize(VideoClip clip)
    {
        if (clip == null)
            return;

        StartCoroutine(InitializeRoutine(clip));
    }

    public void Play(VideoClip clip)
    {
        if (clip == null)
            return;

        if (isTransitioning)
            return;

        StartCoroutine(TransitionRoutine(clip));
    }

    private IEnumerator InitializeRoutine(VideoClip clip)
    {
        activeDisplay.player.Stop();

        activeDisplay.player.clip = clip;

        activeDisplay.player.Prepare();

        while (!activeDisplay.player.isPrepared)
            yield return null;

        activeDisplay.image.texture = activeDisplay.player.targetTexture;

        activeDisplay.player.Play();

        yield return new WaitUntil(() => activeDisplay.player.texture != null);
        yield return new WaitForEndOfFrame();

        activeDisplay.image.color = Color.white;
        inactiveDisplay.image.color = new Color(1f, 1f, 1f, 0f);
    }

    private IEnumerator TransitionRoutine(VideoClip clip)
    {
        isTransitioning = true;

        inactiveDisplay.player.Stop();

        inactiveDisplay.player.clip = clip;

        inactiveDisplay.player.Prepare();

        while (!inactiveDisplay.player.isPrepared)
            yield return null;

        inactiveDisplay.image.texture = inactiveDisplay.player.targetTexture;

        inactiveDisplay.player.Play();

        yield return new WaitUntil(() => inactiveDisplay.player.texture != null);
        yield return new WaitForEndOfFrame();

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float alpha = Mathf.Clamp01(timer / fadeDuration);

            Color activeColor = activeDisplay.image.color;
            activeColor.a = 1f - alpha;
            activeDisplay.image.color = activeColor;

            Color inactiveColor = inactiveDisplay.image.color;
            inactiveColor.a = alpha;
            inactiveDisplay.image.color = inactiveColor;

            yield return null;
        }

        activeDisplay.player.Stop();

        PortraitDisplay temp = activeDisplay;
        activeDisplay = inactiveDisplay;
        inactiveDisplay = temp;

        activeDisplay.image.color = Color.white;
        inactiveDisplay.image.color = new Color(1f, 1f, 1f, 0f);

        isTransitioning = false;
    }

    public bool IsTransitioning => isTransitioning;
}