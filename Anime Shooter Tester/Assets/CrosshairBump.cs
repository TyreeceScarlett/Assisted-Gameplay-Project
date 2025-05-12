using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairBump : MonoBehaviour
{
    public float bumpScale = 1.5f;      // How much to scale up during bump
    public float bumpDuration = 0.2f;   // How fast the bump happens

    private RectTransform rectTransform;
    private Vector3 originalScale;
    private bool isBumping = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
    }

    public void TriggerBump()
    {
        if (!isBumping)
            StartCoroutine(BumpRoutine());
    }

    IEnumerator BumpRoutine()
    {
        isBumping = true;
        float timer = 0f;

        while (timer < bumpDuration)
        {
            float t = timer / bumpDuration;
            float scale = Mathf.Lerp(bumpScale, 1f, t);
            rectTransform.localScale = originalScale * scale;

            timer += Time.deltaTime;
            yield return null;
        }

        rectTransform.localScale = originalScale;
        isBumping = false;
    }
}
