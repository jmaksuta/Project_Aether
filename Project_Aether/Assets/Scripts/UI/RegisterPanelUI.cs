using System;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanelUI : MonoBehaviour
{
    private AuthManager authManager;

    [SerializeField]
    TMPro.TMP_InputField userNameInputField;
    [SerializeField]
    TMPro.TMP_InputField emailInputField;
    [SerializeField]
    TMPro.TMP_InputField passwordInputField;
    [SerializeField]
    TMPro.TMP_InputField confirmPasswordInputField;

    [SerializeField]
    Button RegisterButton;
    [SerializeField]
    Button LoginButton;

    void Awake()
    {
        // Initialize any components or references here if needed
        this.authManager = AuthManager.Instance;

        if (LoginButton != null)
        {
            LoginButton.onClick.AddListener(LoginOnClick);
        }
        if (RegisterButton != null)
        {
            RegisterButton.onClick.AddListener(RegisterOnClick);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LoginOnClick()
    {
        RaiseOnLoginClicked(this, System.EventArgs.Empty);
    }

    private async void RegisterOnClick()
    {
        try
        {
            ValidateFields();
            var response = await ProjectAetherBackendApi.Register(
                this.userNameInputField.text, this.emailInputField.text, this.passwordInputField.text);
            RaiseOnRegisterSuccess(response.message);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Login failed: {ex.Message}");
            RaiseOnRegisterFailure(ex);
        }
    }

    private void ValidateFields()
    {
        string errorMessage = string.Empty;
        if (string.IsNullOrEmpty(userNameInputField.text))
        {
            errorMessage += "Username cannot be empty." + "\n";
        }
        if (string.IsNullOrEmpty(emailInputField.text))
        {
            errorMessage += "Email cannot be empty." + "\n";
        }
        if (string.IsNullOrEmpty(passwordInputField.text))
        {
            errorMessage += "Password cannot be empty." + "\n";
        }
        if (passwordInputField.text != confirmPasswordInputField.text)
        {
            errorMessage += "Passwords do not match." + "\n";
        }
        if (!string.IsNullOrEmpty(errorMessage))
        {
            throw new ArgumentException(errorMessage);
        }
    }

    public delegate void OnRegisterSuccessEventHandler(string message);

    public event OnRegisterSuccessEventHandler OnRegisterSuccess;

    public void RaiseOnRegisterSuccess(string message)
    {
        OnRegisterSuccess?.Invoke(message);
    }

    public delegate void OnRegisterFailureEventHandler(Exception exc);

    public event OnRegisterFailureEventHandler OnRegisterFailure;

    public void RaiseOnRegisterFailure(Exception exc)
    {
        OnRegisterFailure?.Invoke(exc);
    }

    public delegate void OnLoginClickedEventHandler(object sender, System.EventArgs e);

    public event OnLoginClickedEventHandler OnLoginClicked;

    public void RaiseOnLoginClicked(object sender, System.EventArgs e)
    {
        OnLoginClicked?.Invoke(sender, e);
    }

}
