using System.Collections;
//using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public Rigidbody2D myPlayer;
    public float speed = 5.0f; // movement speed
    public float stopSpeed = 0.0f; // linear damping for Rigidbody2D
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
    public float chargeDodgeTime;
    public Collider2D hit;
    ContactFilter2D contactFilter;
    LayerMask mask;
    Vector3 offsetPos;
    public GameObject offset;
    bool isSwamp;

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
        // Controller input
        stickAxis = new Vector2(Input.GetAxis("Joystick Aim X"), Input.GetAxis("Joystick Aim Y"));
        if (!controller && (stickAxis.sqrMagnitude > deadzone.sqrMagnitude || stickAxis.sqrMagnitude < -deadzone.sqrMagnitude))
        {
            controller = true;
        }
        else if (controller && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
        {
            controller = false;
        }

        // Dodge input
        if (Input.GetButtonDown("Dodge")) dodgekeypress = true;
        if (Input.GetButtonUp("Dodge"))
        {
            dodgeclick = true;
            dodgekeypress = false;
        }

        // Update canMove from BJ NPC
        if (BlackJackObject != null)
        {
            canMove = BlackJackObject.GetComponent<BlackJack>().canMove;
        }

        // Aim rotation calculation
        if (!controller)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -Camera.main.transform.position.z;
            aimPos = Camera.main.ScreenToWorldPoint(mousePos);
        }
        else
        {
            if (stickAxis.sqrMagnitude > deadzone.sqrMagnitude)
            {
                Vector2 stickPos = stickAxis.normalized;
                aimPos = (Vector2)transform.position + stickPos;
                lastStickPos = stickPos;
            }
            else
            {
                aimPos = (Vector2)transform.position + lastStickPos;
            }
        }

        // Calculate movement direction
        if (canMove)
        {
            direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        }
        else
        {
            direction = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        // Handle dodging
        if (isDodging)
        {
            dodgeTimer += Time.fixedDeltaTime;
            float t = dodgeTimer / dodgeDuration;
            myPlayer.MovePosition(Vector2.Lerp(dodgeStart, dodgeEnd, t));
            anim.SetBool("isWalking", false);
            anim.SetBool("isDodging", true);
            if (t >= 1f)
            {
                isDodging = false;
                StartCoroutine(DodgeCooldown());
            }
            return;
        }

        // Move player
        if (direction.magnitude > 0.01f)
        {
            myPlayer.MovePosition(myPlayer.position + direction * speed * Time.fixedDeltaTime);
        }

        // Walking animation toggle
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

        // Rotate player
        Vector2 aimDir = (Vector2)aimPos - (Vector2)transform.position;
        if (aimDir.sqrMagnitude > 0.001f)
        {
            float targetAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
            float snappedAngle = Mathf.Round(targetAngle / 45f) * 45f; // snap to nearest 45
            transform.rotation = Quaternion.Euler(0, 0, snappedAngle);
        }

        // FLip sprite if aiming backwards
        float flipAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        if (flipAngle > 90 || flipAngle < -90)
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
                StartDodge(aimDir);
            }
            if ((dodgeclick) && chargeDodge)
            {
                StartChargeRoll(aimDir);
            }
        }

        // Charge dodge movement
        if (chargeDodgeStart)
        {
            dodgeTimer += Time.fixedDeltaTime;
            if (hit != null)
            {
                chargeDodgeStart = false;
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
        dodgeDis = Vector3.Distance(dodgeStart, dir);
        if (dodgeDis > 17) { dodgeDistance = .2f; }
        if (dodgeDis < 5) { dodgeDistance = 1.1f; }
        else { dodgeDistance = 1f; }
        //* END MOUSE AIM DODGE STUFF *//

        dodgeEnd = dodgeStart + dir * dodgeDistance;
        dodgeclick = false;
    }

    private void StartChargeRoll(Vector2 dir)
    {
        chargeTimer = 0;
        dodgeTimer = 0f;
        chargeDodgeStart = true;
        dodgeclick = false;
        chargeDodge = false;
    }

    private IEnumerator DodgeCooldown()
    {
        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }
}