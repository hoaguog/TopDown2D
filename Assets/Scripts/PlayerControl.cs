using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class Player : MonoBehaviour
{
    //player declare
    public string wpName ="SMG";
    private Animator anim;
    [SerializeField]
    private GameObject KnifeZone;
    public Weapon wp;
    [SerializeField]
    private Transform spawnTransform; //player's transform
    [SerializeField]
    private Rigidbody2D rb; //player's rigidbody2d
    //[SerializeField]
    public bool isDead = false;
    public CameraFolow UIcam;

    //control decalre
    [SerializeField]
    private float moveSpeed = 30,
                  rotationSpeed;
    private float walkSpeed;
    private float moveX, moveY;
    private bool readyFire = false,
                 firing = false;
    private float speedFireTimer = 0f;

    public float heath = 100f;
    public int killCount = 0;
    public int score = 0;
    private bool isInSide = false;
    private float timeInSide = 0f;


    public bool moving { get; private set; } = false;
    public bool walk { get; private set; } = false;
    public bool isReloading { get; private set; } = false;

    public string shooter;
    private UserData currentUser;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("SelectedWeapon"))
        {
            string selectedWeapon = PlayerPrefs.GetString("SelectedWeapon");
            //Debug.Log(selectedWeapon);
            wpName = selectedWeapon;
        }
        else
        {
            Debug.Log("No saved data found.");
        }
        if (gameObject.GetComponentInChildren<DataReadWrite>() != null)
        {
            gameObject.GetComponentInChildren<DataReadWrite>().curentWeaponName = wpName;
            //Debug.Log(($"{gameObject.GetComponentInChildren<DataReadWrite>().curentWeaponName} = {wpName}"));

        }
        else { Debug.Log("readwrite null"); }
    }
    // Start is called before the first frame update
    void Start()
    {
        currentUser = UserDataHolder.Instance?.CurrentUser;
        if (currentUser != null)
        {
            gameObject.name = currentUser.UserNameDT;
        }
        else
        {
            Debug.LogError("No user data found. Make sure to login first.");

        }

        shooter = gameObject.name;
        walkSpeed = moveSpeed / 3;
        anim = this.GetComponent<Animator>();
        killCount = 0;
    }
    public void AddKill()
    {
        killCount++;
        //Debug.Log($"{gameObject.name} kill {killCount} men");
    }

    public void ReloadInfo()
    {
        wp.LoadWeaponInfo();
    }

    // Update is called once per frame
    void Update()
    {
        AnimDetermine();
        if (!isDead)
        {
            FireButtonCheck();
            SpeedFireControl();
            CombatFunc();
        }
        if (heath<=0) {isDead = true;}
        if (isInSide)
        {
            timeInSide += Time.deltaTime;
            if (timeInSide >= 2f)
            {
                score +=5;
                timeInSide = 0f;
            }
        }
        else { timeInSide = 0f; }
    }
    private void FixedUpdate()
    {
        if (!isDead)
        {
            WalkFunc();
            RotationFunc();
        }
    }

    //movement func
    private void MoveFunc(float moveValue)
    {
        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0).normalized;
        transform.position += move * moveValue * Time.deltaTime;
        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0 || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0)
        {
            moving = true;
        }
        else
        {
            moving = false;
        }

        //rb.velocity = new Vector2(moveX, moveY);

    }
    private void WalkFunc()
    {
        if (Input.GetButton("Walk"))
        {
            walk = true;
            MoveFunc(walkSpeed);
        }
        else
        {
            walk = false;
            MoveFunc(moveSpeed);
        }
    }
    private void RotationFunc()
    {
        Vector3 mouseScreenPos = Input.mousePosition;//get mouse screen pos

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos); //convert to world pos

        Vector2 direction = mouseWorldPos - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; //Cal angle, convert to Deg 
        Quaternion playerRotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.Slerp(transform.rotation, playerRotation, Time.deltaTime * rotationSpeed);//smooth rotation
    }
    private void CombatFunc()
    {
        //fire buttton code
        if (wp.weaponType != "auto")
        {
            if (Input.GetButtonDown("Fire1") && readyFire && !wp.reloading && !wp.IsKnife)
            {
                wp.FireFunc(shooter);
                readyFire = false;
            }

        }
        else
        {
            if (readyFire == true && firing == true && !wp.reloading && !wp.IsKnife)
            {
                wp.FireFunc(shooter);
                readyFire = false;
            }
        }

        //reload button code
        if (Input.GetButtonDown("Reload") && !wp.IsKnife &&!wp.reloading)
        {
            StartCoroutine(wp.ReloadFunc());
        }
        if (Input.GetButtonDown("Knife") && !wp.reloading && !wp.IsKnife)
        {
            StartCoroutine(wp.KnifeZone());
        }
    }
    private void SpeedFireControl()
    {
        speedFireTimer += Time.deltaTime;
        if (speedFireTimer > wp.weaponSpeedFire)
        {
            readyFire = true;
            speedFireTimer = 0f;
        }
    }
    private void FireButtonCheck()
    {
        if (Input.GetButton("Fire1"))
        {
            firing = true;
        }
        else
        {
            firing = false;
            if (!wp.recoilReseted)
            {
                wp.ResetRecoil();
            }
        }
    }


    //receive damage
    public void TakeDamage(int damage, string shooter)
    {
        heath -= damage; 
        if (heath <= 0)
        {
            isDead = true;
            UIcam.ShowKillName(shooter, gameObject.name);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if AI enters the Side
        if (other.CompareTag("Side"))
        {
            isInSide = true; // Set flag to true when AI enters the Side

        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if AI leaves the Side
        if (other.CompareTag("Side"))
        {
            isInSide = false; // Set flag to false when AI leaves the Side
        }
    }

    private void AnimDetermine()
    {
        if (wp.weaponName == "pistol")
        {
            anim.SetInteger("AnimWeaponName", 0);
        }
        else if (wp.weaponName == "SMG")
        {
            anim.SetInteger("AnimWeaponName", 1);
        }
        else if (wp.weaponName == "rifle")
        {
            anim.SetInteger("AnimWeaponName", 2);
        }

        if (walk && moving)
        {
            anim.SetBool("AnimWalk", true);
        }
        else if (!walk && moving)
        {
            anim.SetBool("AnimMove", true);
        }
        else
        {
            anim.SetBool("AnimWalk", false);
            anim.SetBool("AnimMove", false);
        }

        if (wp.reloading)
        {
            anim.SetBool("AnimReload", true);
        }
        else
        {
            anim.SetBool("AnimReload", false);

        }
        if (wp.isFire)
        {
            anim.SetBool("AnimIsFire", true);
        }
        else
        {
            anim.SetBool("AnimIsFire", false);
        }

        if (wp.IsKnife)
        {
            anim.SetBool("AnimMelee", true);
        }
        else
        {
            anim.SetBool("AnimMelee", false);
        }

    }
}