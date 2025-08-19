using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject playerPrefab;
    private GameObject currentPlayer;
    public int currentWave = 1;
    public int enemiesInWave = 5;
    private int enemiesDefeated = 0;

    #region Initialization
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async void Start()
    {
        // ���� ������������� Nakama
        while (!NakamaManager.Instance.IsInitialized)
        {
            await Task.Yield();
        }

        // �������������� ���������
        await NakamaManager.Instance.InitializeLeaderboard();

        // ��������� ����������� ��������
        await LoadGameProgress();
        await StartGame(); // �������� �� ����������� �����
    }

    private async Task LoadGameProgress()
    {
        try
        {
            var progress = await NakamaManager.Instance.LoadCharacterData(
                "game_progress",
                "current_wave"
            );

            if (!string.IsNullOrEmpty(progress))
            {
                currentWave = int.Parse(progress);
                Debug.Log($"Loaded progress: Wave {currentWave}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Load progress failed: {e.Message}");
        }
    }
    #endregion

    #region Game Flow
    private async Task StartGame()
    {
        await SpawnPlayer(); // ������� ���������� ������ ������
        StartCoroutine(SpawnWave());
    }

    public void EnemyDefeated()
    {
        enemiesDefeated++;

        if (enemiesDefeated >= enemiesInWave)
        {
            CompleteWave();
        }
    }

    public async void CompleteWave()
    {
        currentWave++;
        enemiesDefeated = 0;
        enemiesInWave += 3;

        try
        {
            // ��������� ��������
            await NakamaManager.Instance.SaveCharacterData(
                "game_progress",
                "current_wave",
                currentWave.ToString()
            );

            // ���������� � ���������
            await NakamaManager.Instance.SubmitWaveResult(currentWave);

            Debug.Log($"Wave {currentWave} completed!");
        }
        catch (Exception e)
        {
            Debug.LogError($"Save progress failed: {e.Message}");
        }

        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        Debug.Log($"Starting wave {currentWave}");
        for (int i = 0; i < enemiesInWave; i++)
        {
            if (EnemySpawner.Instance != null)
            {
                EnemySpawner.Instance.SpawnEnemy();
            }
            yield return new WaitForSeconds(1f);
        }
    }

    /// <summary>
    /// ������� ������ � ������ ������������
    /// </summary>
    private async Task SpawnPlayer()
    {
        if (currentPlayer != null) return;

        // ��������� ������ ���������
        var characterData = await NakamaCharacterSystem.Instance.LoadCharacter();

        if (characterData != null)
        {
            currentPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

            // ��������� ������������
            var editor = currentPlayer.GetComponent<CharacterEditorController>();
            if (editor != null)
            {
                editor.LoadFromJson(characterData.appearanceJson);
            }
            else
            {
                Debug.LogError("CharacterEditorController not found!");
            }
        }
        else
        {
            currentPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        }

        DontDestroyOnLoad(currentPlayer);
    }
    #endregion
}