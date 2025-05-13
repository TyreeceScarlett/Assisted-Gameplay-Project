using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // For UI controls
using UnityEngine.AI; // Added for NavMeshAgent

public class EnemyHealth : MonoBehaviour
{
    public float health;
    public float maxHealth = 100f; // Max health for slider normalization
    [HideInInspector] public bool isDead = false;

    private RagdollManager ragdollManger;
    private Renderer[] renderers;
    private Transform enemyParent; // Reference to delete parent object
    private Color originalColor; // To store the original color of the enemy

    [Header("UI")]
    public Slider healthBarSlider; // Reference to the Slider component in Canvas

    [Header("Effects")]
    public AudioClip deathSound; // Optional sound effect
    private AudioSource audioSource;

    private void Start()
    {
        ragdollManger = GetComponent<RagdollManager>();
        renderers = GetComponentsInChildren<Renderer>();
        audioSource = GetComponent<AudioSource>();

        // Store the original color of the enemy
        if (renderers.Length > 0)
        {
            originalColor = renderers[0].material.color;
        }

        // Assume the top-level parent is the full "Enemy" object you want to delete
        enemyParent = transform.root;

        // Initialize health
        if (health <= 0) health = maxHealth;

        // Initialize health bar if assigned
        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = maxHealth;
            healthBarSlider.value = health;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;

        // Clamp health to zero minimum
        health = Mathf.Clamp(health, 0f, maxHealth);

        // Update health bar if assigned
        if (healthBarSlider != null)
        {
            healthBarSlider.value = health;
        }

        // Flash white when damage is taken
        StartCoroutine(FlashWhite());

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

    // Coroutine to flash white for a short duration
    IEnumerator FlashWhite()
    {
        // Set all renderers to white
        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.materials)
            {
                mat.color = Color.white;
            }
        }

        // Wait for a brief moment (flash duration)
        yield return new WaitForSeconds(0.1f);

        // Restore original color
        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.materials)
            {
                mat.color = originalColor;
            }
        }
    }

    void EnemyDeath()
    {
        // Disable Animator to allow ragdoll physics to take over
        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.enabled = false;

        // Disable NavMeshAgent to stop AI control
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        // Optional: play sound
        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        // Activate ragdoll
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
