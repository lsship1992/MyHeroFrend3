using Nakama;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ServerManager : MonoBehaviour
{
    [Header("Connection Settings")]
    [SerializeField] private string scheme = "http";
    [SerializeField] private string host = "212.8.229.4";
    [SerializeField] private int port = 7350;
    [SerializeField] private string serverKey = "defaultkey";

    [Header("UI References")]
    [SerializeField] private TMP_Dropdown serverDropdown;
    [SerializeField] private Button connectButton;
    [SerializeField] private Button refreshButton;
    [SerializeField] private TMP_Text serverStatusText;
    [SerializeField] private GameObject serverInfoPanel;
    [SerializeField] private TMP_Text serverNameText;
    [SerializeField] private TMP_Text serverStatusLabel;
    [SerializeField] private TMP_Text playersCountText;

    private IClient client;
    private ISession session;
    private List<ServerInfo> servers = new List<ServerInfo>();

    [System.Serializable]
    public class ServerInfo
    {
        public string id;
        public string name;
        public string status;
        public int currentPlayers;
        public int maxPlayers;
        public string ip;
        public int port;
    }

    private async void Start()
    {
        string savedServerId = PlayerPrefs.GetString("SelectedServerId", "");

        if (!string.IsNullOrEmpty(savedServerId))
        {
            await ConnectToSavedServer(savedServerId);
            return;
        }

        try
        {
            await InitializeClientAndSession();
            await LoadServers();
            SetupUI();
        }
        catch (Exception e)
        {
            Debug.LogError($"Initialization failed: {e.Message}");
            serverStatusText.text = "Ошибка подключения. Повтор...";
            Invoke(nameof(Start), 5f);
        }
    }

    private async Task ConnectToSavedServer(string serverId)
    {
        serverStatusText.text = "Подключение к вашему серверу...";

        try
        {
            await InitializeClientAndSession();

            var result = await client.ReadStorageObjectsAsync(session, new[] {
                new StorageObjectId {
                    Collection = "server_data",
                    Key = serverId,
                    UserId = session.UserId
                }
            });

            // Исправленная проверка количества объектов
            if (result.Objects != null && result.Objects.Count() > 0)
            {
                var firstObject = result.Objects.FirstOrDefault();
                if (firstObject != null)
                {
                    var server = JsonUtility.FromJson<ServerInfo>(firstObject.Value);
                    PlayerPrefs.SetString("SelectedServerId", server.id);
                    SceneManager.LoadScene("MainScene");
                    return;
                }
            }

            await LoadServers();
            SetupUI();
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка подключения: {e.Message}");
            await LoadServers();
            SetupUI();
        }
    }

    private async Task InitializeClientAndSession()
    {
        client = new Client(scheme, host, port, serverKey);
        session = await client.AuthenticateDeviceAsync(
            SystemInfo.deviceUniqueIdentifier,
            create: true,
            username: $"Player_{UnityEngine.Random.Range(1000, 9999)}");

        PlayerPrefs.SetString("nakama_token", session.AuthToken);
    }

    private void SetupUI()
    {
        connectButton.onClick.RemoveAllListeners();
        connectButton.onClick.AddListener(OnConnectClicked);

        refreshButton.onClick.RemoveAllListeners();
        refreshButton.onClick.AddListener(async () => await LoadServers());

        serverDropdown.onValueChanged.RemoveAllListeners();
        serverDropdown.onValueChanged.AddListener(OnServerSelected);
    }

    private async Task LoadServers()
    {
        serverStatusText.text = "Загрузка серверов...";
        serverDropdown.interactable = false;
        connectButton.interactable = false;

        try
        {
            var result = await client.ListStorageObjectsAsync(session, "server_data");
            servers.Clear();
            serverDropdown.ClearOptions();

            // Исправленная проверка и обработка результатов
            if (result.Objects != null)
            {
                foreach (var obj in result.Objects)
                {
                    try
                    {
                        var server = JsonUtility.FromJson<ServerInfo>(obj.Value);
                        if (server != null)
                        {
                            server.id = obj.Key;
                            servers.Add(server);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Ошибка парсинга: {e.Message}");
                    }
                }
            }

            if (servers.Count == 0)
            {
                servers = GetFallbackServers();
            }

            UpdateDropdownOptions();
            serverStatusText.text = "Выберите сервер";
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка загрузки: {e.Message}");
            serverStatusText.text = "Ошибка загрузки серверов";
            servers = GetFallbackServers();
            UpdateDropdownOptions();
        }
        finally
        {
            serverDropdown.interactable = true;
            connectButton.interactable = servers.Count > 0;
        }
    }

    private void UpdateDropdownOptions()
    {
        serverDropdown.ClearOptions();
        var options = servers.Select(s => $"{s.name} [{s.currentPlayers}/{s.maxPlayers}]").ToList();
        serverDropdown.AddOptions(options);

        if (servers.Count > 0)
        {
            OnServerSelected(0);
        }
    }

    private List<ServerInfo> GetFallbackServers()
    {
        return new List<ServerInfo>
        {
            new ServerInfo
            {
                id = "S1",
                name = "Основной сервер",
                status = "online",
                currentPlayers = 0,
                maxPlayers = 100,
                ip = host,
                port = port
            },
            new ServerInfo
            {
                id = "S2",
                name = "Резервный сервер",
                status = "online",
                currentPlayers = 0,
                maxPlayers = 100,
                ip = host,
                port = port
            }
        };
    }

    private void OnServerSelected(int index)
    {
        if (index < 0 || index >= servers.Count) return;

        var server = servers[index];
        serverNameText.text = server.name;
        serverStatusLabel.text = server.status.ToUpper();
        serverStatusLabel.color = server.status == "online" ? Color.green : Color.red;
        playersCountText.text = $"{server.currentPlayers}/{server.maxPlayers} игроков";
    }

    private async void OnConnectClicked()
    {
        int selectedIndex = serverDropdown.value;
        if (selectedIndex < 0 || selectedIndex >= servers.Count) return;

        var server = servers[selectedIndex];
        serverStatusText.text = "Подключение...";
        connectButton.interactable = false;

        try
        {
            PlayerPrefs.SetString("SelectedServerId", server.id);
            PlayerPrefs.SetString("SelectedServerName", server.name);
            await Task.Delay(500);
            SceneManager.LoadScene("CharacterCreation");
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка: {e.Message}");
            serverStatusText.text = "Ошибка подключения";
        }
        finally
        {
            connectButton.interactable = true;
        }
    }
}