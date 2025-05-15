using UnityEngine;
using TMPro;

public class FloatingDamageText : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float lifetime = 1f;
    private TextMeshProUGUI textMesh;

    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
    }

    public void SetText(string text, Color color)
    {
        if (textMesh == null) textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.text = text;
        textMesh.color = color;
    }
}
