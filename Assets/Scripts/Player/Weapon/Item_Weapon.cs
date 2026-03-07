using UnityEngine;

public class Item_Weapon : MonoBehaviour
{
    private WeaponSwitch weaponScript;
    public int weaponIndex;

    void Start()
    {
        weaponScript = GameObject.FindWithTag("Player").GetComponent<WeaponSwitch>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            weaponScript.weaponInventory[weaponIndex] = true;
            weaponScript.EquipWeapon(weaponIndex);
            Destroy(gameObject);
        }
    }
}
