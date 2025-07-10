using System;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanelUI : MonoBehaviour
{
    private AuthManager authManager;

    [SerializeField]
    public TMPro.TMP_InputField userNameInputField;
    [SerializeField]
    public TMPro.TMP_InputField passwordInputField;    

    [SerializeField]    
    Button LoginButton;
    [SerializeField]
    Button RegisterButton;

    private void Awake()
    {
        authManager = AuthManager.Instance;

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

    private async void LoginOnClick()
    {
        try
        {
            var response = await ProjectAetherBackendApi.Login(userNameInputField.text, passwordInputField.text);
            RaiseOnLoginSuccess(response.token, response.userId, response.username);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Login failed: {ex.Message}");
            RaiseOnLoginFailure(ex);
        }
    }

    private void RegisterOnClick()
    {
        // Implement registration logic here
        Debug.Log("Register button clicked.");
    }

    public delegate void OnLoginSuccessHandler(string authToken, string userId, string userName);

    public event OnLoginSuccessHandler OnLoginSuccess;

    protected virtual void RaiseOnLoginSuccess(string authToken, string userId, string userName)
    {
        OnLoginSuccess?.Invoke(authToken, userId, userName);
    }

    public delegate void OnLoginFailureHandler(Exception exc);

    public event OnLoginFailureHandler OnLoginFailure;

    protected virtual void RaiseOnLoginFailure(Exception exc)
    {
        OnLoginFailure?.Invoke(exc);
    }

    public event EventHandler<EventArgs> OnRegisterClicked;

    protected virtual void RaiseOnRegisterClicked()
    {
        OnRegisterClicked?.Invoke(RegisterButton, EventArgs.Empty);
    }

}
