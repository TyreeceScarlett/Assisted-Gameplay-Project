using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AimAssistUIManager : MonoBehaviour
{
    [Header("Reference to Aim Assist Script")]
    public AimAssist aimAssist;

    [Header("UI Elements")]
    public Toggle aimAssistToggle;
    public Slider rangeSlider;
    public Slider strengthSlider;

    [Header("Optional: Text Labels")]
    public Text rangeValueText;
    public Text strengthValueText;

    void Start()
    {
        // ✅ Sync UI elements with current AimAssist values
        if (aimAssist != null)
        {
            aimAssistToggle.isOn = aimAssist.aimAssistEnabled;
            rangeSlider.value = aimAssist.assistRange;
            strengthSlider.value = aimAssist.assistStrength;

            UpdateRangeText(rangeSlider.value);
            UpdateStrengthText(strengthSlider.value);
        }

        // ✅ Add listeners to UI elements
        aimAssistToggle.onValueChanged.AddListener(OnToggleChanged);
        rangeSlider.onValueChanged.AddListener(OnRangeSliderChanged);
        strengthSlider.onValueChanged.AddListener(OnStrengthSliderChanged);
    }

    // ✅ Called when toggle is changed
    void OnToggleChanged(bool value)
    {
        aimAssist.SetAssistEnabled(value);
    }

    // ✅ Called when range slider is changed
    void OnRangeSliderChanged(float value)
    {
        aimAssist.SetAssistRange(value);
        UpdateRangeText(value);
    }

    // ✅ Called when strength slider is changed
    void OnStrengthSliderChanged(float value)
    {
        aimAssist.SetAssistStrength(value);
        UpdateStrengthText(value);
    }

    // ✅ Optional: Update the value text for range
    void UpdateRangeText(float value)
    {
        if (rangeValueText != null)
            rangeValueText.text = value.ToString("F1");
    }

    // ✅ Optional: Update the value text for strength
    void UpdateStrengthText(float value)
    {
        if (strengthValueText != null)
            strengthValueText.text = value.ToString("F1");
    }
}
