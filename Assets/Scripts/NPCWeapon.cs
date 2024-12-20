using System.Collections;
using UnityEngine;

public class NPCWeapon : MonoBehaviour
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
    public bool recoilReseted = false;

    public Transform weaponPosFire;
    public Transform initialPosFire;
    public Transform knifePos;

    private float speedFireTimer = 0f;
    private bool readyFire = true;
    // Start is called before the first frame update
    void Start()
    {
        LoadWeaponInfo();
        Debug.Log("Name = " + weaponName + weaponType + ammoCapacity + remainingAmmo + magazineSize);
    }
    private void LoadWeaponInfo()
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
        //SpeedFireControl();
        if (remainingAmmo <= 0 && ammoCapacity > 0)
        {
            if (!reloading)
            {
                StartCoroutine(ReloadFunc());

            }
        }
    }
    public void Fire()
    {
        if (weaponType != "auto")
        {
            if (readyFire && !reloading)
            {
                FireFunc();
                readyFire = false;
                StartCoroutine(delayShoot());
                Debug.Log("Name = " + weaponName + weaponType + ammoCapacity + remainingAmmo + magazineSize);

            }

        }
        else if (weaponType == "auto")
        {
            if (!reloading)
            {
                FireFunc();

            }
        }
    }
    private void FixedUpdate()
    {

    }
    public IEnumerator delayShoot()
    {
        yield return new WaitForSeconds(1f);
        readyFire = true;
    }
    private void SpeedFireControl()
    {
        speedFireTimer += Time.deltaTime;
        if (speedFireTimer > weaponSpeedFire)
        {
            readyFire = true;
            speedFireTimer = 0f;
        }
    }


    public void FireFunc()
    {
        Debug.Log("Firing weapon!");

        if (remainingAmmo > 0)
        {
            weaponSound.PlayShooting();
            GameObject bullets = (Instantiate(bulletPrefab, weaponPosFire.position, weaponPosFire.rotation));
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
    //recoi logic  
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
                ammoCapacity = 0;
                remainingAmmo = ammoCapacity + remainingAmmo;
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
