using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public float moveUpSpeed = 1f;
    public float fadeOutSpeed = 2f;
    public float lifetime = 1.2f;

    private TextMeshProUGUI text;
    private Color originalColor;
    private float timer;

    void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            originalColor = text.color;
        }
    }

    public void Setup(float damageAmount, bool isHeadshot)
    {
        if (text == null) return;

        text.text = damageAmount.ToString("F0");
        text.color = isHeadshot ? Color.red : originalColor;

        if (isHeadshot)
            text.fontSize += 6f;

        timer = lifetime;
    }

    void Update()
    {
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0); // So it's not mirrored

        transform.position += Vector3.up * moveUpSpeed * Time.deltaTime;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Color c = text.color;
            c.a -= fadeOutSpeed * Time.deltaTime;
            text.color = c;

            if (c.a <= 0f)
                Destroy(gameObject);
        }
    }
}
