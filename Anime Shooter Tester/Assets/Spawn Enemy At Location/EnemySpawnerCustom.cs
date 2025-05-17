using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnemySpawnerCustom : MonoBehaviour
{
    [Header("Enemy Spawning")]
    public GameObject enemyPrefab;
    public TMP_InputField inputX;
    public TMP_InputField inputZ;

    private const float minCoord = 1f;
    private const float maxCoord = 400f;

    [Header("UI Panels")]
    public GameObject enemySpawnScrollView;

    [HideInInspector]
    public GameObject EnemySpawnPanelShow;

    [Header("UI Controls")]
    public Toggle maximizeToggle;

    private void Start()
    {
        // Make sure the scroll view visibility matches the toggle at start
        if (enemySpawnScrollView != null && maximizeToggle != null)
            enemySpawnScrollView.SetActive(maximizeToggle.isOn);

        // Set up listener for toggle changes
        if (maximizeToggle != null)
            maximizeToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void Update()
    {
        // F6 toggles the panel manually (optional)
        if (Input.GetKeyDown(KeyCode.F6))
        {
            if (enemySpawnScrollView != null)
            {
                bool isActive = enemySpawnScrollView.activeSelf;
                enemySpawnScrollView.SetActive(!isActive);
                if (maximizeToggle != null)
                    maximizeToggle.isOn = !isActive; // sync toggle state
            }
        }
    }

    private void OnToggleChanged(bool isOn)
    {
        if (enemySpawnScrollView != null)
            enemySpawnScrollView.SetActive(isOn);
    }

    public void SpawnEnemyAtInputPosition()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("Enemy prefab not assigned!");
            return;
        }

        if (inputX == null || inputZ == null)
        {
            Debug.LogWarning("Input fields not assigned!");
            return;
        }

        if (!float.TryParse(inputX.text, out float x) || !float.TryParse(inputZ.text, out float z))
        {
            Debug.LogWarning("Please enter valid numbers for X and Z.");
            return;
        }

        x = Mathf.Clamp(x, minCoord, maxCoord);
        z = Mathf.Clamp(z, minCoord, maxCoord);

        Vector3 spawnPos = new Vector3(x, 0f, z);
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        Debug.Log($"Spawned enemy at ({x}, 0, {z})");
    }
}
