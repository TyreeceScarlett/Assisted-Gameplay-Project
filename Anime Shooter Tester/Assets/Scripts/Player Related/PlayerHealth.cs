using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI Root")]
    public GameObject playerHealthBarRoot;

    private Image healthBackground;
    private Image healthDrain;
    private Image healthFill;
    private TMP_Text healthText;

    [Header("Colors")]
    public Color backgroundColor = new Color(0.4f, 0f, 0f);
    public Color healthColor = Color.red;
    public Color drainColor = new Color(1f, 0.5f, 0f);

    private float drainSpeed = 2f;
    private float targetFillAmount;
    private float currentDrainAmount;

    [Header("Ammo UI")]
    public WeaponAmmo weaponAmmo;
    public TMP_Text ammoText;

    [Header("Ammo Colors")]
    public Color ammoHigh = Color.yellow;
    public Color ammoLow = new Color(0.6f, 0.3f, 0f);
    public Color ammoEmpty = Color.gray;

    [Header("Warning Settings")]
    [Range(0f, 1f)] public float lowHealthThreshold = 0.25f;
    [Range(0f, 1f)] public float lowAmmoThreshold = 0.33f;
    public AudioClip lowHealthSound;
    public AudioClip lowAmmoSound;

    private bool healthWarningPlayed = false;
    private bool ammoWarningPlayed = false;
    private AudioSource audioSource;

    [Header("Low Health Flashing")]
    public bool flashWhenLow = true;
    public Color lowHealthColorBright = new Color(1f, 0f, 0f); // Bright red
    public Color lowHealthColorDark = new Color(0.5f, 0f, 0f); // Dark red
    public float flashSpeed = 4f;

    private bool isFlashing = false;

    void Awake()
    {
        if (playerHealthBarRoot == null)
        {
            Debug.LogError("PlayerHealthBarRoot is not assigned!");
            return;
        }

        // UI references
        healthBackground = playerHealthBarRoot.transform.Find("HealthBackground")?.GetComponent<Image>();
        healthDrain = playerHealthBarRoot.transform.Find("HealthDrain")?.GetComponent<Image>();
        healthFill = playerHealthBarRoot.transform.Find("HealthFill")?.GetComponent<Image>();
        healthText = playerHealthBarRoot.transform.Find("Current Health Text (TMP)")?.GetComponent<TMP_Text>();

        if (healthBackground == null) Debug.LogError("HealthBackground Image not found!");
        if (healthDrain == null) Debug.LogError("HealthDrain Image not found!");
        if (healthFill == null) Debug.LogError("HealthFill Image not found!");
        if (healthText == null) Debug.LogError("HealthText TMP component not found!");

        // Audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBackground != null) healthBackground.color = backgroundColor;
        if (healthFill != null) healthFill.color = healthColor;
        if (healthDrain != null) healthDrain.color = drainColor;

        currentDrainAmount = 1f;
        UpdateHealthUI();
        UpdateAmmoUI();
    }

    void Update()
    {
        // Drain animation
        if (healthDrain != null && healthFill != null && currentDrainAmount > targetFillAmount)
        {
            currentDrainAmount -= drainSpeed * Time.deltaTime;
            if (currentDrainAmount < targetFillAmount)
                currentDrainAmount = targetFillAmount;

            healthDrain.fillAmount = currentDrainAmount;
        }

        // Flashing health effect
        if (isFlashing && healthFill != null && flashWhenLow)
        {
            float t = Mathf.PingPong(Time.time * flashSpeed, 1f);
            healthFill.color = Color.Lerp(lowHealthColorDark, lowHealthColorBright, t);
        }

        UpdateAmmoUI();
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

        if (targetFillAmount <= lowHealthThreshold)
        {
            if (!healthWarningPlayed)
            {
                Debug.LogWarning("Low health!");
                PlayWarningSound(lowHealthSound);
                healthWarningPlayed = true;
            }

            isFlashing = true; // Start flashing
        }
        else
        {
            isFlashing = false; // Stop flashing
            healthFill.color = healthColor;
            healthWarningPlayed = false;
        }
    }

    private void UpdateAmmoUI()
    {
        if (weaponAmmo == null || ammoText == null)
            return;

        int current = weaponAmmo.currentAmmo;
        int extra = weaponAmmo.extraAmmo;

        ammoText.text = $"{current} / {extra}";

        float ammoRatio = weaponAmmo.clipSize > 0 ? (float)current / weaponAmmo.clipSize : 0;

        if (current == 0)
        {
            ammoText.color = ammoEmpty;
            if (!ammoWarningPlayed)
            {
                Debug.Log("No ammo!");
                PlayWarningSound(lowAmmoSound);
                ammoWarningPlayed = true;
            }
        }
        else if (ammoRatio <= lowAmmoThreshold)
        {
            ammoText.color = ammoLow;
            if (!ammoWarningPlayed)
            {
                Debug.Log("Low ammo!");
                PlayWarningSound(lowAmmoSound);
                ammoWarningPlayed = true;
            }
        }
        else
        {
            ammoText.color = ammoHigh;
            ammoWarningPlayed = false;
        }
    }

    private void PlayWarningSound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
