using UnityEngine;
using UnityEngine.UI;

public class BetterAimAssistUI : MonoBehaviour
{
    public BetterAimAssist aimAssist;

    public Toggle toggleStickyView;
    public Toggle toggleAssistedTracking;
    public Toggle toggleADSSnapping;
    public Toggle toggleBulletMagnetism;

    void Start()
    {
        if (aimAssist == null) Debug.LogError("Assign BetterAimAssist!");

        // Initialize toggles
        toggleStickyView.isOn = aimAssist.enableStickyView;
        toggleAssistedTracking.isOn = aimAssist.enableAssistedTracking;
        toggleADSSnapping.isOn = aimAssist.enableADSSnapping;
        toggleBulletMagnetism.isOn = aimAssist.enableBulletMagnetism;

        // Add listeners
        toggleStickyView.onValueChanged.AddListener((val) => aimAssist.enableStickyView = val);
        toggleAssistedTracking.onValueChanged.AddListener((val) => aimAssist.enableAssistedTracking = val);
        toggleADSSnapping.onValueChanged.AddListener((val) => aimAssist.enableADSSnapping = val);
        toggleBulletMagnetism.onValueChanged.AddListener((val) => aimAssist.enableBulletMagnetism = val);
    }

}