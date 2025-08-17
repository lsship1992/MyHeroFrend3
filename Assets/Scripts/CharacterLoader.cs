using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using UnityEngine;
using System.Threading.Tasks;

public class CharacterLoader : MonoBehaviour
{
    public Character characterPrefab;
    public Transform spawnPoint;

    private async void Start()
    {
        await Task.Delay(1000); // Ждём инициализации Nakama

        var data = await NakamaManager.Instance.LoadCharacterData("characters", "main");
                
        var characterSystem = FindObjectOfType<NakamaCharacterSystem>();
        if (characterSystem == null)
        {
            Debug.LogError("NakamaCharacterSystem not found!");
            return;
        }

        var characterData = await characterSystem.LoadCharacter();

        if (characterData == null)
        {
            Debug.LogError("No character data found!");
            return;
        }

        var character = Instantiate(characterPrefab, spawnPoint.position, Quaternion.identity);

        if (!string.IsNullOrEmpty(characterData.appearanceJson))
        {
            character.FromJson(characterData.appearanceJson);
        }

        // Добавьте здесь ваши компоненты управления
        character.gameObject.AddComponent<PlayerMovement>();
    }
}

// Пример простого компонента движения
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    private void Update()
    {
        var move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        transform.position += move * speed * Time.deltaTime;
    }
}