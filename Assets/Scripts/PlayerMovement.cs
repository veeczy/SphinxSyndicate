using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public Rigidbody2D myPlayer;
    public float speed = 1.0f;
    public float stopSpeed = 1.0f;
    private bool lookRight;

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
    public bool chargeDodge = false;

    [Header("Player Objects")]
    public GameObject weaponObject;
    private bool isDodging = false;
    private bool canDodge = true;
    private Vector2 dodgeStart;
    private Vector2 dodgeEnd;
    private float dodgeTimer;
    public float chargeTimer;

    // Sound effects
    public AudioClip footstepAudio;
    private AudioSource playerAudio;

    [Header("Aim")]
    public Vector2 aimPos;

    // NEW � animator reference
    private Animator anim;

    void Start()
    {
        myPlayer.linearDamping = stopSpeed;
        myPlayer.gravityScale = 0;
        playerSprite = GetComponent<SpriteRenderer>().sprite;
        playerAudio = GetComponent<AudioSource>();
        playerAudio.clip = footstepAudio;

        anim = GetComponent<Animator>(); // NEW
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
    }

    void FixedUpdate()
    {
        if (isDodging)
        {
            dodgeTimer += Time.fixedDeltaTime;
            float t = dodgeTimer / dodgeDuration;
            myPlayer.MovePosition(Vector2.Lerp(dodgeStart, dodgeEnd, t));
            GetComponent<SpriteRenderer>().sprite = dodgeSprite; //need to change this to anim.setbool dodge state to work

            // NEW � during dodge, force idle animation
            anim.SetBool("isWalking", false);
            anim.SetBool("isDodging", true);
            if (t >= 1f)
            {
                isDodging = false;
                GetComponent<SpriteRenderer>().sprite = playerSprite;
                StartCoroutine(DodgeCooldown());
            }

            return;
        }

        //charge dodge stuff
        if(dodgekeypress)
        {
            chargeTimer += Time.fixedDeltaTime;
            if (chargeTimer > 2) chargeDodge = true;
            else chargeDodge = false;
        }
        if(!dodgekeypress)
        {
            if (chargeTimer <= 2) chargeTimer = 0;
        }

        // Movement
        Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        myPlayer.linearVelocity = direction * speed;

        // NEW � walking animation toggle
        bool isMoving = direction.magnitude > 0.1f;
        anim.SetBool("isWalking", isMoving);
        anim.SetBool("isDodging", isDodging);

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
        //Debug.DrawLine(transform.position, aimPos, Color.red);

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
        chargeTimer = 0;
        //this is where we put script for charge dodge movement

        //Debug.Log("Charge Dodged.");
        dodgeclick = false;
    }
    private IEnumerator DodgeCooldown()
    {
        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }
}
