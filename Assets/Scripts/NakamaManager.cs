using Nakama;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class NakamaManager : MonoBehaviour
{
    public static NakamaManager Instance { get; private set; }

    public IClient Client { get; private set; }
    public ISession Session { get; private set; }
    public ISocket Socket { get; private set; }
    public bool IsInitialized { get; private set; }

    [Header("Server Settings")]
    public string scheme = "http";
    public string host = "212.8.229.4";
    public int port = 7350;
    public string serverKey = "f7069cefe7e14000d6ab27cf331d42a2f0982bc9b88788c70be6106702650f21";

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
            Client = new Client(scheme, host, port, serverKey)
            {
                Timeout = 10
            };

            // Попытка восстановить сессию
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

            // Новая аутентификация
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

    public async Task SaveCharacterData(string collection, string key, string value)
    {
        await EnsureSessionIsValid();
        var writeObject = new WriteStorageObject
        {
            Collection = collection,
            Key = key,
            Value = value
        };
        await Client.WriteStorageObjectsAsync(Session, new[] { writeObject });
    }

    // Методы для кастомизации персонажа

    public async Task<string> LoadCharacterData(string collection, string key)
    {
        await EnsureSessionIsValid();

        var storageObjectId = new StorageObjectId
        {
            Collection = collection,
            Key = key,
            UserId = Session.UserId
        };

        var result = await Client.ReadStorageObjectsAsync(Session, new[] { storageObjectId });

        if (result.Objects != null && result.Objects.Any())
        {
            return result.Objects.First().Value;
        }
        return null;
    }
}