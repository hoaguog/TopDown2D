using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KnifeZoneDamage : MonoBehaviour
{
    public string shooter;
    public void SetShooter(string shooterName)
    {
        shooter = shooterName;
    }
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 0.2f);
        SetShooter(gameObject.name);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(50, shooter);
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
                bot.TakeDamage(50, shooter);
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
                hardBOT.TakeDamage(50, shooter);
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
                insane.TakeDamage(50, shooter);
                if (hardBOT.isDead)
                {
                    UpdateShooterKill(shooter);
                    insane.isDead = false;
                }
                Destroy(gameObject);
            }
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