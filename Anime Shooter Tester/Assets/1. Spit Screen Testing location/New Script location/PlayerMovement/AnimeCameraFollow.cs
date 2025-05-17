using UnityEngine;

public class AnimeCameraFollow : MonoBehaviour
{
    public Transform target; // assign the player transform here
    public Vector3 offset = new Vector3(0, 2, -4);

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.LookAt(target);
        }
    }
}
