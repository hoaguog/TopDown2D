using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;

public class UserDataManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public Button createAccountButton;
    public TMP_Text notifiText;

    private void Start()
    {
        loginButton.onClick.AddListener(OnLogin);
        createAccountButton.onClick.AddListener(OnCreateAccount);
    }

    public void OnCreateAccount()
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            notifiText.text = "Username and password cannot be empty!";
            return;
        }

        UserData newUser = new UserData
        {
            UserNameDT = username,
            PasswordDT = EncryptionHelper.Encrypt(password),
            ExpDT = 0,
            LevelDT = 0
        };

        List<UserData> users = UserDataHolder.Instance.LoadUserData();
        if (users.Exists(user => user.UserNameDT == username))
        {
            notifiText.text = "Username already exists!";
            return;
        }

        users.Add(newUser);
        UserDataHolder.Instance.SaveUserData(users);

        notifiText.text = "Account created successfully!";
        UserDataHolder.Instance.GoToMainMenu(newUser);
    }

    public void OnLogin()
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            notifiText.text = "Username and password cannot be empty!";
            return;
        }

        List<UserData> users = UserDataHolder.Instance.LoadUserData();
        UserData user = users.Find(u => u.UserNameDT == username);

        if (user == null)
        {
            notifiText.text = "User not found!";
        }
        else if (EncryptionHelper.Decrypt(user.PasswordDT) == password)
        {
            notifiText.text = $"Welcome back, {user.UserNameDT}!";
            UserDataHolder.Instance.GoToMainMenu(user); 
        }
        else
        {
            notifiText.text = "Incorrect password!";
        }
    }
}
