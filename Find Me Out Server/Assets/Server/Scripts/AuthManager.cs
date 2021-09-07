using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;

public class AuthManager : MonoBehaviour
{
    public static AuthManager instance;

    private const float CONNECTION_TIMEOUT = 5f;

    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;

    private void Awake()
    {
        if(instance == null)
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                dependencyStatus = task.Result;
                if(dependencyStatus == DependencyStatus.Available)
                {
                    InitializeFirebase();
                }
                else
                {
                    Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
            });

            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        auth = FirebaseAuth.DefaultInstance;
    }

    public void Login(int _clientId, string _email, string _password)
    {
        StartCoroutine(TryLogin(_clientId, _email, _password));
    }

    public void SignUp(int _clientId, string _username, string _email, string _password, string _passwordConfirmation)
    {
        StartCoroutine(TrySignUp(_clientId, _username, _email, _password, _passwordConfirmation));
    }

    public void SignOut(int _clientId)
    {
        Server.clients[_clientId].user = null;
        ServerSend.SignOutFeedback(_clientId, true);
    }

    private IEnumerator TryLogin(int _clientId, string _email, string _password)
    {
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        string message;
        if (LoginTask.Exception != null) 
        {
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
            message = "Login Failed!";
            switch(errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            ServerSend.LoginFeedback(_clientId, false, message);
            yield break;
        }

        FirebaseUser _firebaseUser = LoginTask.Result;
        Server.clients[_clientId].user = FirebaseUserToUser(_firebaseUser);

        message = $"User signed in successfully: {_firebaseUser.DisplayName} ({_firebaseUser.UserId})";
        ServerSend.LoginFeedback(_clientId, true, message);
    }

    private IEnumerator TrySignUp(int _clientId, string _username, string _email, string _password, string _passwordConfirmation)
    {
        string message;
        if(_username == "") {
            message = "Username must be filled";
            ServerSend.SignUpFeedback(_clientId, false, message);
            yield break;
        } 
        else if(_password != _passwordConfirmation)
        {
            message = "Password and Password confirmation is different";
            ServerSend.SignUpFeedback(_clientId, false, message);
            yield break;
        }

        var SignUpTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
        yield return new WaitUntil(predicate: () => SignUpTask.IsCompleted);

        if(SignUpTask.Exception != null) 
        {
            FirebaseException firebaseEx = SignUpTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
            message = "Login Failed!";
            switch(errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WeakPassword:
                    message = "Weak Password";
                    break;
                case AuthError.EmailAlreadyInUse:
                    message = "Email Already In Use";
                    break;
            }
            ServerSend.SignUpFeedback(_clientId, false, message);
            yield break;
        }
        
        // Firebase user has been created.
        FirebaseUser _firebaseUser = SignUpTask.Result;

        if(_firebaseUser != null)
        {
            UserProfile profile = new UserProfile { DisplayName = _username };

            var ProfileTask = _firebaseUser.UpdateUserProfileAsync(profile);
            yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

            if(ProfileTask.Exception != null) {
                Debug.LogError("UpdateUserProfileAsync encountered an error: " + ProfileTask.Exception);
            }
        }

        message = $"Firebase user created successfully: {_firebaseUser.DisplayName} ({_firebaseUser.UserId})";
        ServerSend.SignUpFeedback(_clientId, true, message);
    }

    public User FirebaseUserToUser(FirebaseUser _firebaseUser)
    {
        string _userId = _firebaseUser.UserId;
        string _displayName = _firebaseUser.DisplayName;
        string _email = _firebaseUser.Email;

        return new User(_userId, _displayName, _email);
    }
}