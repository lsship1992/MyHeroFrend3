using System.Collections;
using UnityEngine;

[System.Serializable]
public class WaveSystem
{
    [Header("Настройки волн")]
    public int enemiesPerWave = 5;
    public float timeBetweenWaves = 5f;
    public float bossTimeLimit = 40f;
    public int wavesBeforeBoss = 4;

    [Header("Сложность")]
    public float healthMultiplierPerWave = 0.1f;
    public float damageMultiplierPerWave = 0.05f;

    private GameManager gameManager;
    private PlayerController player;
    private int currentWave;
    private bool bossActive;

    public void Initialize(GameManager manager)
    {
        gameManager = manager;
        StartWaveCycle();
    }

    public void SetPlayer(PlayerController playerController)
    {
        player = playerController;
    }

    void StartWaveCycle()
    {
        gameManager.StartCoroutine(WaveRoutine());
    }

    IEnumerator WaveRoutine()
    {
        while (true)
        {
            if (currentWave % (wavesBeforeBoss + 1) == 0)
            {
                StartBossFight();
                yield return new WaitWhile(() => bossActive);
            }
            else
            {
                SpawnNormalWave();
                yield return new WaitForSeconds(timeBetweenWaves);
            }
            currentWave++;
        }
    }

    void SpawnNormalWave()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            Vector3 spawnPos = gameManager.spawnManager.enemySpawnPoints[i % gameManager.spawnManager.enemySpawnPoints.Length].position;
            gameManager.spawnManager.SpawnEnemy(spawnPos, false);
        }
    }

    void StartBossFight()
    {
        bossActive = true;
        Vector3 spawnPos = gameManager.spawnManager.enemySpawnPoints[0].position;
        gameManager.spawnManager.SpawnEnemy(spawnPos, true);
        gameManager.uiManager.ShowBossTimer(bossTimeLimit);
    }

    public void OnEnemyDied(bool isBoss)
    {
        if (isBoss) bossActive = false;
    }

    public GameStats GetScaledStats(GameStats baseStats)
    {
        return new GameStats
        {
            health = Mathf.RoundToInt(baseStats.health * (1 + healthMultiplierPerWave * currentWave)),
            attack = Mathf.RoundToInt(baseStats.attack * (1 + damageMultiplierPerWave * currentWave)),
            attackSpeed = baseStats.attackSpeed
        };
    }
}