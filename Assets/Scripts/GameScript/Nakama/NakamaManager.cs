using Nakama;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class NakamaManager : MonoBehaviour
{
    public static NakamaManager Instance { get; private set; }

    // Основные компоненты Nakama
    public IClient Client { get; private set; }
    public ISession Session { get; private set; }
    public ISocket Socket { get; private set; }
    public bool IsInitialized { get; private set; }

    [Header("Server Settings")]
    public string scheme = "http";
    public string host = "212.8.229.4";
    public int port = 7350;
    public string serverKey = "f7069cefe7e14000d6ab27cf331d42a2f0982bc9b88788c70be6106702650f21";

    // Константы для прав доступа к хранилищу
    private const int StorageReadPermissionOwner = 1; // Только владелец
    private const int StorageWritePermissionOwner = 1; // Только владелец

    #region Инициализация
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private async void Start()
    {
        await InitializeNakama();
    }

    public async Task InitializeNakama()
    {
        if (IsInitialized) return;

        try
        {
            // Создаем клиент Nakama
            Client = new Client(scheme, host, port, serverKey)
            {
                Timeout = 10
            };

            // Пробуем восстановить сессию из PlayerPrefs
            var authToken = PlayerPrefs.GetString("nakama_token");
            if (!string.IsNullOrEmpty(authToken))
            {
                Session = Nakama.Session.Restore(authToken);
                if (!Session.HasExpired(DateTime.UtcNow.AddDays(1)))
                {
                    var account = await Client.GetAccountAsync(Session);
                    Debug.Log($"Session restored for: {account.User.Id}");
                    IsInitialized = true;
                    return;
                }
                PlayerPrefs.DeleteKey("nakama_token");
            }

            // Новая аутентификация по устройству
            await AuthenticateDevice();
            Socket = Client.NewSocket();
            await Socket.ConnectAsync(Session);
            IsInitialized = true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Nakama initialization failed: {e}");
        }
    }
        #endregion

    #region Аутентификация
    private async Task AuthenticateDevice()
    {
        try
        {
            string deviceId = SystemInfo.deviceUniqueIdentifier;
            if (string.IsNullOrEmpty(deviceId))
            {
                deviceId = Guid.NewGuid().ToString();
                Debug.Log($"Generated new Device ID: {deviceId}");
            }

            Session = await Client.AuthenticateDeviceAsync(deviceId);
            PlayerPrefs.SetString("nakama_token", Session.AuthToken);
            Debug.Log($"Authenticated successfully! User ID: {Session.UserId}");
        }
        catch (ApiResponseException e)
        {
            Debug.LogError($"Authentication failed: {e.Message}");
            throw;
        }
    }

    public async Task EnsureSessionIsValid()
    {
        if (Session == null || Session.IsExpired)
        {
            await AuthenticateDevice();
        }
    }
    #endregion

    #region Работа с хранилищем
    /// <summary>
    /// Сохраняет данные в хранилище Nakama
    /// </summary>
    public async Task SaveCharacterData(string collection, string key, string value)
    {
        await EnsureSessionIsValid();

        var writeObject = new WriteStorageObject
        {
            Collection = collection,
            Key = key,
            Value = value,
            PermissionRead = StorageReadPermissionOwner,
            PermissionWrite = StorageWritePermissionOwner
        };

        await Client.WriteStorageObjectsAsync(Session, new[] { writeObject });
    }

    /// <summary>
    /// Загружает данные из хранилища Nakama
    /// </summary>
    public async Task<string> LoadCharacterData(string collection, string key)
    {
        await EnsureSessionIsValid();

        var objectId = new StorageObjectId
        {
            Collection = collection,
            Key = key,
            UserId = Session.UserId
        };

        var result = await Client.ReadStorageObjectsAsync(Session, new[] { objectId });
        return result.Objects?.FirstOrDefault()?.Value;
    }
    #endregion

    #region Лидерборды
    /// <summary>
    /// Инициализирует лидерборд (создает при первом запуске)
    /// </summary>
    public async Task InitializeLeaderboard()
    {
        try
        {
            // Добавляем await для реальной асинхронной работы
            var record = await Client.WriteLeaderboardRecordAsync(
                session: Session,
                leaderboardId: "wave_leaderboard",
                score: 0
            );
            Debug.Log("Leaderboard initialized");
        }
        catch (ApiResponseException e) when (e.StatusCode == 404)
        {
            Debug.LogWarning("Leaderboard doesn't exist");
            await CreateLeaderboardManually();
        }
        catch (Exception e)
        {
            Debug.LogError($"Leaderboard init failed: {e.Message}");
        }
    }

    private async Task CreateLeaderboardManually()
    {
        // Реализация создания лидерборда через RPC или другие методы
        Debug.Log("Creating leaderboard...");
        await Task.Delay(100); // Заглушка для примера
    }
    /// <summary>
    /// Отправляет результат волны в лидерборд
    /// </summary>
    public async Task SubmitWaveResult(int wave)
    {
        await EnsureSessionIsValid();

        try
        {
            await Client.WriteLeaderboardRecordAsync(
                session: Session,
                leaderboardId: "wave_leaderboard",
                score: wave
            );
            Debug.Log($"Wave {wave} submitted to leaderboard!");
        }
        catch (ApiResponseException e)
        {
            Debug.LogError($"Leaderboard submit failed: {e.Message}");
        }
    }

    /// <summary>
    /// Получает записи лидерборда
    /// </summary>
    public async Task<IApiLeaderboardRecordList> GetLeaderboard(int limit = 10)
    {
        await EnsureSessionIsValid();
        return await Client.ListLeaderboardRecordsAsync(
            session: Session,
            leaderboardId: "wave_leaderboard",
            limit: limit
        );
    }
    #endregion
    #region Character Customization
    /// <summary>
    /// Загружает данные кастомизации персонажа
    /// </summary>
    public async Task<string> LoadCharacterCustomization()
    {
        await EnsureSessionIsValid();

        var objectId = new StorageObjectId
        {
            Collection = "characters",
            Key = "appearance",
            UserId = Session.UserId
        };

        try
        {
            var result = await Client.ReadStorageObjectsAsync(Session, new[] { objectId });
            return result.Objects.FirstOrDefault()?.Value;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load character customization: {e.Message}");
            return null;
        }
    }

    /// <summary>
    /// Сохраняет данные кастомизации персонажа
    /// </summary>
    public async Task SaveCharacterCustomization(string jsonData)
    {
        await EnsureSessionIsValid();

        var writeObject = new WriteStorageObject
        {
            Collection = "characters",
            Key = "appearance",
            Value = jsonData,
            PermissionRead = StorageReadPermissionOwner,
            PermissionWrite = StorageWritePermissionOwner
        };

        await Client.WriteStorageObjectsAsync(Session, new[] { writeObject });
    }
    #endregion
}