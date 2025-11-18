using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{
    [Header("Weapon Settings")]
    public Transform weaponHolder; // Location of weapon
    public GameObject[] weaponPrefabs; // Prefab assignment
    public float switchCooldown = 0.25f;

    private GameObject[] weaponInstances;
    private int currentIndex = 0;
    private float lastSwitchTime = 0f;

    void Start()
    {
        weaponInstances = new GameObject[weaponPrefabs.Length];

        
        for (int i = 0; i < weaponPrefabs.Length; i++)
        {
            GameObject weapon = Instantiate(weaponPrefabs[i], weaponHolder);
            weapon.SetActive(false);
            weaponInstances[i] = weapon;
        }

        // First weapon equip!
        if (weaponInstances.Length > 0)
        {
            EquipWeapon(0);
        }
    }

    void Update()
    {
        if (weaponInstances.Length == 0) return;

        // Switch via Scroll Wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
            EquipWeapon((currentIndex - 1 + weaponInstances.Length) % weaponInstances.Length);
        else if (scroll < 0f)
            EquipWeapon((currentIndex + 1) % weaponInstances.Length);

        // Switch via Numbers 1-9 (Will be fully fleshed out in final version)
        for (int i = 0; i < weaponInstances.Length && i < 9; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
                EquipWeapon(i);
        }
    }

    void EquipWeapon(int index)
    {
        if (Time.time - lastSwitchTime < switchCooldown) return;
        if (index == currentIndex) return;

        // Disable all
        for (int i = 0; i < weaponInstances.Length; i++)
            weaponInstances[i].SetActive(i == index);

        currentIndex = index;
        lastSwitchTime = Time.time;
    }

    public GameObject GetCurrentWeapon()
    {
        if (weaponInstances.Length == 0) return null;
        return weaponInstances[currentIndex];
    }
}