using UnityEngine;
using UnityEngine.UI;

public class AppearanceCustomizer : MonoBehaviour
{
    [System.Serializable]
    public class AppearancePart
    {
        public string partName;
        public Sprite[] options;
    }

    [Header("Appearance Parts")]
    public AppearancePart[] parts;

    [Header("UI References")]
    [SerializeField] private Transform partsContainer;
    [SerializeField] private GameObject partButtonPrefab;
    [SerializeField] private Image characterDisplay;

    private int[] currentSelections;

    private void Start()
    {
        currentSelections = new int[parts.Length];
        CreateCustomizationUI();
        UpdateCharacterDisplay();
    }

    private void CreateCustomizationUI()
    {
        for (int i = 0; i < parts.Length; i++)
        {
            int partIndex = i;
            var part = parts[partIndex];

            // ������� ��������� ��� �����
            var header = new GameObject($"Header_{part.partName}");
            header.transform.SetParent(partsContainer);
            var headerText = header.AddComponent<Text>();
            headerText.text = part.partName;
            headerText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

            // ������� ������ ��� ������ �����
            for (int j = 0; j < part.options.Length; j++)
            {
                int optionIndex = j;
                var button = Instantiate(partButtonPrefab, partsContainer);
                var buttonImage = button.GetComponent<Image>();

                if (buttonImage != null && part.options[optionIndex] != null)
                {
                    buttonImage.sprite = part.options[optionIndex];
                }

                button.GetComponent<Button>().onClick.AddListener(() =>
                {
                    currentSelections[partIndex] = optionIndex;
                    UpdateCharacterDisplay();
                });
            }
        }
    }

    private void UpdateCharacterDisplay()
    {
        // ����� ������ ���� ������ ���������� ����������� ���������
        // �� ������ currentSelections
    }

    public string GetAppearanceJson()
    {
        // ����������� ������� ������ � JSON
        // ��� ���������� ������ - � �������� ������� ����� ����� ������� ��������������
        return JsonUtility.ToJson(currentSelections);
    }
}