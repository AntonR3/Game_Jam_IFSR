using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class CleaningSlider : MonoBehaviour
{
    public Slider slider;
    public TMP_Text percentageText;

    [Tooltip("Dauer der Interpolation in Sekunden")]
    public float lerpDuration = 0.5f;

    private Coroutine lerpCoroutine;
    private int displayedPercent = 0;

    private void Start()
    {
        if (slider == null)
        {
            slider = GetComponentInChildren<Slider>();
            if (slider == null)
                Debug.LogWarning("CleaningSlider: Kein Slider zugewiesen und keiner in den Kindern gefunden.");
        }

        if (slider != null)
            slider.value = 0f; // Initialize the slider value to 0

        displayedPercent = 0;
        if (percentageText != null)
            percentageText.text = $"{displayedPercent}%";
    }

    public void AnimateTo(float target)
    {
        if (slider == null) return;
        target = Mathf.Clamp01(target);
        if (lerpCoroutine != null)
        {
            StopCoroutine(lerpCoroutine);
        }
        lerpCoroutine = StartCoroutine(LerpSlider(slider.value, target, lerpDuration));
    }

    private IEnumerator LerpSlider(float from, float to, float duration)
    {
        if (duration <= 0f)
        {
            slider.value = to;
            UpdateProgressDisplayImmediate();
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            slider.value = Mathf.Lerp(from, to, t);

            UpdateProgressDisplayImmediate();

            yield return null;
        }
        slider.value = to;
        UpdateProgressDisplayImmediate();
        lerpCoroutine = null;
    }
    private void UpdateProgressDisplayImmediate()
    {
        if (percentageText == null || slider == null)
            return;

        int targetPercent = Mathf.RoundToInt(slider.value * 100f);
        if (targetPercent == displayedPercent)
            return;

        displayedPercent = targetPercent;
        percentageText.text = $"{displayedPercent}%";
    }
}