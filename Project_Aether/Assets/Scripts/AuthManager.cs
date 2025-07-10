using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

public class AuthManager : MonoBehaviour
{
    private static AuthManager _Instance;

    public static AuthManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new AuthManager();
            }
            return _Instance;

        }
        private set
        {
            _Instance = value;
        }
    }

    public string AuthToken { get; private set; }
    public string UserId { get; private set; }
    public string UserName { get; private set; }

    public void setAuthToken(string token)
    {
        this.AuthToken = token;
    }

    public void setUserId(string playerId)
    {
        this.UserId = playerId;
    }

    public void setUserName(string playerName)
    {
        this.UserName = playerName;
    }

    public bool IsAuthenticated()
    {
        return !string.IsNullOrEmpty(AuthToken) && !string.IsNullOrEmpty(UserId);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Logout()
    {
        AuthToken = null;
        UserId = null;
        UserName = null;
        Debug.Log("User logged out.");
    }

}
