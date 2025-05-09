#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class ForceTPoseInEditor : MonoBehaviour
{
    void Update()
    {
        if (!Application.isPlaying)
        {
            Animator animator = GetComponent<Animator>();
            if (animator)
                animator.enabled = false;  // Force bind pose (T-Pose)
        }
    }
}
#endif
