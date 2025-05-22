using UnityEngine;

public class AimAssistPanelToggle : MonoBehaviour
{
    [Header("Aim Assist UI")]
    public GameObject aimAssistPanel;

    private void Update()
    {
        // F5 toggles the Aim Assist panel
        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (aimAssistPanel != null)
                aimAssistPanel.SetActive(!aimAssistPanel.activeSelf);
        }
    }
}
