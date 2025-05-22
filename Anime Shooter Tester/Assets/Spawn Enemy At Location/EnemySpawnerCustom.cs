using UnityEngine;
using TMPro;

public class EnemySpawnerCustom : MonoBehaviour
{
    [Header("Enemy Spawning")]
    public GameObject enemyPrefab;
    public TMP_InputField inputX;
    public TMP_InputField inputZ;

    private const float minCoord = 1f;
    private const float maxCoord = 400f;

    [Header("UI Panels")]
    public GameObject EnemySpawnPanelShow;

    private void Update()
    {
        // F6 toggles the EnemySpawnPanelShow manually
        if (Input.GetKeyDown(KeyCode.F6))
        {
            if (EnemySpawnPanelShow != null)
                EnemySpawnPanelShow.SetActive(!EnemySpawnPanelShow.activeSelf);
        }
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
