using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class BlackWhiteToggle : MonoBehaviour
{
    public PostProcessVolume postProcessVolume; // Reference to the PostProcessVolume
    private ColorGrading colorGrading;          // Reference to the ColorGrading settings

    void Start()
    {
        // Check if the PostProcessVolume is assigned in the inspector
        if (postProcessVolume == null)
        {
            Debug.LogError("PostProcessVolume is not assigned in the inspector!");
            return;
        }

        // Get the ColorGrading effect from the PostProcessVolume profile
        if (!postProcessVolume.profile.TryGetSettings(out colorGrading))
        {
            Debug.LogError("ColorGrading effect not found in the PostProcessProfile!");
        }
    }

    public void ToggleBlackWhite()
    {
        if (colorGrading == null)
        {
            Debug.LogError("ColorGrading effect is not available!");
            return;
        }

        // Toggle saturation between 0 (black & white) and 100 (normal color)
        if (colorGrading.saturation.value == 0f)
        {
            colorGrading.saturation.value = 100f; // Restore normal color saturation
        }
        else
        {
            colorGrading.saturation.value = 0f; // Set saturation to 0 for black & white
        }
    }
}
