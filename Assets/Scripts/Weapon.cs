using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private DataReadWrite data;
    [SerializeField]
    private WeaponAudio weaponSound;
    public GameObject bulletPrefab;
    public GameObject knifeZonePrefab;

    //Gun info
    [SerializeField]
    private float fireRange;
    [SerializeField]
    public string weaponType { get; private set; }
    [SerializeField]
    public string weaponName { get; private set; }
    public float weaponSpeedFire { get; private set; }
    public float weaponReloadTime { get; private set; }
    public int ammoCapacity { get; private set; }
    public int magazineSize { get; private set; }
    public int remainingAmmo { get; private set; }
    //private bool countinueFire = true;


    public bool reloading { get; private set; } = false;
    public bool isFire { get; private set; } = false;
    public bool gunEmty { get; private set; } = false;
    public bool IsKnife { get; private set; } = false;

    //recoil control declare
    private float recoilAngle = 0f,
                  maxRecoilAngle = 2f;

    public float recoilIncrease { get; private set; } = 0.2f;
    //private float currentRecoilAngle = 0f;
    public bool recoilReseted = false;

    public Transform weaponPosFire;
    public Transform initialPosFire;
    public Transform knifePos;
    private int ammoType = 0;
    //public float weaponSpeedFire { get { return rateFire; } }
    //public string weaponType { get { return typeOfWeapon; } }
    //public float weaponReloadTime { get { return reloadTime; } }

    // Start is called before the first frame update
    void Start()
    {
        LoadWeaponInfo();
        switch (weaponName)
        {
            case ("pistol"): ammoType = 0; break;
            case ("SMG"): ammoType = 1; break;
            case ("rifle"): ammoType = 2; break;
        }
        //Debug.Log("Name = " + weaponName + weaponType + ammoCapacity + remainingAmmo + magazineSize);
    }
    public void LoadWeaponInfo()
    {
        if (data)
        {
            this.weaponName = data.weaponNameIG;
            this.weaponReloadTime = data.weaponReloadTimeIG;
            this.weaponSpeedFire = data.weaponRateFireIG;
            this.weaponType = data.weaponTypeIG;
            this.fireRange = data.weaponRangeIG;
            this.ammoCapacity = data.weaponAmmoCapacityIG;
            this.magazineSize = data.weaponMagazineSizeIG;
            this.remainingAmmo = this.magazineSize;
            this.recoilIncrease = data.wpRecoilIncreaseIG;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Weapon fire" + isFire);
        //isFire = false;
    }

    //recoi logic  
    public void FireFunc(string shooter)
    {

        if (remainingAmmo > 0)
        {
            weaponSound.PlayShooting();
            GameObject bullets = (Instantiate(bulletPrefab, weaponPosFire.position, weaponPosFire.rotation));
            Bullet bulletComponent = bullets.GetComponent<Bullet>();
            if (bulletComponent != null)
            {
                bulletComponent.SetShooter(shooter);
                bulletComponent.SetAmmotype(ammoType);
            }
            bullets.GetComponent<Rigidbody2D>().AddForce(weaponPosFire.up * bulletPrefab.GetComponent<Bullet>().getAmmoSpeed, ForceMode2D.Impulse);
            ApplyRecoil();
            remainingAmmo--;
            isFire = true;
            StartCoroutine(SetIsFire());

        }
        else
        {
            weaponSound.PlayEmptySound();
            isFire = false;
            gunEmty = true;
        }

    }
    private IEnumerator SetIsFire()
    {
        yield return new WaitForSeconds(0.1f);
        isFire = false;
    }
    private void ApplyRecoil()
    {
        recoilAngle += recoilIncrease;
        recoilAngle = Mathf.Min(recoilAngle, maxRecoilAngle);
        float randomRecoil = Random.Range(-recoilAngle, recoilAngle);
        weaponPosFire.Rotate(0, 0, randomRecoil);
        recoilReseted = false;
    }

    public void ResetRecoil()
    {
        recoilAngle = 0f;
        weaponPosFire.rotation = initialPosFire.rotation;
        recoilReseted = true;
    }

    //reload logic
    public IEnumerator ReloadFunc()
    {
        if (ammoCapacity > 0)
        {
            if (ammoCapacity + remainingAmmo >= magazineSize)
            {
                reloading = true;
                isFire = false;
                weaponSound.PlayReloadSound();
                yield return new WaitForSeconds(weaponReloadTime);
                ammoCapacity = ammoCapacity + remainingAmmo - magazineSize;
                remainingAmmo = magazineSize;
                reloading = false;
            }
            else
            {
                isFire = false;
                reloading = true;
                weaponSound.PlayReloadSound();
                yield return new WaitForSeconds(weaponReloadTime);
                remainingAmmo = ammoCapacity + remainingAmmo;
                ammoCapacity = 0;
                reloading = false;
            }
        }

    }

    public IEnumerator KnifeZone()
    {
        IsKnife = true;
        weaponSound.PlayeMeleeSound();
        yield return new WaitForSeconds(0.4f);
        Instantiate(knifeZonePrefab, knifePos.position, knifePos.rotation);
        yield return new WaitForSeconds(0.2f);
        IsKnife = false;
    }

}

