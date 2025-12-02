using UnityEngine;

public class Health_Chest : MonoBehaviour
{
    public bool hasOpened = false;
    public GameObject interactUI;
    public int healthBoost = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player"))//IF PLAYER HITS TRIGGER COLLIDER
        {  
            if(!hasOpened)
            {
                col.GetComponent<PlayerHealth>().TakeDamage(-(healthBoost));
                interactUI.SetActive(true);
                hasOpened = true;
            }

        }
    }
}
