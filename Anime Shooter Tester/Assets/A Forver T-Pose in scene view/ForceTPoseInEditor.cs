#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

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
        if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
        {
            if (animator)
            {
                if (AnimatorIsInPreviewMode(animator))
                {
                    // Do nothing if previewing animation
                }
                else
                {
                    if (animator.enabled)
                        animator.enabled = false;  // Force T-Pose after preview
                }
            }
        }
        else
        {
            if (animator && !animator.enabled)
                animator.enabled = true;  // Re-enable during Play Mode
        }
    }

    bool AnimatorIsInPreviewMode(Animator anim)
    {
        return AnimationMode.InAnimationMode();  // Detects if Preview button is active
    }

    // Method to force the T-Pose by searching for the "T-Pose" animation clip
    public void ForceTPose()
    {
        if (animator)
        {
            // Look for a state named "T-Pose" in the Animator
            var controller = animator.runtimeAnimatorController as AnimatorController;
            if (controller != null)
            {
                foreach (var layer in controller.layers)
                {
                    foreach (var state in layer.stateMachine.states)
                    {
                        // Check if the state name is "T-Pose"
                        if (state.state.name == "T-Pose")
                        {
                            animator.Play("T-Pose");
                            Debug.Log("T-Pose animation found and played.");
                            return;
                        }
                    }
                }
            }
            Debug.LogWarning("T-Pose animation not found in the Animator.");
        }
    }

    [CustomEditor(typeof(ForceTPoseInEditor))]
    public class ForceTPoseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ForceTPoseInEditor forceTPoseScript = (ForceTPoseInEditor)target;

            if (GUILayout.Button("Force T-Pose"))
            {
                forceTPoseScript.ForceTPose(); // Calls the ForceTPose method when button is pressed
            }
        }
    }
}
#endif
