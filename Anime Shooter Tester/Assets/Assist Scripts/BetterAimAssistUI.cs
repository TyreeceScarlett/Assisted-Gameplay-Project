using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AimAssistUIManager : MonoBehaviour
{
    public BetterAimAssist aimAssist;

    [Header("Sticky View Settings")]
    public Toggle stickyToggle;
    public Slider stickyRadiusSlider;
    public Slider stickySlowdownSlider;
    public Slider stickyAngleSlider;
    public TMP_Text stickyRadiusText;
    public TMP_Text stickySlowdownText;
    public TMP_Text stickyAngleText;

    [Header("Assisted Tracking Settings")]
    public Toggle trackingToggle;
    public Slider trackingRadiusSlider;
    public Slider trackingSpeedSlider;
    public Slider trackingAngleSlider;
    public TMP_Text trackingRadiusText;
    public TMP_Text trackingSpeedText;
    public TMP_Text trackingAngleText;

    [Header("ADS Snapping Settings")]
    public Toggle adsToggle;
    public Slider adsRadiusSlider;
    public Slider adsSpeedSlider;
    public TMP_Text adsRadiusText;
    public TMP_Text adsSpeedText;

    [Header("Bullet Magnetism Settings")]
    public Toggle magnetismToggle;
    public Slider magnetRadiusSlider;
    public Slider magnetStrengthSlider;
    public Slider bulletSpeedSlider;
    public TMP_Text magnetRadiusText;
    public TMP_Text magnetStrengthText;
    public TMP_Text bulletSpeedText;

    [Header("Canvas Toggle")]
    public GameObject aimAssistCanvas;
    public Toggle showUIToggle;
    public Toggle hideUIToggle;

    [Header("Reset Button")]
    public Button resetButton;

    void Start()
    {
        if (aimAssist == null)
        {
            Debug.LogError("AimAssistUIManager: 'aimAssist' reference is missing!");
            enabled = false;
            return;
        }

        showUIToggle.onValueChanged.AddListener(OnShowUIToggleChanged);
        hideUIToggle.onValueChanged.AddListener(OnHideUIToggleChanged);
        resetButton.onClick.AddListener(ResetToDefaults);

        stickyToggle.onValueChanged.AddListener(value => aimAssist.enableStickyView = value);
        trackingToggle.onValueChanged.AddListener(value => aimAssist.enableAssistedTracking = value);
        adsToggle.onValueChanged.AddListener(value => aimAssist.enableADSSnapping = value);
        magnetismToggle.onValueChanged.AddListener(value => aimAssist.enableBulletMagnetism = value);
    }

    void OnShowUIToggleChanged(bool isOn)
    {
        if (isOn)
        {
            aimAssistCanvas.SetActive(true);
            hideUIToggle.isOn = false;
        }
    }

    void OnHideUIToggleChanged(bool isOn)
    {
        if (isOn)
        {
            aimAssistCanvas.SetActive(false);
            showUIToggle.isOn = false;
        }
    }

    void Update()
    {
        if (aimAssist == null) return;

        // Sticky View
        aimAssist.stickyDetectionRadius = stickyRadiusSlider.value;
        aimAssist.stickySlowDownFactor = stickySlowdownSlider.value;
        aimAssist.stickyMaxAngle = stickyAngleSlider.value;
        stickyRadiusText.text = stickyRadiusSlider.value.ToString("F1");
        stickySlowdownText.text = stickySlowdownSlider.value.ToString("F2");
        stickyAngleText.text = stickyAngleSlider.value.ToString("F0");

        // Assisted Tracking
        aimAssist.trackingDetectionRadius = trackingRadiusSlider.value;
        aimAssist.trackingSpeed = trackingSpeedSlider.value;
        aimAssist.trackingMaxAngle = trackingAngleSlider.value;
        trackingRadiusText.text = trackingRadiusSlider.value.ToString("F1");
        trackingSpeedText.text = trackingSpeedSlider.value.ToString("F1");
        trackingAngleText.text = trackingAngleSlider.value.ToString("F0");

        // ADS Snapping
        aimAssist.adsDetectionRadius = adsRadiusSlider.value;
        aimAssist.adsSnapSpeed = adsSpeedSlider.value;
        adsRadiusText.text = adsRadiusSlider.value.ToString("F1");
        adsSpeedText.text = adsSpeedSlider.value.ToString("F1");

        // Bullet Magnetism
        aimAssist.magnetRadius = magnetRadiusSlider.value;
        aimAssist.magnetStrength = magnetStrengthSlider.value;
        aimAssist.bulletSpeed = bulletSpeedSlider.value;
        magnetRadiusText.text = magnetRadiusSlider.value.ToString("F1");
        magnetStrengthText.text = magnetStrengthSlider.value.ToString("F0");
        bulletSpeedText.text = bulletSpeedSlider.value.ToString("F0");
    }

    void ResetToDefaults()
    {
        stickyToggle.isOn = true;
        stickyRadiusSlider.value = 20f;
        stickySlowdownSlider.value = 0.3f;
        stickyAngleSlider.value = 10f;

        trackingToggle.isOn = true;
        trackingRadiusSlider.value = 20f;
        trackingSpeedSlider.value = 5f;
        trackingAngleSlider.value = 30f;

        adsToggle.isOn = true;
        adsRadiusSlider.value = 30f;
        adsSpeedSlider.value = 10f;

        magnetismToggle.isOn = true;
        magnetRadiusSlider.value = 2f;
        magnetStrengthSlider.value = 1000f;
        bulletSpeedSlider.value = 50f;
    }
}
