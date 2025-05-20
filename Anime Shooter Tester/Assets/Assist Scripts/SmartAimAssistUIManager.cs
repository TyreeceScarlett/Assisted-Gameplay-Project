using UnityEngine;
using UnityEngine.UI;

public class SmartAimAssistUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject aimAssistPanelShow;
    public GameObject aimAssistScrollView;
    public Toggle maximizeToggle;

    private void Start()
    {
        if (maximizeToggle != null)
        {
            maximizeToggle.onValueChanged.AddListener(OnMaximizeToggled);
        }

        // Optional: initialize panel as hidden
        if (aimAssistPanelShow != null)
            aimAssistPanelShow.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (aimAssistPanelShow != null)
                aimAssistPanelShow.SetActive(!aimAssistPanelShow.activeSelf);
        }
    }

    void OnMaximizeToggled(bool isOn)
    {
        if (aimAssistScrollView != null)
            aimAssistScrollView.SetActive(isOn);
    }
}
