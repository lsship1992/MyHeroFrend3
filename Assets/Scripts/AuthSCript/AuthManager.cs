using Nakama;
using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AuthManager : MonoBehaviour
{
    [Header("Server Settings")]
    public string serverIP = "212.8.229.4";
    public string serverKey = "defaultkey";

    [Header("Login Panel")]
    public GameObject loginPanel;
    public TMP_InputField loginEmailInput;
    public TMP_InputField loginPasswordInput;
    public Button loginButton;
    public Button switchToRegisterButton;
    public Button guestButton;
    public Toggle rememberMeToggle; // Чекбокс "Запомнить меня"
    public Button showPasswordButton; // Кнопка "глазик"
    public Image showPasswordIcon; // Иконка (👁️ или 🔒)

    [Header("Register Panel")]
    public GameObject registerPanel;
    public TMP_InputField regEmailInput;
    public TMP_InputField regPasswordInput;
    public TMP_InputField confirmPasswordInput;
    public Button registerButton;
    public Button switchToLoginButton;
    public TMP_Text passwordRulesText;
    public Button showRegPasswordButton; // Кнопка "глазик" для регистрации

    [Header("Common UI")]
    public TMP_Text statusText;

    private IClient client;
    private ISession session;
    private bool isPasswordVisible = false;

    private const string RememberMeKey = "nakama_remember";
    private const string EmailKey = "nakama_email";
    private const string PasswordKey = "nakama_password";

    private void Start()
    {
        Debug.Log("Initializing AuthManager...");
        client = new Client("http", serverIP, 7350, serverKey);

        passwordRulesText.text = "Password must contain:\n- 8+ characters\n- 1 uppercase letter\n- 1 digit\n- Only English characters";

        // Настройка кнопок
        loginButton.onClick.AddListener(OnLoginClick);
        registerButton.onClick.AddListener(OnRegisterClick);
        guestButton.onClick.AddListener(OnGuestClick);
        switchToRegisterButton.onClick.AddListener(SwitchToRegister);
        switchToLoginButton.onClick.AddListener(SwitchToLogin);

        // Валидация пароля
        regPasswordInput.onValueChanged.AddListener(ValidatePasswordVisual);
        confirmPasswordInput.onValueChanged.AddListener(ValidatePasswordMatch);

        // Кнопка "показать пароль" (логин)
        showPasswordButton.onClick.AddListener(TogglePasswordVisibility);

        // Кнопка "показать пароль" (регистрация)
        showRegPasswordButton.onClick.AddListener(ToggleRegPasswordVisibility);

        // Загружаем сохраненные данные, если "Запомнить меня" было включено
        LoadSavedCredentials();

        SwitchToLogin();
        Debug.Log("AuthManager initialized successfully");
    }

    // Загрузка сохраненных данных
    private void LoadSavedCredentials()
    {
        if (PlayerPrefs.GetInt(RememberMeKey, 0) == 1)
        {
            loginEmailInput.text = PlayerPrefs.GetString(EmailKey, "");
            loginPasswordInput.text = PlayerPrefs.GetString(PasswordKey, "");
            rememberMeToggle.isOn = true;
        }
    }

    // Переключение видимости пароля (логин)
    private void TogglePasswordVisibility()
    {
        isPasswordVisible = !isPasswordVisible;
        loginPasswordInput.contentType = isPasswordVisible ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        loginPasswordInput.ForceLabelUpdate();
        showPasswordIcon.color = isPasswordVisible ? Color.green : Color.gray;
    }

    // Переключение видимости пароля (регистрация)
    private void ToggleRegPasswordVisibility()
    {
        isPasswordVisible = !isPasswordVisible;
        regPasswordInput.contentType = isPasswordVisible ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        confirmPasswordInput.contentType = isPasswordVisible ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        regPasswordInput.ForceLabelUpdate();
        confirmPasswordInput.ForceLabelUpdate();
    }

    private void SwitchToLogin()
    {
        Debug.Log("Switching to login panel");
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
        ClearInputs();
        statusText.text = "";
    }

    private void SwitchToRegister()
    {
        Debug.Log("Switching to register panel");
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        ClearInputs();
        statusText.text = "";
    }

    private void ClearInputs()
    {
        loginEmailInput.text = "";
        loginPasswordInput.text = "";
        regEmailInput.text = "";
        regPasswordInput.text = "";
        confirmPasswordInput.text = "";
    }

    private bool ValidatePassword(string password)
    {
        var regex = new Regex(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)[A-Za-z\d]{8,}$");
        bool isValid = regex.IsMatch(password);
        Debug.Log($"Password validation result: {isValid} for password: {password}");
        return isValid;
    }

    private void ValidatePasswordVisual(string password)
    {
        bool isValid = ValidatePassword(password);
        passwordRulesText.color = isValid ? Color.green : Color.red;
        Debug.Log($"Password visual validation: {isValid}");
    }

    private void ValidatePasswordMatch(string password)
    {
        bool isMatch = password == regPasswordInput.text;
        confirmPasswordInput.image.color = isMatch ? Color.green : Color.red;
        Debug.Log($"Password match validation: {isMatch}");
    }

    public async void OnLoginClick()
    {
        Debug.Log("Login button clicked");
        if (string.IsNullOrEmpty(loginEmailInput.text) || string.IsNullOrEmpty(loginPasswordInput.text))
        {
            statusText.text = "Please fill all fields!";
            Debug.LogWarning("Login fields empty");
            return;
        }

        try
        {
            statusText.text = "Logging in...";
            Debug.Log($"Attempting login with email: {loginEmailInput.text}");

            session = await client.AuthenticateEmailAsync(loginEmailInput.text, loginPasswordInput.text);

            statusText.text = "Login successful!";
            Debug.Log($"Login successful. User ID: {session.UserId}");

            // Сохраняем данные, если "Запомнить меня" включено
            if (rememberMeToggle.isOn)
            {
                PlayerPrefs.SetInt(RememberMeKey, 1);
                PlayerPrefs.SetString(EmailKey, loginEmailInput.text);
                PlayerPrefs.SetString(PasswordKey, loginPasswordInput.text);
            }
            else
            {
                PlayerPrefs.DeleteKey(RememberMeKey);
                PlayerPrefs.DeleteKey(EmailKey);
                PlayerPrefs.DeleteKey(PasswordKey);
            }

            PlayerPrefs.SetString("nakama_token", session.AuthToken);
            SceneManager.LoadScene("ServerSelection");
        }
        catch (ApiResponseException e)
        {
            statusText.text = $"Error: {e.Message}";
            Debug.LogError($"Login failed: {e.Message}\n{e.StackTrace}");
        }
    }

    public async void OnRegisterClick()
    {
        Debug.Log("Register button clicked");

        if (string.IsNullOrEmpty(regEmailInput.text) ||
            string.IsNullOrEmpty(regPasswordInput.text) ||
            string.IsNullOrEmpty(confirmPasswordInput.text))
        {
            statusText.text = "Please fill all fields!";
            Debug.LogWarning("Registration fields empty");
            return;
        }

        if (regPasswordInput.text != confirmPasswordInput.text)
        {
            statusText.text = "Passwords don't match!";
            Debug.LogWarning("Password confirmation failed");
            return;
        }

        if (!ValidatePassword(regPasswordInput.text))
        {
            statusText.text = "Password doesn't meet requirements!";
            Debug.LogWarning("Password validation failed");
            return;
        }

        try
        {
            statusText.text = "Registering...";
            Debug.Log($"Attempting registration with email: {regEmailInput.text}");

            session = await client.AuthenticateEmailAsync(
                email: regEmailInput.text,
                password: regPasswordInput.text,
                create: true);

            statusText.text = "Registration successful! Please login.";
            Debug.Log($"Registration successful. User ID: {session.UserId}");

            SwitchToLogin();
        }
        catch (ApiResponseException e)
        {
            statusText.text = $"Error: {e.Message}";
            Debug.LogError($"Registration failed: {e.Message}\n{e.StackTrace}");
        }
    }

    public async void OnGuestClick()
    {
        Debug.Log("Guest login button clicked");
        try
        {
            statusText.text = "Guest login...";

            string deviceId = SystemInfo.deviceUniqueIdentifier;
            if (string.IsNullOrEmpty(deviceId))
            {
                deviceId = System.Guid.NewGuid().ToString();
                Debug.Log($"Generated new device ID: {deviceId}");
            }
            else
            {
                Debug.Log($"Using existing device ID: {deviceId}");
            }

            session = await client.AuthenticateDeviceAsync(deviceId);

            statusText.text = "Guest login successful!";
            Debug.Log($"Guest login successful. User ID: {session.UserId}");

            PlayerPrefs.SetString("nakama_token", session.AuthToken);
            SceneManager.LoadScene("ServerSelection");
        }
        catch (ApiResponseException e)
        {
            statusText.text = $"Error: {e.Message}";
            Debug.LogError($"Guest login failed: {e.Message}\n{e.StackTrace}");
        }
    }
}