using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{
    [Header("Weapon Settings")]
    public Transform weaponHolder; // Location of weapon
    public Transform bulletSpawner;
    public GameObject[] weaponPrefabs; // Prefab assignment
    public float switchCooldown = 0.25f;
    public PlayerMovement pmScript;
    public GameObject[] weaponInstances;
    public bool[] weaponInventory;
    private int currentIndex = 0;
    private float lastSwitchTime = 0f;
    void Start()
    {
        weaponInstances = new GameObject[weaponPrefabs.Length];
        weaponInventory = new bool[weaponPrefabs.Length];
        currentIndex = -1;
        for (int i = 0; i < weaponPrefabs.Length; i++)
        {
            GameObject weapon = Instantiate(weaponPrefabs[i], weaponHolder);
            weapon.GetComponent<Shoot>().bulletSpawn = bulletSpawner;
            weapon.SetActive(false);
            weaponInstances[i] = weapon;
        }

        // First weapon equip!
        if (weaponInstances.Length > 0)
        {
            pmScript.weaponObject = weaponInstances[0];
            EquipWeapon(0);
        }
    }

    void Update()
    {
        if (weaponInstances.Length == 0) return;

        // Switch via Scroll Wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if ((scroll > 0f) || (Input.GetButtonDown("Next Weapon")) && weaponInventory[(currentIndex - 1 + weaponInstances.Length) % weaponInstances.Length])
            EquipWeapon((currentIndex - 1 + weaponInstances.Length) % weaponInstances.Length);
        else if ((scroll < 0f) || (Input.GetButtonDown("Last Weapon"))  && weaponInventory[(currentIndex + 1) % weaponInstances.Length])
            EquipWeapon((currentIndex + 1) % weaponInstances.Length);

        // Switch via Numbers 1-9 (Will be fully fleshed out in final version)
        /*for (int i = 0; i < weaponInstances.Length; i++)
        {
            if (Input.GetKeyDown((1)))
                EquipWeapon(i);
        }*/
        if (Input.GetKeyDown(KeyCode.Alpha1) && weaponInstances.Length > 0 && weaponInventory[0] && pmScript.weaponObject != weaponInstances[0])
        {
            EquipWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && weaponInstances.Length > 1 && weaponInventory[1] && pmScript.weaponObject != weaponInstances[1])
        {
            EquipWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && weaponInstances.Length > 2 && weaponInventory[2] && pmScript.weaponObject != weaponInstances[2])
        {
            EquipWeapon(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && weaponInstances.Length > 3 && weaponInventory[3] && pmScript.weaponObject != weaponInstances[3])
        {
            EquipWeapon(3);
        }
    }

    public void EquipWeapon(int index)
    {
        if (pmScript.weaponObject)
        {
            pmScript.weaponObject.GetComponent<Shoot>().isShooting = false;//Forcibly disables isShooting condition in shoot script before switching weapons
        }
        pmScript.weaponObject = weaponInstances[index];
        if (Time.time - lastSwitchTime < switchCooldown) return;
        if (index == currentIndex) return;
        // Disable all
        for (int i = 0; i < weaponInstances.Length; i++)
        {
            weaponInstances[i].SetActive(i == index);
        }
        currentIndex = index;
        lastSwitchTime = Time.time;
    }

    public GameObject GetCurrentWeapon()
    {
        if (weaponInstances.Length == 0) return null;
        return weaponInstances[currentIndex];
    }
}