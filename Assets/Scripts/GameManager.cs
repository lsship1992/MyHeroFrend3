using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("References")]
    public SpawnManager spawnManager;
    public WaveSystem waveSystem;
    public UIManager uiManager;

    [Header("Player")]
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;

    [Header("Enemies")]
    public List<GameObject> enemyPrefabs;
    public List<Transform> enemySpawnPoints;

    private GameObject currentPlayer;
    private bool playerSpawned = false; // Добавляем флаг
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DebugLogger.Log("GameManager initialized as singleton");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeManagers();
        SpawnPlayer();
        waveSystem.StartWaveCycle();
    }

    private void InitializeManagers()
    {
        if (spawnManager == null) spawnManager = GetComponent<SpawnManager>();
        if (waveSystem == null) waveSystem = GetComponent<WaveSystem>();
        if (uiManager == null) uiManager = FindObjectOfType<UIManager>();

        spawnManager.Initialize(this);
        waveSystem.Initialize(this, spawnManager, uiManager);

        DebugLogger.Log("All managers initialized");
    }

    public void SpawnPlayer()
    {
        if (playerSpawned) return;

        currentPlayer = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
        waveSystem.SetPlayer(currentPlayer.GetComponent<PlayerController>());
        playerSpawned = true;

        DebugLogger.Log("Player spawned");
    }
}