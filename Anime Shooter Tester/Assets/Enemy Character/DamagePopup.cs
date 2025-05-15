using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    [Header("Popup Settings")]
    public float moveUpSpeed = 30f;
    public float fadeOutSpeed = 2f;
    public float lifetime = 1f;

    private TextMeshProUGUI text;
    private RectTransform rectTransform;
    private Color startColor;
    private float timer;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
        startColor = text.color;
    }

    public void Setup(int damageAmount, bool isHeadshot)
    {
        text.text = damageAmount.ToString();

        if (isHeadshot)
        {
            text.color = Color.red;
            text.fontSize += 6;
        }
        else
        {
            text.color = startColor;
        }

        timer = lifetime;
    }

    void Update()
    {
        // Move up
        rectTransform.anchoredPosition += Vector2.up * moveUpSpeed * Time.deltaTime;

        // Fade out
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Color color = text.color;
            color.a -= fadeOutSpeed * Time.deltaTime;
            text.color = color;

            if (color.a <= 0f)
                Destroy(gameObject);
        }
    }
}
