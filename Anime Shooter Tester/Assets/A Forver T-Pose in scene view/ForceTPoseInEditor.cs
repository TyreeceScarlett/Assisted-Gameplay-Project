#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class ForceTPoseInEditor : MonoBehaviour
{
    private Animator animator;

    void OnEnable()
    {
        animator = GetComponent<Animator>();
        ForceTPose();
    }

    void Update()
    {
        ForceTPose();
    }

    void ForceTPose()
    {
        if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
        {
            if (animator && animator.enabled)
                animator.enabled = false;  // Disable animator in Scene View (edit mode only)
        }
        else
        {
            if (animator && !animator.enabled)
                animator.enabled = true;  // Enable animator during Play Mode
        }
    }
}
#endif