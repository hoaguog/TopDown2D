
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public string shooter;
    public void SetShooter(string shooterName)
    {
        shooter = shooterName;
    }

    public string GetShooter()
    {
        return shooter;
    }

    [SerializeField]
    private float ammoSpeed = 30f;
    [SerializeField]
    private int bulletDamage;
    [SerializeField]
    public int currentAmmoType;
    public void SetAmmotype(int type)
    {
        currentAmmoType = type;
    }
    public int getBulletDamage { get { return bulletDamage; } }

    public float getAmmoSpeed { get { return ammoSpeed; } }
    // Start is called before the first frame update
    void Start()
    {
        switch (currentAmmoType)
        {
            case 0: bulletDamage = 20; break;
            case 1: bulletDamage = 10; break;
            case 2: bulletDamage = 30; break;
            default: bulletDamage = 15; break;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(bulletDamage, shooter);
                if (player.isDead)
                {
                    UpdateShooterKill(shooter);
                    player.isDead = false;
                }
                Destroy(gameObject);

            }
        }
        if (other.CompareTag("BOT"))
        {
            NormalAI bot = other.GetComponent<NormalAI>();
            if (bot != null)
            {
                bot.TakeDamage(bulletDamage, shooter);
                if (bot.isDead)
                {
                    UpdateShooterKill(shooter);
                    bot.isDead = false;
                }
                Destroy(gameObject);
            }
            HardAI hardBOT = other.GetComponent<HardAI>();
            if (hardBOT != null)
            {
                hardBOT.TakeDamage(bulletDamage, shooter);
                if (hardBOT.isDead)
                {
                    UpdateShooterKill(shooter);
                    hardBOT.isDead = false;
                }
                Destroy(gameObject);
            }
            InsaneAI insane = other.GetComponent<InsaneAI>();
            if (insane != null)
            {
                insane.TakeDamage(bulletDamage, shooter);
                if (insane.isDead)
                {
                    UpdateShooterKill(shooter);
                    insane.isDead = false;
                }
                Destroy(gameObject);
            }
            else { Debug.Log("insan null"); }
        }

        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
    private void UpdateShooterKill(string shooterName)
    {
        GameObject shooterObject = GameObject.Find(shooterName);

        if (shooterObject != null)
        {
            Player shooterPlayer = shooterObject.GetComponent<Player>();
            if (shooterPlayer != null)
            {
                shooterPlayer.AddKill();
                shooterPlayer.score++;
            }
            else
            {
                NormalAI shooterBot = shooterObject.GetComponent<NormalAI>();
                if (shooterBot != null)
                {
                    shooterBot.AddKill();
                    shooterBot.score++;
                }
                HardAI hardAI = shooterObject.GetComponent<HardAI>();
                if (hardAI != null)
                {
                    hardAI.AddKill();
                    hardAI.score++;
                }
                InsaneAI insane = shooterObject.GetComponent<InsaneAI>();
                if (insane != null)
                {
                    insane.AddKill();
                    insane.score++;
                }
            }

        }
        else
        {
            Debug.LogWarning($"Shooter {shooterName} not found in Scene!");
        }

    }
}
