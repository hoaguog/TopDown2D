using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserDataHolder : MonoBehaviour
{
    public static UserDataHolder Instance { get; private set; }
    public UserData CurrentUser { get; private set; }
    private string filePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            filePath = Path.Combine(Application.persistentDataPath, "UserData.json");
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    public void SetCurrentUser(UserData user)
    {
        CurrentUser = user;
    }

    public void SaveUserData(List<UserData> users)
    {
        try
        {
            string json = JsonUtility.ToJson(new UserDataList { Users = users }, true);
            File.WriteAllText(filePath, json);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error saving user data: " + ex.Message);
        }
    }

    public List<UserData> LoadUserData()
    {
        if (!File.Exists(filePath))
        {
            return new List<UserData>();
        }

        try
        {
            string json = File.ReadAllText(filePath);
            UserDataList userDataList = JsonUtility.FromJson<UserDataList>(json);
            return userDataList?.Users ?? new List<UserData>();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error loading user data: " + ex.Message);
            return new List<UserData>();
        }
    }

    public void GoToMainMenu(UserData user)
    {
        SetCurrentUser(user);
        SceneManager.LoadScene("MainMenu");
    }
}

[System.Serializable]
public class UserDataList
{
    public List<UserData> Users;
}
