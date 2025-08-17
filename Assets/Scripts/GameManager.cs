using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Подсистемы")]
    public WaveSystem waveSystem;
    public SpawnManager spawnManager;
    public UIManager uiManager;
    public GameData gameData;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAllSystems();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeAllSystems()
    {
        waveSystem.Initialize(this);
        spawnManager.Initialize(this);
        uiManager.Initialize(this);
    }
}