using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeServerButton : MonoBehaviour
{
    [SerializeField] private Button changeServerButton;

    private void Start()
    {
        if (changeServerButton != null)
        {
            changeServerButton.onClick.AddListener(OnChangeServerClicked);
        }
    }

    private void OnChangeServerClicked()
    {
        // Устанавливаем флаг смены сервера
        PlayerPrefs.SetInt("ChangingServer", 1);
        PlayerPrefs.Save();

        // Возвращаемся на сцену выбора сервера
        SceneManager.LoadScene("ServerSelection");
    }
}