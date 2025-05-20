using UnityEngine;
using UnityEngine.UI;

public class SmartAimAssistUI : MonoBehaviour
{
    public SmartAimAssist aimAssist;

    [Header("Sliders")]
    public Slider detectionRadiusSlider;
    public Slider aimPullStrengthSlider;
    public Slider minLockOnDistanceSlider;
    public Slider maxLockOnDistanceSlider;
    public Slider maxTargetAngleSlider;
    public Slider lockoutDurationSlider;

    [Header("Text Feedback")]
    public Text detectionRadiusText;
    public Text aimPullStrengthText;
    public Text minLockOnDistanceText;
    public Text maxLockOnDistanceText;
    public Text maxTargetAngleText;
    public Text lockoutDurationText;

    void Start()
    {
        if (aimAssist == null)
        {
            Debug.LogError("SmartAimAssist not assigned!");
            return;
        }

        // Initialize sliders to current values
        detectionRadiusSlider.value = aimAssist.detectionRadius;
        aimPullStrengthSlider.value = aimAssist.aimPullStrength;
        minLockOnDistanceSlider.value = aimAssist.minLockOnDistance;
        maxLockOnDistanceSlider.value = aimAssist.maxLockOnDistance;
        maxTargetAngleSlider.value = aimAssist.maxTargetAngle;
        lockoutDurationSlider.value = aimAssist.lockoutDuration;

        // Set listeners
        detectionRadiusSlider.onValueChanged.AddListener(val =>
        {
            aimAssist.detectionRadius = val;
            detectionRadiusText.text = val.ToString("F1");
        });

        aimPullStrengthSlider.onValueChanged.AddListener(val =>
        {
            aimAssist.aimPullStrength = val;
            aimPullStrengthText.text = val.ToString("F1");
        });

        minLockOnDistanceSlider.onValueChanged.AddListener(val =>
        {
            aimAssist.minLockOnDistance = val;
            minLockOnDistanceText.text = val.ToString("F1");
        });

        maxLockOnDistanceSlider.onValueChanged.AddListener(val =>
        {
            aimAssist.maxLockOnDistance = val;
            maxLockOnDistanceText.text = val.ToString("F1");
        });

        maxTargetAngleSlider.onValueChanged.AddListener(val =>
        {
            aimAssist.maxTargetAngle = val;
            maxTargetAngleText.text = val.ToString("F1");
        });

        lockoutDurationSlider.onValueChanged.AddListener(val =>
        {
            aimAssist.lockoutDuration = val;
            lockoutDurationText.text = val.ToString("F1");
        });

        // Manually trigger updates to set initial text
        detectionRadiusSlider.onValueChanged.Invoke(aimAssist.detectionRadius);
        aimPullStrengthSlider.onValueChanged.Invoke(aimAssist.aimPullStrength);
        minLockOnDistanceSlider.onValueChanged.Invoke(aimAssist.minLockOnDistance);
        maxLockOnDistanceSlider.onValueChanged.Invoke(aimAssist.maxLockOnDistance);
        maxTargetAngleSlider.onValueChanged.Invoke(aimAssist.maxTargetAngle);
        lockoutDurationSlider.onValueChanged.Invoke(aimAssist.lockoutDuration);
    }
}
