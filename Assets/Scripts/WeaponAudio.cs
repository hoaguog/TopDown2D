using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponAudio : MonoBehaviour
{
    [SerializeField]
    private Weapon weapon;
    [SerializeField]
    private DataReadWrite data;

    [SerializeField] 
    AudioSource soundSource_0;
    [SerializeField]
    AudioSource soundSource_1;

    //Gun sound
    [SerializeField]
    private AudioClip meleeSound;
    [SerializeField]
    private AudioClip pistolSound;
    [SerializeField]
    private AudioClip pistolReloadSound;//2s
    [SerializeField]
    private AudioClip SMGSound;
    [SerializeField]
    private AudioClip SMGReloadSound;
    [SerializeField]
    private AudioClip rifleSound;
    [SerializeField]
    private AudioClip rifleReloadSound;
    [SerializeField]
    private AudioClip emptyShotSound;
    private AudioClip gunFireSound;
    private AudioClip gunReloadSound;


    // Start is called before the first frame update
    void Start()
    {
        SetAudioClip();
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void PlayShooting()
    {
        soundSource_0.PlayOneShot(gunFireSound);

    }
    public void PlayEmptySound()
    {
        soundSource_0.PlayOneShot(emptyShotSound);
    }
    public void PlayReloadSound()
    {
        soundSource_1.PlayOneShot(gunReloadSound);
    }
    public void PlayeMeleeSound()
    {
        soundSource_1.PlayOneShot(meleeSound);
    }
    private void SetAudioClip()
    {
        switch (data.curentWeaponName)
        {
            case "pistol":
                gunFireSound = pistolSound;
                gunReloadSound = pistolReloadSound;
                break;
            case "SMG":
                gunFireSound = SMGSound;
                gunReloadSound = SMGReloadSound;
                break;
            case "rifle":
                gunFireSound = rifleSound;
                gunReloadSound = rifleReloadSound;
                break;
            default:
                gunFireSound = pistolSound;
                gunReloadSound = pistolSound;
                break;
        }
        
    }

}
