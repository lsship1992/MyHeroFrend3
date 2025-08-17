using UnityEngine;

[System.Serializable]
public class SpawnManager
{
    [Header("Префабы")]
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject bossPrefab;
    public GameObject fireballPrefab;

    [Header("Точки спавна")]
    public Transform playerSpawnPoint;
    public Transform[] enemySpawnPoints;

    private GameManager gameManager;

    public void Initialize(GameManager manager)
    {
        gameManager = manager;
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        GameObject player = Object.Instantiate(
            playerPrefab,
            playerSpawnPoint.position,
            Quaternion.identity
        );
        gameManager.waveSystem.SetPlayer(player.GetComponent<PlayerController>());
    }

    public void SpawnEnemy(Vector3 position, bool isBoss)
    {
        GameObject prefab = isBoss ? bossPrefab : enemyPrefab;
        var enemy = Object.Instantiate(prefab, position, Quaternion.identity);
        enemy.GetComponent<EnemyController>().Initialize(isBoss);
    }

    public GameObject SpawnFireball(Vector3 position, bool isPlayerProjectile, int damage)
    {
        var fireball = Object.Instantiate(fireballPrefab, position, Quaternion.identity);
        fireball.GetComponent<FireballController>().Initialize(isPlayerProjectile, damage);
        return fireball;
    }
}