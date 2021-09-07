using FindMeOut.MainMenu;
using System.Collections;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;

    private const float TIMEOUT = 5f;
    private bool quitWait = false;
    
    [Header("Connection")]
    public TMP_InputField serverIpField;
    public TMP_InputField serverPortField;
    public TextMeshProUGUI connectionStatusText;
    public Button connectButton;

    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TextMeshProUGUI loginStatusText;
    public Button loginButton;
    public Button switchSignUpButton;

    [Header("Sign Up")]
    public TMP_InputField usernameSignUpField;
    public TMP_InputField emailSignUpField;
    public TMP_InputField passwordSignUpField;
    public TMP_InputField confirmationPasswordSignUpField;
    public TextMeshProUGUI signUpStatusText;
    public Button signUpButton;
    public Button switchLoginButton;

    [Header("Main Menu")]
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI mainMenuStatus;
    public Button playButton;
    public Button settingsButton;
    public Button creditsButton;
    //public Button signOutButton;

    [Header("Credits & Settings")]
    public Button creditBackButton;
    public Button settingBackButton;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        connectionStatusText.text = "";
        loginStatusText.text = "";
        signUpStatusText.text = "";
        mainMenuStatus.text = "";

        connectButton.onClick.AddListener(Connect);
        loginButton.onClick.AddListener(Login);
        signUpButton.onClick.AddListener(SignUp);
        //signOutButton.onClick.AddListener(SignOut);
        playButton.onClick.AddListener(EnterLobby);
        switchSignUpButton.onClick.AddListener(() => SwitchCanvas(CanvasType.SignUp));
        switchLoginButton.onClick.AddListener(() => SwitchCanvas(CanvasType.Login));
        creditsButton.onClick.AddListener(() => SwitchCanvas(CanvasType.Credit));
        settingsButton.onClick.AddListener(() => SwitchCanvas(CanvasType.Setting));
        creditBackButton.onClick.AddListener(() => SwitchCanvas(CanvasType.Dashboard));
        settingBackButton.onClick.AddListener(() => SwitchCanvas(CanvasType.Dashboard));
        StartCoroutine(TryConnect("128.199.106.226", int.Parse("26950")));
        StartCoroutine(SendRefresh());
    }

    private IEnumerator SendRefresh()
    {
        yield return new WaitForSeconds(1f);
        ClientSend.SendRefreshData();
        StartCoroutine(SendRefresh());
    }

    private void SwitchCanvas(CanvasType _type)
    {
        CanvasManager.instance.SwitchCanvas(_type);
    }

    private void Connect()
    {
        StartCoroutine(TryConnect("128.199.106.226", int.Parse("26950")));
    }

    private void Login()
    {
        StartCoroutine(TryLogin(emailLoginField.text, passwordLoginField.text));
    }

    private void SignUp()
    {
        StartCoroutine(TrySignUp(usernameSignUpField.text, emailSignUpField.text, passwordSignUpField.text, confirmationPasswordSignUpField.text));
    }

    private void SignOut()
    {
        ClientSend.SignOut();
    }

    private void EnterLobby()
    {
        ClientSend.EnterLobbyRequest();
    }

    public void ConnectionFeedback()
    {
        quitWait = true;
        SwitchCanvas(CanvasType.Login);
    }

    public void LoginFeedback(bool _value, string _message)
    {
        quitWait = true;
        if(!_value) 
        {
            loginStatusText.text = _message;
            StartCoroutine(HideTextAfterTimeout(loginStatusText, 2f));
            return;
        }

        Debug.Log(_message);
        loginStatusText.text = "";
        usernameText.text = Client.instance.user.DisplayName;
        SwitchCanvas(CanvasType.Dashboard);
    }

    public void SignUpFeedback(bool _value, string _message)
    {
        quitWait = true;
        if(!_value) 
        {
            signUpStatusText.text = _message;
            StartCoroutine(HideTextAfterTimeout(signUpStatusText, 2f));
            return;
        }

        Debug.Log(_message);
        signUpStatusText.text = "";
        SwitchCanvas(CanvasType.Login);
    }

    public void SignOutFeedback(bool _value)
    {
        if(!_value) return;

        Client.instance.user = null;
        SwitchCanvas(CanvasType.Login);
    }

    public void EnterLobbyFeedback(bool _value, string _message = null)
    {
        quitWait = true;
        if(!_value) 
        {
            mainMenuStatus.text = _message;
            StartCoroutine(HideTextAfterTimeout(mainMenuStatus, 2f));
            return;
        }

        LoadingScript.instance.LoadingStart();
    }    

    private IEnumerator TryConnect(string _address, int _port)
    {
        connectionStatusText.text = "Connecting to server...";
        connectButton.gameObject.SetActive(false);

        string _ip = null;
        if(ValidateIPv4(_address)) 
        {
            _ip = _address;
        } else {
            //check if ip address is url
            _ip = GetIPFromURL(_address);

            if(_ip == null)
            {
                connectionStatusText.text = "IP address is not valid";
                yield break;
            }
        }
        Client.instance.ConnectToServer(_ip, _port);

        yield return WaitTimeout();
        if(quitWait) yield break;

        connectionStatusText.text = "Connection timeout!";
        connectButton.gameObject.SetActive(true);
    }

    private IEnumerator TryLogin(string _email, string _password)
    {
        loginStatusText.text = "Logging in...";
        ClientSend.Login(_email, _password);
        
        yield return WaitTimeout();
        if(quitWait) yield break;

        loginStatusText.text = "Login timeout!";
        StartCoroutine(HideTextAfterTimeout(signUpStatusText, 2f));
    }

    private IEnumerator TrySignUp(string _username, string _email, string _password, string _confirmationPassword)
    {
        signUpStatusText.text = "Signing up...";
        ClientSend.SignUp(_username, _email, _password, _confirmationPassword);

        yield return WaitTimeout();
        if(quitWait) yield break;

        signUpStatusText.text = "Sign up timeout!";
        StartCoroutine(HideTextAfterTimeout(signUpStatusText, 2f));
    }

    private IEnumerator TryEnterLobby()
    {
        mainMenuStatus.text = "Entering lobby...";
        ClientSend.EnterLobbyRequest();

        yield return WaitTimeout();
        if(quitWait) yield break;

        mainMenuStatus.text = "Enter lobby timeout!";
        StartCoroutine(HideTextAfterTimeout(mainMenuStatus, 2f));
    }

    private IEnumerator WaitTimeout()
    {
        quitWait = false;

        float counter = 0;
        while(counter < TIMEOUT)
        {
            if(quitWait) yield break;

            counter += Time.deltaTime; 
            yield return null;
        }
    }

    private IEnumerator HideTextAfterTimeout(TextMeshProUGUI textMeshProText, float _timeout)
    {
        yield return new WaitForSeconds(_timeout);

        textMeshProText.text = "";
    }

    public static bool ValidateIPv4(string _ipString)
    {
        IPAddress address;
        return IPAddress.TryParse(_ipString, out address);
    }

    public string GetIPFromURL(string _url) 
    {
        _url = _url.Replace("http://", ""); 
        _url = _url.Replace("https://", ""); 
    
        IPHostEntry hosts = Dns.GetHostEntry(_url);
        if(hosts.AddressList.Length > 0) {
            return hosts.AddressList[0].ToString();
        } else {
            return null;
        }
    }
}