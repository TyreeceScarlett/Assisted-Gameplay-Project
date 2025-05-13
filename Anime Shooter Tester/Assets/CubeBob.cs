using UnityEngine;

public class CubeBob : MonoBehaviour
{
    public float speed = 1f;

    private Vector3 startPos;
    private float moveDistance = 1.822f; // 2.0 - 0.178

    void Start()
    {
        // Set the cube's initial position to Y = 0.178
        startPos = new Vector3(transform.position.x, 0.178f, transform.position.z);
        transform.position = startPos;
    }

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * speed) * (moveDistance / 2f);
        transform.position = startPos + new Vector3(0, yOffset + moveDistance / 2f, 0);
    }
}
