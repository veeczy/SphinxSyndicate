using UnityEngine;

public class Health_Chest : MonoBehaviour
{
    public bool isOpened = false;
    // public GameObject interactUI;
    public SpriteRenderer crate;
    public Sprite openCrateSprite;
    public int healthBoost = 1;
    private bool canOpen;
    private PlayerHealth ph;

     // Start is called once before the first execution of Update after the MonoBehaviour is created
     void Start()
    {
      //  interactUI.SetActive(false);
        ph = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
     }
    void Update()
    {
        if(Input.GetButton("Interact") && !isOpened && canOpen)//Press E to open health crate
        {
            ph.TakeDamage(-(healthBoost));
            crate.sprite = openCrateSprite;
            isOpened = true;
        }
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player"))//IF PLAYER HITS TRIGGER COLLIDER
        {  
            if(!isOpened)
            {
              //  interactUI.SetActive(true);
                canOpen = true;
            }

        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))//IF PLAYER HITS TRIGGER COLLIDER
        {
           // interactUI.SetActive(false);
            canOpen = false;
        }
    }
}
