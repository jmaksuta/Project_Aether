using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuthenticationUI : MonoBehaviour
{
    private AuthManager authManager;

    public Button closeButton;
    private LoginPanelUI LoginPanelUI;
    private RegisterPanelUI RegisterPanelUI;
    private TextMeshProUGUI StatusText;

    public GameObject loginPanel;
    public GameObject registerPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(() => gameObject.SetActive(false));
        }

        if (LoginPanelUI == null)
        {
            LoginPanelUI = loginPanel.GetComponent<LoginPanelUI>();
            LoginPanelUI.OnLoginSuccess += OnLoginSuccess; // Subscribe to login success event
            LoginPanelUI.OnLoginFailure += OnLoginFailure; // Subscribe to login failure event
            LoginPanelUI.OnRegisterClicked += (sender, e) =>
            {
                if (RegisterPanelUI != null)
                {
                    RegisterPanelUI.gameObject.SetActive(true);
                    LoginPanelUI.gameObject.SetActive(false);
                }
            };
        }
        if (RegisterPanelUI == null)
        {
            RegisterPanelUI = registerPanel.GetComponent<RegisterPanelUI>();
            RegisterPanelUI.OnRegisterSuccess += OnRegisterSuccess; // Subscribe to register success event
            RegisterPanelUI.OnRegisterFailure += OnRegisterFailure; // Subscribe to register failure event
            RegisterPanelUI.OnLoginClicked += (sender, e) =>
            {
                if (LoginPanelUI != null)
                {
                    LoginPanelUI.gameObject.SetActive(true);
                    RegisterPanelUI.gameObject.SetActive(false);
                }
            };
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        if (LoginPanelUI != null)
        {
            LoginPanelUI.gameObject.SetActive(true);
        }
        if (RegisterPanelUI != null)
        {
            RegisterPanelUI.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (LoginPanelUI != null)
        {
            LoginPanelUI.gameObject.SetActive(false);
        }
        if (RegisterPanelUI != null)
        {
            RegisterPanelUI.gameObject.SetActive(false);
        }
    }

    private void OnLoginSuccess(string authToken, string userId, string userName)
    {
        AuthManager.Instance.setAuthToken(authToken);
        AuthManager.Instance.setUserId(userId);
        AuthManager.Instance.setUserName(userName);
        // Handle successful login here, e.g., update UI or notify other systems
        Debug.Log($"Login successful! Token: {authToken}, User ID: {userId}, Username: {userName}");
        gameObject.SetActive(false); // Close the authentication UI
    }

    private void OnLoginFailure(Exception exc)
    {
        string message = $"Login failed: {exc.Message}";
        // Handle login failure here, e.g., show an error message to the user
        Debug.LogError(message);
        // Optionally, you can show a UI element with the error message
        if (StatusText != null)
        {
            StatusText.text = message;
        }   
    }

    private void OnRegisterSuccess(string message)
    {
        // Handle successful registration here, e.g., update UI or notify other systems
        Debug.Log($"Registration successful. message: {message}");
        RegisterPanelUI.gameObject.SetActive(false); // Close the registration panel
        LoginPanelUI.gameObject.SetActive(true); // Show the login panel
    }

    private void OnRegisterFailure(Exception exc)
    {
        string message = $"Registration failed: {exc.Message}";
        // Handle registration failure here, e.g., show an error message to the user
        Debug.LogError(message);
        // Optionally, you can show a UI element with the error message
        if (StatusText != null)
        {
            StatusText.text = message;
        }
    }

    public void ShowLoginPanel()
    {
        if (LoginPanelUI != null)
        {
            LoginPanelUI.gameObject.SetActive(true);
        }
        if (RegisterPanelUI != null)
        {
            RegisterPanelUI.gameObject.SetActive(false);
        }
    }

    public void ShowRegisterPanel()
    {
        if (RegisterPanelUI != null)
        {
            RegisterPanelUI.gameObject.SetActive(true);
        }
        if (LoginPanelUI != null)
        {
            LoginPanelUI.gameObject.SetActive(false);
        }
    }   

}
