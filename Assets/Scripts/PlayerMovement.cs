using System.Collections;
//using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public Rigidbody2D myPlayer;
    public float speed = 1.0f;
    public float stopSpeed = 1.0f;
    private bool lookRight;
    public Vector2 direction;

    // Reference to the BlackJack script on the BJ NPC object
    public bool canMove = true;
    private GameObject BlackJackObject;

    [Header("Dodge Settings")]
    public float dodgeDistance = 1f;
    public float dodgeDis;
    private double disX;
    private double disY;
    public float dodgeDuration = 0.15f;
    public float dodgeCooldown = 0.6f;
    public Sprite dodgeSprite;
    private Sprite playerSprite;
    public bool dodgekeypress = false;
    public bool dodgeclick = false;

    [Header("Charge Dodge Settings")]
    public bool chargeDodge = false;
    public bool chargeDodgeStart = false;
    public float chargedodgeDuration = 0.5f;
    public bool isCharging = false;
    public Collider2D hit;
    ContactFilter2D contactFilter;
    LayerMask mask;
    Vector3 offsetPos;
    public GameObject offset;

    [Header("Player Objects")]
    public GameObject weaponObject;
    public bool isDodging = false;
    private bool canDodge = true;
    private Vector2 dodgeStart;
    private Vector2 dodgeEnd;
    private float dodgeTimer;
    public float chargeTimer;


    [Header("Dodge Audio")]
    public AudioClip footstepAudio;
    private AudioSource playerAudio;
    public AudioClip dodgeAudio;
    public float dodgeVolume = 1f;

    [Header("Aim")]
    public Vector2 aimPos;
    public Vector2 lastStickPos;

    // NEW   animator reference
    private Animator anim;
    //INPUT
    public bool controller = false;//true if controller input detected
    public float controllerTurnSpeed = 15f;//Controller turn sensitivity
    public Vector2 deadzone = new Vector2(0.5f, 0.5f);
    public Vector2 stickAxis;

    void Start()
    {
        myPlayer.linearDamping = stopSpeed;
        myPlayer.gravityScale = 0;
        playerSprite = GetComponent<SpriteRenderer>().sprite;
        playerAudio = GetComponent<AudioSource>();
        playerAudio.clip = footstepAudio;


        BlackJackObject = GameObject.Find("BJ-NPC-Test");
        if (BlackJackObject != null)
        {
            canMove = BlackJackObject.GetComponent<BlackJack>().canMove;
        }


        anim = GetComponent<Animator>(); // NEW

        if (offset == null) //if offset not linked in inspector
        {
            offset = GameObject.Find("GunHolder"); //look for offset object in hierarchy and set it to that
        }
    }

    private void Update()
    {

        //the controller for xbox rt is an axis, not a button
        //dodgekeypress = Input.GetButton("Dodge");
        if (Input.GetButtonDown("Dodge")) dodgekeypress = true;
        if (Input.GetButtonUp("Dodge"))
        {
            dodgeclick = true;
            dodgekeypress = false;
        }

        if (BlackJackObject != null)
        {
            canMove = BlackJackObject.GetComponent<BlackJack>().canMove;
        }

    }

    void FixedUpdate()
    {
        if (isDodging)
        {
            dodgeTimer += Time.fixedDeltaTime;
            float t = dodgeTimer / dodgeDuration;
            myPlayer.MovePosition(Vector2.Lerp(dodgeStart, dodgeEnd, t));
            //GetComponent<SpriteRenderer>().sprite = dodgeSprite; //OLD

            // NEW   during dodge, force idle animation
            anim.SetBool("isWalking", false);
            anim.SetBool("isDodging", true);
            if (t >= 1f)
            {
                isDodging = false;
                //GetComponent<SpriteRenderer>().sprite = playerSprite; //OLD
                StartCoroutine(DodgeCooldown());
            }

            return;
        }

        //raycast stuff
        mask = LayerMask.GetMask("Wall");
        contactFilter.layerMask = mask;
        offsetPos = offset.transform.position;



        //charge dodge stuff
        if (dodgekeypress)
        {
            chargeTimer += Time.fixedDeltaTime;
            if (chargeTimer > .3)
            {
                isCharging = true;
                anim.SetBool("ischarging", isCharging);
            }
            if (chargeTimer > 1.5) chargeDodge = true;
            else chargeDodge = false;
        }
        if (!dodgekeypress)
        {
            if (chargeTimer <= 1.5) chargeTimer = 0;
            isCharging = false;
        }

        //detect if movement should be frozen
        if (!canMove)
        {
            // stop ability to move
            direction = Vector2.zero;
        }
        if (canMove)
        {
            // Movement
            direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            myPlayer.linearVelocity = direction * speed;
        }


        // NEW   walking animation toggle
        bool isMoving = direction.magnitude > 0.1f;
        anim.SetBool("isWalking", isMoving);
        anim.SetBool("isDodging", isDodging);
        anim.SetBool("ischarging", isCharging);
        anim.SetBool("chargeRoll", chargeDodge);

        // Footstep SFX
        if (!isDodging && isMoving)
        {
            if (!playerAudio.isPlaying)
                playerAudio.Play();
        }
        else
        {
            if (playerAudio.isPlaying)
                playerAudio.Stop();
        }

        // Aim Rotation
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;
        aimPos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 aimDir = (Vector2)aimPos - (Vector2)transform.position;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        //more raycast stuff
        //.DrawLine(transform.position, aimPos, Color.red);
        hit = Physics2D.OverlapCircle(offsetPos, .5f, mask);

        //charge dodge movement
        if (chargeDodgeStart)
        {
            dodgeTimer += Time.fixedDeltaTime;

            if (hit != null) 
            {
                chargeDodgeStart = false; //stop charge dodge if hit wall
                Debug.Log("Hit wall.");
                anim.SetBool("isDodging", false);
                StartCoroutine(DodgeCooldown());
            }
            if (hit == null)
            {
                myPlayer.position = myPlayer.position + aimDir * dodgeTimer;
                anim.SetBool("isWalking", false);
                anim.SetBool("isDodging", true);
                anim.SetBool("ischarging", false);
            }

            return;
        }




        if (angle > 90 || angle < -90)
        {
            GetComponent<SpriteRenderer>().flipY = true;
            weaponObject.GetComponent<SpriteRenderer>().flipY = true;
        }
        else
        {
            GetComponent<SpriteRenderer>().flipY = false;
            weaponObject.GetComponent<SpriteRenderer>().flipY = false;
        }

        // Dodge input
        if (canDodge)
        {

            if ((dodgeclick) && !chargeDodge)
            {
                //StartDodge(direction); // Dodge towards Keyboard Movement

                StartDodge(aimDir); // Dodge towards Mouse Click
            }
            if ((dodgeclick) && chargeDodge)
            {
                //StartChargeRoll(direction); //Charge Roll towards Keyboard Movement

                StartChargeRoll(aimDir); //Charge Roll towards Mouse Click
            }

        }
    }

    private void StartDodge(Vector2 dir)
    {
        isDodging = true;
        canDodge = false;
        dodgeTimer = 0f;

        dodgeStart = myPlayer.position;

        if (dodgeAudio != null) { playerAudio.PlayOneShot(dodgeAudio, dodgeVolume); }


        //* THIS IS STUFF FOR MOUSE AIM DODGE, DO NOT DELETE *//

        //calculate distance between mouse aim (where you dodge towards) and player position 
        dodgeDis = Vector3.Distance(dodgeStart, dir);

        if (dodgeDis > 17) { dodgeDistance = .2f; } //if dodge is aimed really farm from arnold, dodge is lessened
        if (dodgeDis < 5) { dodgeDistance = 1.1f; } //if dodge is aimed really close to arnold, dodge is amplified to be farther
        else { dodgeDistance = 1f; }

        //* END MOUSE AIM DODGE STUFF *//


        dodgeEnd = dodgeStart + dir * dodgeDistance;

        dodgeclick = false;
        //Debug.Log("Dodged.");
    }

    private void StartChargeRoll(Vector2 dir)
    {
        chargeTimer = 0; // reset timer of holding button
        dodgeTimer = 0f;
        chargeDodgeStart = true; //turn on charge dodge start
        //Debug.Log("Charge Dodged.");
        dodgeclick = false; //reset clicking button
        chargeDodge = false; //reset charge dodge from holding button being reached
    }
    private IEnumerator DodgeCooldown()
    {
        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }
}
