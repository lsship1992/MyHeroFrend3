using Assets.HeroEditor.Common.Scripts.EditorScripts;
using Nakama;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class PlayerCreationData
{
    public string nickname;
    public int classId;
    public string appearanceJson;
}

public class PlayerCustomizationController : MonoBehaviour
{
    [System.Serializable]
    public class CharacterClassConfig
    {
        public string name;
        public string description;
        public float health;
        public float attack;
        public float speed;
        public string skillName;
        public string skillDescription;
        public Color classColor;
    }

    // UI References
    [Header("UI Elements")]
    public TMP_InputField nicknameField;
    public Button confirmButton;
    public TextMeshProUGUI statusDisplay;
    public TextMeshProUGUI classInfoText;
    public TextMeshProUGUI classStatsText;
    public TextMeshProUGUI classAbilityText;

    [Header("Class Buttons")]
    public Button warriorButton;
    public Button mageButton;
    public Button archerButton;

    [Header("Character Editor")]
    public CharacterEditor characterEditor;

    [Header("Class Configurations")]
    public CharacterClassConfig[] availableClasses;

    private int selectedClassIndex = 0;

    private void Start()
    {
        // Initialize buttons
        confirmButton.onClick.AddListener(CreateCharacter);

        warriorButton.onClick.AddListener(() => SelectClass(0));
        mageButton.onClick.AddListener(() => SelectClass(1));
        archerButton.onClick.AddListener(() => SelectClass(2));

        // Set random nickname
        nicknameField.text = "Player" + UnityEngine.Random.Range(1000, 9999);

        // Initialize first class
        UpdateClassDisplay(0);
    }

    public void SelectClass(int classIndex)
    {
        selectedClassIndex = classIndex;
        UpdateClassDisplay(classIndex);

        // Visual feedback for selected button
        warriorButton.interactable = (classIndex != 0);
        mageButton.interactable = (classIndex != 1);
        archerButton.interactable = (classIndex != 2);
    }

    public void UpdateClassDisplay(int classIndex)
    {
        if (classIndex < 0 || classIndex >= availableClasses.Length) return;

        CharacterClassConfig selectedClass = availableClasses[classIndex];

        classInfoText.text = selectedClass.description;
        classStatsText.text = $"Health: {selectedClass.health}\nAttack: {selectedClass.attack}\nSpeed: {selectedClass.speed}";
        classAbilityText.text = $"<color=#{ColorUtility.ToHtmlStringRGB(selectedClass.classColor)}>{selectedClass.skillName}</color>\n{selectedClass.skillDescription}";
    }

    private void OpenCustomization()
    {
        if (characterEditor != null)
        {
            characterEditor.gameObject.SetActive(true);
        }
    }

    private async void CreateCharacter()
    {
        if (string.IsNullOrEmpty(nicknameField.text))
        {
            statusDisplay.text = "Please enter a nickname!";
            return;
        }

        try
        {
            confirmButton.interactable = false;
            statusDisplay.text = "Creating character...";

            string appearanceData = characterEditor?.Character?.ToJson() ?? "{}";

            var creationData = new PlayerCreationData
            {
                nickname = nicknameField.text,
                classId = selectedClassIndex,
                appearanceJson = appearanceData
            };

            // Добавляем проверку подключения
            if (NakamaManager.Instance == null || NakamaManager.Instance.Session == null)
            {
                throw new Exception("Nakama is not initialized!");
            }

            var response = await NakamaManager.Instance.Client.RpcAsync(
                NakamaManager.Instance.Session,
                "create_character_rpc",  // Должно совпадать с именем в register_rpc
                JsonUtility.ToJson(creationData)
            );

            Debug.Log("Character created successfully: " + response.Payload);
            SceneManager.LoadScene("GameScene");
        }
        catch (Exception error)
        {
            Debug.LogError("Creation failed: " + error);
            statusDisplay.text = "Error creating character!";

            // Более детальное отображение ошибки
            if (error is ApiResponseException apiError)
            {
                statusDisplay.text += $"\nStatus: {apiError.StatusCode}";
                if (apiError.Message.Contains("not found"))
                {
                    statusDisplay.text += "\n(RPC function not deployed)";
                }
            }
        }
        finally
        {
            confirmButton.interactable = true;
        }
    }
}