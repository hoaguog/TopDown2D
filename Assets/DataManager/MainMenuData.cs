using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuData : MonoBehaviour

{
    [Header ("Main Menu")]
    [SerializeField]
    private TMP_Text txtUser;
    [SerializeField]
    private TMP_Text txtLevel;
    [SerializeField]
    private TMP_Text txtExp;
    [SerializeField]
    private Button btnPlay;
    [SerializeField]
    private Button btnTutorial;
    [SerializeField]
    private Button btnLogOut;
    [SerializeField]
    private Button btnQuit;

    [Header("Play Panel")]
    [SerializeField]
    private Button btnPlayGame;
    [SerializeField]
    private Button btnBack;
    [SerializeField]
    private Image imgMap;
    [SerializeField]
    private Image imgWeapon;
    [SerializeField]
    private TMP_Dropdown dropWeapon;
    [SerializeField]
    private TMP_Dropdown dropMap;
    [SerializeField]
    private Sprite[] weaponSprite;
    [SerializeField]
    private Sprite[] mapSprite;
    [SerializeField]
    private GameObject panel;
    private UserData currentUser;
    // Start is called before the first frame update
    void Start()
    {
        //declare user
        currentUser = UserDataHolder.Instance?.CurrentUser;

        if (currentUser != null)
        {
            txtUser.text = currentUser.UserNameDT;
            txtLevel.text = currentUser.LevelDT.ToString();
            txtExp.text = currentUser.ExpDT.ToString();
        }
        else
        {
            Debug.LogError("No user data found. Make sure to login first.");
            txtUser.text = "Data error!!!";
            txtLevel.text = "";
            txtExp.text = "";
        }
        //
        dropWeapon.onValueChanged.AddListener(delegate { ImageByDropdown(); });
        dropMap.onValueChanged.AddListener (delegate { ImageByDropdown(); });
        ImageByDropdown();
        CheckWeaponByLevel();
        CheckMapByLevel();
        //Play Panel
        btnBack.onClick.AddListener(BackToMain);
        btnPlay.onClick.AddListener(PlayMenuButton);
        //Main Menu Panel
        btnLogOut.onClick.AddListener(LogOut);
        btnQuit.onClick.AddListener(Application.Quit);
        btnTutorial.onClick.AddListener(Tutorial);
        btnPlayGame.onClick.AddListener(PlayGameButton);

    }

    private void BackToMain()
    {
        if (panel.activeSelf == true)
        {
            panel.SetActive(false);
        }
    }
    private void PlayMenuButton()
    {
        if (panel.activeSelf == false)
        {
            panel.SetActive(true);
        }
    }
    private void LogOut()
    {
        SceneManager.LoadScene("LoginMenu");
    }
    private void Tutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }
    private void CheckWeaponByLevel()
    {
        if (currentUser != null)
        {
            if (currentUser.LevelDT < 10)
            {
                dropWeapon.ClearOptions();
                dropWeapon.AddOptions(new List<string> { "pistol" });
            }
            else if (currentUser.LevelDT < 50)
            {
                dropWeapon.ClearOptions();
                dropWeapon.AddOptions(new List<string> { "pistol", "SMG" });
            }
            else if (currentUser.LevelDT >= 50)
            {
                dropWeapon.ClearOptions();
                dropWeapon.AddOptions(new List<string> { "pistol", "SMG", "rifle" });
            }
            else
            {
                dropWeapon.ClearOptions();
                dropWeapon.AddOptions(new List<string> { "pistol" });
            }
        }
        else return;
    }
    private void CheckMapByLevel()
    {
        if (currentUser != null)
        {
            if (currentUser.LevelDT < 10)
            {
                dropMap.ClearOptions();
                dropMap.AddOptions(new List<string> { "Wood" });
            }
            else if (currentUser.LevelDT < 50)
            {
                dropMap.ClearOptions();
                dropMap.AddOptions(new List<string> { "Wood", "Desert" });
            }
            else if (currentUser.LevelDT >= 50)
            {
                dropMap.ClearOptions();
                dropMap.AddOptions(new List<string> { "Wood", "Desert", "Room" });
            }
            else
            {
                dropMap.ClearOptions();
                dropMap.AddOptions(new List<string> { "Wood" });
            }
        }
        else return;
    }
    private void ImageByDropdown()
    {
        switch (dropWeapon.value)
        {
            case 0:
                imgWeapon.sprite = weaponSprite[0];
                break;
            case 1:
                imgWeapon.sprite = weaponSprite[1];
                break;
            case 2:
                imgWeapon.sprite = weaponSprite[2];
                break;
            default:
                imgWeapon.sprite = weaponSprite[0];
                break;
        }
        switch (dropMap.value)
        {
            case 0:
                imgMap.sprite = mapSprite[0];
                break;
            case 1:
                imgMap.sprite = mapSprite[1];
                break;
            default:
                imgMap.sprite = mapSprite[0];
                break;
        }
    }
    private void PlayGameButton()
    {
        Time.timeScale = 1;
        string selectedWeapon = dropWeapon.options[dropWeapon.value].text;
        string selectedMap = dropMap.options[dropMap.value].text;
        PlayerPrefs.SetString("SelectedWeapon", selectedWeapon);
        PlayerPrefs.Save();
        switch (selectedMap)
        {
            case "Wood":
                SceneManager.LoadScene("Wood");
                break;
            case "Desert":
                SceneManager.LoadScene("Desert");
                break;
            case "Room":
                SceneManager.LoadScene("Room");
                break;
            default:
                SceneManager.LoadScene("Wood");
                break;
        }
    }
}
