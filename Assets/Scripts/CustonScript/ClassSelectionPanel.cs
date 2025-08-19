using Assets.HeroEditor.Common.Scripts.EditorScripts;
using Nakama;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class CharacterCreationData
{
    public string playerName;
    public int selectedClassId;
    public string appearanceData;
}

public class ClassSelectionPanel : MonoBehaviour
{
    [System.Serializable]
    public class GameClass
    {
        public string name;
        public string description;
        public Sprite icon;
        public float baseHealth;
        public float attackPower;
        public float movementSpeed;
        public string specialAbility;
        public string abilityDescription;
        public Color classThemeColor;
    }

    [Header("UI Components")]
    public TMP_InputField nameInputField;
    public Button confirmCreationButton;
    public Button openCustomizationButton;
    public TextMeshProUGUI operationStatusText;
    public TextMeshProUGUI classDetailsText;
    public Image classIconImage;
    public TextMeshProUGUI classAttributesText;
    public TextMeshProUGUI classAbilityText;

    [Header("Panels")]
    public GameObject classSelectionScreen;
    public GameObject customizationScreen;

    [Header("Character Editor")]
    public CharacterEditor characterEditor; // Используем класс из ассета

    [Header("Available Classes")]
    public GameClass[] gameClasses;

    private int activeClassIndex = 0;

    private void Start()
    {
        confirmCreationButton.onClick.AddListener(HandleCharacterCreation);
        openCustomizationButton.onClick.AddListener(ShowCustomizationPanel);

        nameInputField.text = "Player_" + UnityEngine.Random.Range(1000, 9999);
        DisplayClassInfo(0);
    }

    public void DisplayClassInfo(int classIndex)
    {
        if (classIndex < 0 || classIndex >= gameClasses.Length) return;

        activeClassIndex = classIndex;
        GameClass currentClass = gameClasses[classIndex];

        classDetailsText.text = currentClass.description;
        classIconImage.sprite = currentClass.icon;
        classAttributesText.text = $"Health: {currentClass.baseHealth}\nAttack: {currentClass.attackPower}\nSpeed: {currentClass.movementSpeed}";
        classAbilityText.text = $"<color=#{ColorUtility.ToHtmlStringRGB(currentClass.classThemeColor)}>{currentClass.specialAbility}</color>\n{currentClass.abilityDescription}";
    }

    private void ShowCustomizationPanel()
    {
        customizationScreen.SetActive(true);
        if (characterEditor != null)
        {
            characterEditor.gameObject.SetActive(true);
        }
    }

    private async void HandleCharacterCreation()
    {
        if (string.IsNullOrEmpty(nameInputField.text))
        {
            operationStatusText.text = "Please enter a name!";
            return;
        }

        try
        {
            confirmCreationButton.interactable = false;
            operationStatusText.text = "Creating character...";

            string appearanceJson = characterEditor != null && characterEditor.Character != null ?
    characterEditor.Character.ToJson() : "{}";

            var creationPayload = new CharacterCreationData
            {
                playerName = nameInputField.text,
                selectedClassId = activeClassIndex,
                appearanceData = appearanceJson
            };

            var serverResponse = await NakamaManager.Instance.Client.RpcAsync(
                NakamaManager.Instance.Session,
                "create_character_endpoint",
                JsonUtility.ToJson(creationPayload)
            );

            SceneManager.LoadScene("MainGameScene");
        }
        catch (Exception error)
        {
            Debug.LogError("Creation error: " + error);
            operationStatusText.text = "Creation failed!";
        }
        finally
        {
            confirmCreationButton.interactable = true;
        }
    }

    public void SelectNextClass() => DisplayClassInfo((activeClassIndex + 1) % gameClasses.Length);
    public void SelectPreviousClass() => DisplayClassInfo((activeClassIndex - 1 + gameClasses.Length) % gameClasses.Length);
}