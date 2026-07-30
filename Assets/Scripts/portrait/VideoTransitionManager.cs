using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class VideoTransitionManager : MonoBehaviour
{
    [Header("Displays")]
    [SerializeField] private PortraitDisplay displayA;
    [SerializeField] private PortraitDisplay displayB;

    [Header("Fade")]
    [SerializeField] private float fadeDuration = 0.35f;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Scale Pulse")]
    [SerializeField] private bool enableScalePulse = true;
    [SerializeField] private float scalePulseAmount = 0.04f;
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private PortraitDisplay activeDisplay;
    private PortraitDisplay inactiveDisplay;

    private bool isTransitioning;
    private VideoClip preWarmedClip;

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

    public void PreWarm(VideoClip clip)
    {
        if (clip == null)
            return;

        if (preWarmedClip == clip)
            return;

        StartCoroutine(PreWarmRoutine(clip));
    }

    private IEnumerator InitializeRoutine(VideoClip clip)
    {
        yield return PrepareClip(activeDisplay, clip);
        activeDisplay.player.Play();

        activeDisplay.image.texture = activeDisplay.player.targetTexture;

        yield return new WaitUntil(() => activeDisplay.player.texture != null);
        yield return new WaitForEndOfFrame();

        activeDisplay.image.color = Color.white;
        activeDisplay.image.transform.localScale = Vector3.one;
        inactiveDisplay.image.color = new Color(1f, 1f, 1f, 0f);
        inactiveDisplay.image.transform.localScale = Vector3.one;
    }

    private IEnumerator PreWarmRoutine(VideoClip clip)
    {
        yield return PrepareClip(inactiveDisplay, clip);

        inactiveDisplay.image.texture = inactiveDisplay.player.targetTexture;

        yield return new WaitUntil(() => inactiveDisplay.player.texture != null);
        yield return new WaitForEndOfFrame();

        preWarmedClip = clip;
    }

    private IEnumerator TransitionRoutine(VideoClip clip)
    {
        isTransitioning = true;

        if (preWarmedClip == clip)
        {
            inactiveDisplay.player.Play();
            preWarmedClip = null;
        }
        else
        {
            yield return PrepareClip(inactiveDisplay, clip);
            inactiveDisplay.player.Play();

            inactiveDisplay.image.texture = inactiveDisplay.player.targetTexture;

            yield return new WaitUntil(() => inactiveDisplay.player.texture != null);
            yield return new WaitForEndOfFrame();
        }

        if (activeDisplay.image.texture == null)
        {
            SwapDisplays();
            isTransitioning = false;
            yield break;
        }

        Vector3 originalScale = activeDisplay.image.transform.localScale;

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / fadeDuration);
            float easedT = fadeCurve.Evaluate(t);

            Color activeColor = activeDisplay.image.color;
            activeColor.a = 1f - easedT;
            activeDisplay.image.color = activeColor;

            Color inactiveColor = inactiveDisplay.image.color;
            inactiveColor.a = easedT;
            inactiveDisplay.image.color = inactiveColor;

            if (enableScalePulse)
            {
                float scaleT = scaleCurve.Evaluate(t);
                float pulse = 1f + scalePulseAmount * Mathf.Sin(scaleT * Mathf.PI);
                inactiveDisplay.image.transform.localScale = originalScale * pulse;
            }

            yield return null;
        }

        activeDisplay.player.Stop();

        SwapDisplays();

        activeDisplay.image.color = Color.white;
        activeDisplay.image.transform.localScale = Vector3.one;
        inactiveDisplay.image.color = new Color(1f, 1f, 1f, 0f);
        inactiveDisplay.image.transform.localScale = Vector3.one;

        isTransitioning = false;
    }

    private void SwapDisplays()
    {
        PortraitDisplay temp = activeDisplay;
        activeDisplay = inactiveDisplay;
        inactiveDisplay = temp;
    }

    private static IEnumerator PrepareClip(PortraitDisplay display, VideoClip clip)
    {
        display.player.Stop();
        display.player.clip = clip;
        display.player.Prepare();

        while (!display.player.isPrepared)
            yield return null;
    }

    public bool IsTransitioning => isTransitioning;
}
