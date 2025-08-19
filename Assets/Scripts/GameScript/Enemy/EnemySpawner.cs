using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;
    public GameObject enemyPrefab;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SpawnEnemy()
    {
        if (GameManager.Instance == null || enemyPrefab == null) return;

        Instantiate(enemyPrefab, GetSpawnPosition(), Quaternion.identity);
    }

    Vector3 GetSpawnPosition()
    {
        // Ваша логика определения позиции спавна
        return new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
    }
}