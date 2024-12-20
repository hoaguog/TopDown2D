using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Metadata;

public class PlayerAudio : MonoBehaviour
{
    private Player player;
    [SerializeField]
    private AudioSource movementAudio;
    //movement
    [SerializeField]
    private AudioClip runningSound;
    [SerializeField]
    private AudioClip walkingSound;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        MovementSoundManager();

    }


    //movement sound
    private void MovementSoundManager()
    {
        if (!movementAudio.isPlaying)
        {
            if (player.moving)
            {
                movementAudio.clip = player.walk ? walkingSound : runningSound;
                movementAudio.Play();
            }
            else
            {
                movementAudio.Stop();
            }
        }
    }

}
