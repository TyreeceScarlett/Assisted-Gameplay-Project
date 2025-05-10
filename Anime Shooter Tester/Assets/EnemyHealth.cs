using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health;
    [HideInInspector] public bool isDead = false;

    private RagdollManager ragdollManger;
    private Renderer[] renderers;
    private Transform enemyParent; // Reference to delete parent object

    private void Start()
    {
        ragdollManger = GetComponent<RagdollManager>();
        renderers = GetComponentsInChildren<Renderer>();

        // Assume the top-level parent is the full "Enemy" object you want to delete
        enemyParent = transform.root;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;

        if (health <= 0)
        {
            isDead = true;
            EnemyDeath();
        }
        else
        {
            Debug.Log("Hit");
        }
    }

    void EnemyDeath()
    {
        ragdollManger.TriggerRagdoll();
        Debug.Log("Enemy has Died");

        // Start fading + sinking coroutine after 20 seconds
        StartCoroutine(FadeSinkAndDestroy(20f, 2f)); // wait 20s, fade+sink over 2s
    }

    IEnumerator FadeSinkAndDestroy(float waitTime, float fadeDuration)
    {
        yield return new WaitForSeconds(waitTime);

        float timer = 0f;
        Vector3 startPos = enemyParent.position;
        Vector3 targetPos = startPos + Vector3.down * 0.5f; // Sink 0.5 units down

        while (timer < fadeDuration)
        {
            float t = timer / fadeDuration;

            // Fade
            float alpha = Mathf.Lerp(1f, 0f, t);
            SetAlpha(alpha);

            // Sink
            enemyParent.position = Vector3.Lerp(startPos, targetPos, t);

            timer += Time.deltaTime;
            yield return null;
        }

        // Ensure fully invisible + fully sunk
        SetAlpha(0f);
        enemyParent.position = targetPos;

        // Delete the parent object
        Destroy(enemyParent.gameObject);
    }

    void SetAlpha(float alpha)
    {
        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.materials)
            {
                // For MToon (VRM) shader:
                if (mat.HasProperty("_Color"))
                {
                    Color color = mat.color;
                    color.a = alpha;
                    mat.color = color;

                    // Set MToon or Standard shader to Transparent mode
                    SetMaterialToTransparent(mat);
                }
            }
        }
    }

    void SetMaterialToTransparent(Material mat)
    {
        if (mat.shader.name.Contains("MToon"))
        {
            // VRM MToon transparency setup
            mat.SetFloat("_BlendMode", 2); // 0: Opaque, 1: Cutout, 2: Transparent
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }
        else
        {
            // Fallback for Standard shader
            mat.SetFloat("_Mode", 2); // Fade
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }
    }
}
