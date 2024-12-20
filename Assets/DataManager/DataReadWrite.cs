using UnityEngine;
using System.Collections.Generic;
using System.IO;
public class DataReadWrite : MonoBehaviour
{
    //[SerializeField]
    //private Weapon Weapon;
    public string weaponNameIG { get; private set; }
    public string weaponTypeIG { get; private set; }
    public float weaponRangeIG { get; private set; }
    public float weaponRateFireIG { get; private set; }
    public float weaponReloadTimeIG { get; private set; }
    public int weaponAmmoCapacityIG { get; private set; }
    public int weaponMagazineSizeIG { get; private set; }
    public float wpRecoilIncreaseIG { get; private set; }

    public string UserNameIG { get; private set; }
    public string PasswordIG { get; private set; }
    public int ExpIG { get; private set; }
    public int LevelIG { get; private set; }

    public string curentWeaponName;
    private string weaponDataPath;

    private void Awake()
    {
        weaponDataPath = Application.persistentDataPath + "/weapon_data.json";
        CreateEncryptedWeaponData();
    }
    private void Start()
    {

        GetWeaponByName(curentWeaponName);

    }

    private void CreateEncryptedWeaponData()
    {

        List<WeaponData> defaultWeapons = new List<WeaponData>
        {
            new WeaponData
            {
                weaponNameDT = "pistol",
                weaponTypeDT = "semi",
                weaponRangeDT = 25.0f,
                weaponRateFireDT = 0.5f,
                weaponReloadTimeDT = 2f,
                weaponAmmoCapacityDT = 52,
                weaponMagazineSizeDT = 13,
                wpRecoilIncreaseDT = 0.2f
            },
            new WeaponData
            {
                weaponNameDT = "SMG",
                weaponTypeDT = "auto",
                weaponRangeDT = 50.0f,
                weaponRateFireDT = 0.09f,
                weaponReloadTimeDT = 3.0f,
                weaponAmmoCapacityDT = 120,
                weaponMagazineSizeDT = 30,
                wpRecoilIncreaseDT = 1f
            },
            new WeaponData
            {
                weaponNameDT = "rifle",
                weaponTypeDT = "auto",
                weaponRangeDT = 100.0f,
                weaponRateFireDT = 0.15f,
                weaponReloadTimeDT = 3f,
                weaponAmmoCapacityDT = 90,
                weaponMagazineSizeDT = 30,
                wpRecoilIncreaseDT = 1.8f
            }
        };

        string json = JsonUtility.ToJson(new WeaponList { Weapons = defaultWeapons }, true);
        string encryptedJson = EncryptionHelper.Encrypt(json);
        File.WriteAllText(weaponDataPath, encryptedJson);

        //Debug.Log("Encrypted weapon data created!");
    }


    public List<WeaponData> LoadWeaponData()
    {
        if (File.Exists(weaponDataPath))
        {
            string encryptedJson = File.ReadAllText(weaponDataPath);
            string decryptedJson = EncryptionHelper.Decrypt(encryptedJson);

            WeaponList weaponList = JsonUtility.FromJson<WeaponList>(decryptedJson);
            return weaponList.Weapons;
        }

        //Debug.LogError("Weapon data file not found!");
        return null;
    }


    public void GetWeaponByName(string weaponName)
    {
        List<WeaponData> weapons = LoadWeaponData();
        if (weapons != null)
        {
            foreach (WeaponData weapon in weapons)
            {
                if (weapon.weaponNameDT.Equals(weaponName, System.StringComparison.OrdinalIgnoreCase))
                {

                    //Debug.Log(weapon.weaponNameDT + weapon.weaponTypeDT);
                    weaponNameIG = weapon.weaponNameDT;
                    weaponTypeIG = weapon.weaponTypeDT;
                    weaponRangeIG = weapon.weaponRangeDT;
                    weaponRateFireIG = weapon.weaponRateFireDT;
                    weaponReloadTimeIG = weapon.weaponReloadTimeDT;
                    weaponAmmoCapacityIG = weapon.weaponAmmoCapacityDT;
                    weaponMagazineSizeIG = weapon.weaponMagazineSizeDT;
                    wpRecoilIncreaseIG = weapon.wpRecoilIncreaseDT; ;
                    //Debug.Log("weaponAmmoCapacityIG = " + weaponAmmoCapacityIG + ", weaponAmmoCapacityDT= " + weapon.weaponAmmoCapacityDT);
                }
            }
        }

    }

}
[System.Serializable]
public class WeaponList
{
    public List<WeaponData> Weapons;
}
