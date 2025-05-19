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
    private CanvasGroup canvasGroup;

    void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (text != null)
            originalColor = text.color;

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
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
        // Billboard toward camera
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0); // prevent mirrored text

        // Move up
        transform.position += Vector3.up * moveUpSpeed * Time.deltaTime;

        // Fade out over time
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            canvasGroup.alpha -= fadeOutSpeed * Time.deltaTime;
            if (canvasGroup.alpha <= 0f)
                Destroy(gameObject);
        }
    }
}
