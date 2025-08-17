using Nakama;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class NakamaCharacterSystem : MonoBehaviour
{
    private IClient client;
    private ISession session;
    private bool isInitialized;

    public bool IsInitialized => isInitialized;

    private async void Start()
    {
        try
        {
            while (NakamaManager.Instance == null || !NakamaManager.Instance.IsInitialized)
            {
                await Task.Delay(100);
            }

            client = NakamaManager.Instance.Client;
            session = NakamaManager.Instance.Session;
            isInitialized = true;

            Debug.Log("NakamaCharacterSystem initialized successfully");
        }
        catch (Exception e)
        {
            Debug.LogError($"Initialization failed: {e}");
            isInitialized = false;
        }
    }

    public async Task<CharacterClass[]> LoadCharacterClasses()
    {
        if (!isInitialized) return null;

        try
        {
            await NakamaManager.Instance.EnsureSessionIsValid();
            var result = await client.ListStorageObjectsAsync(session, "game_data", limit: 50);

            return result.Objects
                .Where(obj => obj.Key.StartsWith("class_"))
                .Select(obj => JsonUtility.FromJson<CharacterClass>(obj.Value))
                .ToArray();
        }
        catch (ApiResponseException e)
        {
            Debug.LogError($"Failed to load classes: {e.Message}");
            return null;
        }
    }

    public async Task<bool> SaveCharacter(CharacterSaveData characterData)
    {
        if (!isInitialized) return false;

        try
        {
            await NakamaManager.Instance.EnsureSessionIsValid();

            var writeObject = new WriteStorageObject
            {
                Collection = "characters",
                Key = "main",
                Value = JsonUtility.ToJson(characterData)
            };

            await client.WriteStorageObjectsAsync(session, new[] { writeObject });
            return true;
        }
        catch (ApiResponseException e)
        {
            Debug.LogError($"Save failed: {e.Message}");
            return false;
        }
    }

    public async Task<CharacterSaveData> LoadCharacter()
    {
        if (!isInitialized) return null;

        try
        {
            await NakamaManager.Instance.EnsureSessionIsValid();
            var result = await client.ReadStorageObjectsAsync(session, new[]
            {
                new StorageObjectId
                {
                    Collection = "characters",
                    Key = "main",
                    UserId = session.UserId
                }
            });

            if (result.Objects.Any())
            {
                return JsonUtility.FromJson<CharacterSaveData>(result.Objects.First().Value);
            }
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading character: {e}");
            return null;
        }
    }

    [System.Serializable]
    public class CharacterClass
    {
        public string id;
        public string name;
        public string description;
        public int health;
        public int attack;
        public int defense;
        public string[] abilities;
    }

    [System.Serializable]
    public class CharacterSaveData
    {
        public string nickname;
        public int classId;
        public string appearanceJson; // Изменили с appearance на appearanceJson
        public string creationDate;
        public int level;
        public int experience;
    }
}