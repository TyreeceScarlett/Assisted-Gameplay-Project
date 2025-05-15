using UnityEngine;

public class ControlsHintToggle : MonoBehaviour
{
    public GameObject controlsHintPanel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F4))
        {
            if (controlsHintPanel != null)
            {
                bool isActive = controlsHintPanel.activeSelf;
                controlsHintPanel.SetActive(!isActive);
                Debug.Log("Toggled Controls Hint: " + (!isActive));
            }
            else
            {
                Debug.LogWarning("ControlsHintPanel is not assigned!");
            }
        }
    }
}
