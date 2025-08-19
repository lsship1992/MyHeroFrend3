using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using UnityEngine;

public class CharacterEditorController : MonoBehaviour
{
    [SerializeField] private Character character; // ���������� Character ������ CharacterEditor

    private void Awake()
    {
        if (character == null)
        {
            character = GetComponent<Character>();
            if (character == null)
            {
                Debug.LogError("Character component not found!");
            }
        }
    }

    public void LoadFromJson(string jsonData)
    {
        if (character == null)
        {
            Debug.LogError("Character reference is missing!");
            return;
        }

        try
        {
            // ������ �������� JSON � ���������
            character.LoadFromJson(jsonData);
            Debug.Log("Character customization loaded successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load character: {e.Message}");
        }
    }
}