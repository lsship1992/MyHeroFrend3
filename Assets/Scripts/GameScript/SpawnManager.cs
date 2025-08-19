using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject bossPrefab;
    public GameObject fireballPrefab;

    [Header("Spawn Points")]
    public Transform playerSpawnPoint;
    public Transform[] enemySpawnPoints;

    private GameManager gameManager;
    public WaveSystem waveSystem;
    // И создайте класс WaveSystem:
    [System.Serializable]
    public class WaveSystem
    {
        public int currentEnemyCount;
        // Другие нужные параметры волн
    }

       
    public void SpawnEnemy(Vector3 position, bool isBoss)
    {
        GameObject prefab = isBoss ? bossPrefab : enemyPrefab;
        var enemy = Object.Instantiate(prefab, position, Quaternion.identity);

        // Автонастройка параметров
        GameStats stats = new GameStats(
            health: isBoss ? 200 : 100,
            attack: isBoss ? 20 : 10,
            defense: 2,
            moveSpeed: isBoss ? 1.5f : 2f,
            attackSpeed: isBoss ? 1.5f : 1f
        );

        enemy.GetComponent<EnemyController>().Initialize(stats, 10, 5, isBoss);
        DebugLogger.Log($"Enemy spawned - IsBoss: {isBoss}");
    }

    public GameObject SpawnFireball(Vector3 position, bool isPlayerProjectile, int damage)
    {
        var fireball = Object.Instantiate(fireballPrefab, position, Quaternion.identity);
        fireball.GetComponent<FireballController>().Initialize(isPlayerProjectile, damage);
        return fireball;
    }
}