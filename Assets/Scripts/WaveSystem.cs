using UnityEngine;
using System.Collections;

public class WaveSystem : MonoBehaviour
{
    // Singleton pattern
    public static WaveSystem Instance { get; private set; }

    [Header("Wave Settings")]
    public int enemiesPerWave = 5;
    public float timeBetweenWaves = 5f;
    public float bossTimeLimit = 40f;
    public int wavesBeforeBoss = 4;

    [Header("Scaling")]
    public float healthMultiplierPerWave = 0.1f;
    public float damageMultiplierPerWave = 0.05f;

    public int CurrentWave { get; private set; } = 1; // Текущая волна

    // Ссылки на другие менеджеры
    private GameManager gameManager;
    private SpawnManager spawnManager;
    private UIManager uiManager;
    private PlayerController player;

    private bool bossActive;
    private Coroutine waveCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DebugLogger.Log("WaveSystem initialized as singleton");
        }
        else
        {
            DebugLogger.LogWarning("Duplicate WaveSystem detected - destroying", this);
            Destroy(gameObject);
        }
    }

    public void Initialize(GameManager manager, SpawnManager spawnMgr, UIManager uiMgr)
    {
        gameManager = manager;
        spawnManager = spawnMgr;
        uiManager = uiMgr;

        DebugLogger.Log("WaveSystem initialized with managers");
        StartWaveCycle();
    }

    public void SetPlayer(PlayerController playerController)
    {
        player = playerController;
        DebugLogger.Log("Player reference set in WaveSystem");
    }

    public void StartWaveCycle()
    {
        if (waveCoroutine != null)
        {
            StopCoroutine(waveCoroutine);
        }
        waveCoroutine = StartCoroutine(WaveRoutine());
        DebugLogger.Log("Wave cycle started");
    }

    private IEnumerator WaveRoutine()
    {
        while (true)
        {
            yield return StartCoroutine(HandleWave());
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    private IEnumerator HandleWave()
    {
        DebugLogger.Log($"Starting wave {CurrentWave}");

        if (CurrentWave % (wavesBeforeBoss + 1) == 0)
        {
            StartBossFight();
            yield return new WaitWhile(() => bossActive);
        }
        else
        {
            SpawnNormalWave();
            yield return new WaitWhile(() => AreEnemiesAlive());
        }

        CurrentWave++;
        uiManager?.UpdateWaveInfo(1, CurrentWave);
    }
    private bool AreEnemiesAlive()
    {
        // Ищем только активных врагов
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        foreach (var enemy in enemies)
        {
            if (enemy.gameObject.activeInHierarchy)
                return true;
        }
        return false;
    }
    private void SpawnNormalWave()
    {
        DebugLogger.Log($"Spawning normal wave with {enemiesPerWave} enemies");
        for (int i = 0; i < enemiesPerWave; i++)
        {
            Vector3 spawnPos = spawnManager.enemySpawnPoints[i % spawnManager.enemySpawnPoints.Length].position;
            spawnManager.SpawnEnemy(spawnPos, false);
        }
    }

    private void StartBossFight()
    {
        DebugLogger.Log("Starting boss fight");
        bossActive = true;
        Vector3 spawnPos = spawnManager.enemySpawnPoints[0].position;
        spawnManager.SpawnEnemy(spawnPos, true);
        uiManager?.ShowBossTimer(bossTimeLimit);
    }

    // Методы для обработки событий
    public void OnPlayerDeath()
    {
        DebugLogger.Log("Player died - handling in WaveSystem");
        if (bossActive)
        {
            CurrentWave--;
            bossActive = false;
            uiManager?.HideBossButton();
        }
    }

    public void OnBossDefeated()
    {
        DebugLogger.Log("Boss defeated!");
        bossActive = false;
        uiManager?.HideBossButton();
    }

    public void SetWave(int wave)
    {
        CurrentWave = Mathf.Max(1, wave);
        DebugLogger.Log($"Wave set to {CurrentWave}");
    }

    public GameStats GetScaledStats(GameStats baseStats)
    {
        return new GameStats(
            health: Mathf.RoundToInt(baseStats.health * (1 + healthMultiplierPerWave * CurrentWave)),
            attack: Mathf.RoundToInt(baseStats.attack * (1 + damageMultiplierPerWave * CurrentWave)),
            defense: baseStats.defense,
            moveSpeed: baseStats.moveSpeed,
            attackSpeed: baseStats.attackSpeed
        );
    }
}