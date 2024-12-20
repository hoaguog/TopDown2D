using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEditor;
using UnityEngine;
//using UnityEngine.UIElements;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Xml.Linq;




public class CameraFolow : MonoBehaviour
{
    public Transform playerTransform;
    public Transform tranform;
    public Vector3 offset;
    public float smoothSpeed = 0.125f;
    public GameObject player;
    public TMP_Text txtAmmo;
    public TMP_Text txtHeath;
    public Weapon weapon;
    public TMP_Text[] txtKillInfo;
    private int currentIndex = 0;
    public GameObject[] BOT;
    public GameObject[] rowData;
    public GameObject leaderBoard;
    [SerializeField] private TMP_Text txtName;
    [SerializeField] private TMP_Text txtKill;
    [SerializeField] private TMP_Text txtScore;
    [SerializeField] private Button btnBack;
    [SerializeField]private UserDataManager userDT;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private Transform side;
    [SerializeField] private Transform compass;
    //private UserData user;

    // Start is called before the first frame update
    void Start()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject;
            playerTransform = playerObject.transform;

            weapon = player.GetComponentInChildren<Weapon>();
            if (weapon == null)
            {
                Debug.LogError("Weapon component not found in the player object.");
            }
        }
        else
        {
            Debug.LogError("No object with tag 'Player' found in the scene.");
        }
        BOT = GameObject.FindGameObjectsWithTag("BOT");
        if (BOT != null && rowData != null)
        {
            for (int i = BOT.Length + 1; i < rowData.Length; i++)
            {
                rowData[i].gameObject.SetActive(false);
            }
        }
        leaderBoard.SetActive(false);
        UpdateLeaderboard();
        btnBack.onClick.AddListener(BackToMain);

    }
    public void UpdateLeaderboard()
    {
        {

            if (rowData.Length > 0)
            {
                GameObject playerRow = rowData[0];
                TMP_Text txtName = playerRow.transform.Find("txtName").GetComponent<TMP_Text>();
                TMP_Text txtKill = playerRow.transform.Find("txtKill").GetComponent<TMP_Text>();
                TMP_Text txtScore = playerRow.transform.Find("txtScores").GetComponent<TMP_Text>();

                if (txtName != null) txtName.text = player.name;
                if (txtKill != null) txtKill.text = player.GetComponent<Player>().killCount.ToString();
                if (txtScore != null) txtScore.text = player.GetComponent<Player>().score.ToString();
                if (player.GetComponent<Player>().score >= 50)
                {
                    ShowWinPanel(player.name, player.GetComponent<Player>().killCount, player.GetComponent<Player>().score);
                    return;
                }

                for (int i = 0; i < BOT.Length; i++)
                {
                    if (i + 1 < rowData.Length)
                    {
                        GameObject botRow = rowData[i + 1];
                        txtName = botRow.transform.Find("txtName").GetComponent<TMP_Text>();
                        txtKill = botRow.transform.Find("txtKill").GetComponent<TMP_Text>();
                        txtScore = botRow.transform.Find("txtScores").GetComponent<TMP_Text>();

                        BOTscoreboard botScoreboard = BOT[i].GetComponent<BOTscoreboard>();
                        if (botScoreboard != null)
                        {
                            if (txtName != null) txtName.text = BOT[i].name;
                            if (txtKill != null) txtKill.text = botScoreboard.kill.ToString();
                            if (txtScore != null) txtScore.text = botScoreboard.score.ToString();
                            //Debug.Log($"name {BOT[i].name}, kill = {botScoreboard.kill.ToString()}, score = {botScoreboard.score.ToString()} ");
                            if (botScoreboard.score >= 50)
                            {
                                ShowWinPanel(BOT[i].name, botScoreboard.kill, botScoreboard.score);
                                return;
                            }
                        }
                        else
                        {
                            Debug.LogError("BOTscoreboard component not found on " + BOT[i].name);
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("No rowData.");
            }
        }
    }
    private void ShowWinPanel(string winnerName, int kills, int score)
    {
        //Debug.Log($"Game Over! Winner: {winnerName}, Kills: {kills}, Score: {score}");
        if (!winPanel.activeSelf)
        {
            winPanel.SetActive(true);
        }
        Time.timeScale = 0;
        leaderBoard.SetActive(true);
        txtName.text = winnerName;
        txtKill.text = kills.ToString();
        txtScore.text = score.ToString();
    }
    private void BackToMain()
    {
        Time.timeScale = 1;

        //Debug.Log("back to main clicked");
        if (UserDataHolder.Instance.CurrentUser != null)
        {
            UserData currentUser = UserDataHolder.Instance.CurrentUser;

            int score = player.GetComponent<Player>().score;
            float levelMultiplier = 1f / Mathf.Max(currentUser.LevelDT, 1);
            float weaponMultiplier = GetWeaponMultiplier(player.GetComponent<Player>().wpName);
            int expGained = Mathf.RoundToInt(score * levelMultiplier * weaponMultiplier);

            currentUser.ExpDT += expGained;
            while (currentUser.ExpDT >= CalculateExpForNextLevel(currentUser.LevelDT))
            {
                currentUser.ExpDT -= CalculateExpForNextLevel(currentUser.LevelDT);
                currentUser.LevelDT++;
                //Debug.Log($"Level Up! New Level: {currentUser.LevelDT}");
            }

            List<UserData> users = UserDataHolder.Instance.LoadUserData();
            int userIndex = users.FindIndex(u => u.UserNameDT == currentUser.UserNameDT);
            if (userIndex >= 0)
            {
                users[userIndex] = currentUser;
                UserDataHolder.Instance.SaveUserData(users);
            }
            else
            {
                Debug.LogWarning("Current user not found in user data list.");
            }
        }
        else
        {
            Debug.LogWarning("No current user found.");
        }

        SceneManager.LoadScene("MainMenu");
    }

    private float GetWeaponMultiplier(string name)
    {
        if (name == "pistol") return 2f;
        else if (name == "SMG") return 1.5f;
        else return 1f;
    }
    private int CalculateExpForNextLevel(int level)
    {
        return Mathf.RoundToInt(100 * level * (1 + level * 0.1f));
    }

    // Update is called once per frame
    void Update()
    {
        AlignCompassToSide();
        
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!leaderBoard.activeSelf)
            {
                leaderBoard.SetActive(true);
            }
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            if (leaderBoard.activeSelf)
            {
                leaderBoard.SetActive(false);
            }
        }
        UpdateLeaderboard();
        if (weapon != null)
        {
            txtAmmo.text = $"{weapon.remainingAmmo} / {weapon.ammoCapacity}";
            //Debug.Log($"{weapon.remainingAmmo} / {weapon.ammoCapacity}");
        }
        else
        {
            Debug.LogWarning("Weapon is not assigned or not found.");
        }
        if (player != null)
        {
            txtHeath.text = player.GetComponent<Player>().heath.ToString();
        }
        else
        {
            Debug.LogWarning("Player is not assigned or not found.");

        }
    }
    public void ShowKillName(string killer, string killed)
    {
        for (int i = 0; i < txtKillInfo.Length; i++)
        {
            if (string.IsNullOrEmpty(txtKillInfo[i].text))
            {
                txtKillInfo[i].text = ($"{killed} defeated by {killer}");
                return;
            }
        }

        txtKillInfo[currentIndex].text = ($"{killed} defeated by {killer}"); ;

        currentIndex = (currentIndex + 1) % txtKillInfo.Length;
    }
    public void AlignCompassToSide()
    {
        if (side == null || compass == null)
        {
            Debug.LogWarning("Compass or Side transform is not assigned.");
            return;
        }

        Vector3 directionToSide = (side.position - compass.position).normalized;
        float angle = Mathf.Atan2(directionToSide.y, directionToSide.x) * Mathf.Rad2Deg;
        compass.rotation = Quaternion.Euler(0, 0, angle -90);
    }

    private void LateUpdate()
    {
        Vector3 cameraPosition = playerTransform.position + offset;

        Vector3 smootedPosiotion = Vector3.Lerp(transform.position, cameraPosition, smoothSpeed * Time.deltaTime);

        tranform.position = smootedPosiotion;

        tranform.LookAt(playerTransform);
    }

}

