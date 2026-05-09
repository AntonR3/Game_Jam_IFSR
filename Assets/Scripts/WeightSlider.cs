using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class WeightSlider : MonoBehaviour
{
    [SerializeField] private Color emptyColor = Color.green;
    [SerializeField] private Color fullColor = Color.red;
    [SerializeField] private TextMeshProUGUI maxWeightText;
    [SerializeField] private TextMeshProUGUI currentWeightText;
    [SerializeField] private float currentWeightTextOffsetX = 120f;
    [SerializeField] private float currentWeightTextOffsetYMin = -240f;
    [SerializeField] private float currentWeightTextOffsetYMax = 260f;

    private Slider slider;
    private Image fillImage;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        if (slider.fillRect != null)
        {
            fillImage = slider.fillRect.GetComponent<Image>();
        }
    }

    private void OnEnable()
    {
        if (slider != null)
        {
            slider.onValueChanged.AddListener(UpdateColor);
            UpdateColor(slider.value);
        }
    }

    public void Update()
    {
        if (slider == null)
        {
            return;
        }

        if (maxWeightText != null)
        {
            maxWeightText.text = slider.maxValue.ToString();
        }

        if (currentWeightText != null)
        {
            String text = "";
            if (slider.value < slider.maxValue)
            {
                text = Mathf.RoundToInt(slider.maxValue - slider.value).ToString();
            }
            else
            {
                text = "Overweight";
            }
            currentWeightText.text = text;
            RectTransform currentWeightTextRect = currentWeightText.rectTransform;

            float normalized = Mathf.InverseLerp(slider.minValue, slider.maxValue, slider.value);
            float y = Mathf.Lerp(currentWeightTextOffsetYMin, currentWeightTextOffsetYMax, normalized);

            Vector3 basePos;
            if (slider.handleRect != null)
            {
                basePos = slider.handleRect.position;
            }
            else
            {
                // No handle (flat slider) — compute a world position along the slider rect
                RectTransform sliderRect = slider.GetComponent<RectTransform>();
                Vector3[] corners = new Vector3[4];
                sliderRect.GetWorldCorners(corners);
                Vector3 leftMid = (corners[0] + corners[1]) * 0.5f;
                Vector3 rightMid = (corners[2] + corners[3]) * 0.5f;
                basePos = Vector3.Lerp(leftMid, rightMid, normalized);
            }

            // Use anchoredPosition so X remains constant in the UI layout; only Y is interpolated.
            currentWeightTextRect.anchoredPosition = new Vector2(currentWeightTextOffsetX, y);
        }

        // This is a fallback in case the slider value is changed programmatically without invoking the event
        UpdateColor(slider.value);
    }

    private void OnDisable()
    {
        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(UpdateColor);
        }
    }

    private void UpdateColor(float value)
    {
        if (fillImage == null || slider == null)
        {
            return;
        }

        float normalizedValue = Mathf.InverseLerp(slider.minValue, slider.maxValue, value);
        fillImage.color = Color.Lerp(emptyColor, fullColor, normalizedValue);
    }
}
