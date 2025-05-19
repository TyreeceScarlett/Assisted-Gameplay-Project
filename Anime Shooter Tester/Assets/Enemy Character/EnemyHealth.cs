using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro; // ✅ Add this for TMP_Text support

[RequireComponent(typeof(AudioSource))]
public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    [HideInInspector] public float health;
    [HideInInspector] public bool isDead = false;

    [Header("UI")]
    public Slider healthBarSlider;
    public TMP_Text healthText; // ✅ Use TMP_Text instead of Text

    [Header("Effects")]
    public AudioClip deathSound;

    private AudioSource audioSource;
    private Renderer[] renderers;
    private RagdollManager ragdollManager;
    private Transform enemyParent;
    private Color originalColor;

    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private AiAgent aiAgent;

    private void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        ragdollManager = GetComponent<RagdollManager>();
        aiAgent = GetComponent<AiAgent>();
        renderers = GetComponentsInChildren<Renderer>();

        enemyParent = transform.root;

        if (renderers.Length > 0)
        {
            originalColor = renderers[0].material.color;
        }

        health = maxHealth;

        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = maxHealth;
            healthBarSlider.value = health;
        }

        if (healthText != null)
        {
            healthText.text = $"{health} / {maxHealth}";
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;
        health = Mathf.Clamp(health, 0f, maxHealth);

        if (healthBarSlider != null)
        {
            healthBarSlider.value = health;
        }

        if (healthText != null)
        {
            healthText.text = $"{health} / {maxHealth}";
        }

        StartCoroutine(FlashWhite());

        if (health <= 0)
        {
            isDead = true;
            EnemyDeath();
        }
        else
        {
            Debug.Log("Enemy hit, health remaining: " + health);
        }
    }

    private void EnemyDeath()
    {
        if (animator != null) animator.enabled = false;
        if (navMeshAgent != null) navMeshAgent.enabled = false;
        if (aiAgent != null) aiAgent.enabled = false;

        if (healthBarSlider != null)
        {
            healthBarSlider.gameObject.SetActive(false);
        }

        if (healthText != null)
        {
            healthText.gameObject.SetActive(false);
        }

        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        if (ragdollManager != null)
        {
            ragdollManager.TriggerRagdoll();
        }

        Debug.Log("Enemy has died.");

        StartCoroutine(FadeSinkAndDestroy(1f, 2f));
    }

    IEnumerator FlashWhite()
    {
        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.materials)
            {
                mat.color = Color.white;
            }
        }

        yield return new WaitForSeconds(0.1f);

        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.materials)
            {
                mat.color = originalColor;
            }
        }
    }

    IEnumerator FadeSinkAndDestroy(float waitTime, float fadeDuration)
    {
        yield return new WaitForSeconds(waitTime);

        float timer = 0f;
        Vector3 startPos = enemyParent.position;
        Vector3 endPos = startPos + Vector3.down * 0.5f;

        while (timer < fadeDuration)
        {
            float t = timer / fadeDuration;

            SetAlpha(Mathf.Lerp(1f, 0f, t));
            enemyParent.position = Vector3.Lerp(startPos, endPos, t);

            timer += Time.deltaTime;
            yield return null;
        }

        SetAlpha(0f);
        enemyParent.position = endPos;

        Destroy(enemyParent.gameObject);
    }

    void SetAlpha(float alpha)
    {
        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.materials)
            {
                if (mat.HasProperty("_Color"))
                {
                    Color color = mat.color;
                    color.a = alpha;
                    mat.color = color;

                    SetMaterialToTransparent(mat);
                }
            }
        }
    }

    void SetMaterialToTransparent(Material mat)
    {
        if (mat.shader.name.Contains("MToon"))
        {
            mat.SetFloat("_BlendMode", 2);
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
            mat.SetFloat("_Mode", 2);
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
