using UnityEngine;
using UnityEngine.UI;
using TMPro;  // Add this for TextMeshPro support

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI Root")]
    public GameObject playerHealthBarRoot;  // Assign your PlayerHealthBar GameObject here

    private Image healthBackground;
    private Image healthDrain;
    private Image healthFill;
    private TMP_Text healthText;  // Use TMP_Text here

    [Header("Colors")]
    public Color backgroundColor = new Color(0.4f, 0f, 0f); // Dark red
    public Color healthColor = Color.red;
    public Color drainColor = new Color(1f, 0.5f, 0f); // Orange

    private float drainSpeed = 2f;
    private float targetFillAmount;
    private float currentDrainAmount;

    void Awake()
    {
        if (playerHealthBarRoot == null)
        {
            Debug.LogError("PlayerHealthBarRoot is not assigned!");
            return;
        }

        // Auto-find UI components by name inside the root
        healthBackground = playerHealthBarRoot.transform.Find("HealthBackground")?.GetComponent<Image>();
        healthDrain = playerHealthBarRoot.transform.Find("HealthDrain")?.GetComponent<Image>();
        healthFill = playerHealthBarRoot.transform.Find("HealthFill")?.GetComponent<Image>();
        healthText = playerHealthBarRoot.transform.Find("Current Health Text (TMP)")?.GetComponent<TMP_Text>();

        // Check for missing references and warn
        if (healthBackground == null) Debug.LogError("HealthBackground Image not found in PlayerHealthBar!");
        if (healthDrain == null) Debug.LogError("HealthDrain Image not found in PlayerHealthBar!");
        if (healthFill == null) Debug.LogError("HealthFill Image not found in PlayerHealthBar!");
        if (healthText == null) Debug.LogError("Current Health Text (TMP) component not found in PlayerHealthBar!");
    }

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBackground != null) healthBackground.color = backgroundColor;
        if (healthFill != null) healthFill.color = healthColor;
        if (healthDrain != null) healthDrain.color = drainColor;

        currentDrainAmount = 1f;

        UpdateHealthUI();
    }

    void Update()
    {
        if (healthDrain == null || healthFill == null)
            return;

        if (currentDrainAmount > targetFillAmount)
        {
            currentDrainAmount -= drainSpeed * Time.deltaTime;
            if (currentDrainAmount < targetFillAmount)
                currentDrainAmount = targetFillAmount;

            healthDrain.fillAmount = currentDrainAmount;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthUI();
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthFill == null || healthText == null)
            return;

        targetFillAmount = (float)currentHealth / maxHealth;
        healthFill.fillAmount = targetFillAmount;

        healthText.text = $"{currentHealth} / {maxHealth}";
    }
}
