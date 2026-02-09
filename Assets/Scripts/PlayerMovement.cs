using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public Rigidbody2D myPlayer;
    public float speed = 1.0f;
    public float stopSpeed = 1.0f;
    private bool lookRight;

    [Header("Dodge Settings")]
    public float dodgeDistance = 5f;
    public float dodgeDuration = 0.15f;
    public float dodgeCooldown = 0.6f;
    public Sprite dodgeSprite;
    private Sprite playerSprite;
    public bool dodgekeypress = false;

    [Header("Player Objects")]
    public GameObject weaponObject;
    private bool isDodging = false;
    private bool canDodge = true;
    private Vector2 dodgeStart;
    private Vector2 dodgeEnd;
    private float dodgeTimer;

    // Sound effects
    public AudioClip footstepAudio;
    private AudioSource playerAudio;

    [Header("Dodge Audio")]
    public AudioClip dodgeAudio;
    public float dodgeVolume = 1f;

    [Header("Aim")]
    public Vector2 aimPos;
    public Vector2 lastStickPos;

    // NEW � animator reference
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
        lastStickPos = Vector2.right;
        anim = GetComponent<Animator>(); // NEW
    }

    private void Update()
    {
        stickAxis = new Vector2(Input.GetAxis("Joystick Aim X"), Input.GetAxis("Joystick Aim Y"));
        if(!controller && (stickAxis.sqrMagnitude > deadzone.sqrMagnitude || stickAxis.sqrMagnitude < -deadzone.sqrMagnitude))
        {
            controller = true;
        }
        else if(controller && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
        {
            controller = false;
        }
        dodgekeypress = Input.GetButton("Dodge");
    }

    void FixedUpdate()
    {
        if (isDodging)
        {
            dodgeTimer += Time.fixedDeltaTime;
            float t = dodgeTimer / dodgeDuration;
            myPlayer.MovePosition(Vector2.Lerp(dodgeStart, dodgeEnd, t));
            GetComponent<SpriteRenderer>().sprite = dodgeSprite;

            // NEW � during dodge, force idle animation
            anim.SetBool("isWalking", false);

            if (t >= 1f)
            {
                isDodging = false;
                GetComponent<SpriteRenderer>().sprite = playerSprite;
                StartCoroutine(DodgeCooldown());
            }

            return;
        }

        // Movement
        Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        myPlayer.linearVelocity = direction * speed;

        // NEW � walking animation toggle
        bool isMoving = direction.magnitude > 0.1f;
        anim.SetBool("isWalking", isMoving);

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
/*
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        aimPos = mousePos;
        Vector2 aimDir = mousePos - (Vector2)transform.position;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
*/
        if(!controller)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -Camera.main.transform.position.z;
            aimPos = Camera.main.ScreenToWorldPoint(mousePos);
        }
        else
        {
            if(stickAxis.sqrMagnitude  > deadzone.sqrMagnitude)
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
        Vector2 aimDir = (Vector2)aimPos - (Vector2)transform.position;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), controllerTurnSpeed * Time.deltaTime);
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
            if ((Input.GetButtonDown("Dodge")))
                StartDodge(direction); // Dodge
        }
    }

    private void StartDodge(Vector2 dir)
    {
        isDodging = true;
        canDodge = false;
        dodgeTimer = 0f;
        
        dodgeStart = myPlayer.position;
        dodgeEnd = dodgeStart + dir * dodgeDistance;
        
         if (dodgeAudio != null)
        playerAudio.PlayOneShot(dodgeAudio, dodgeVolume);
    }

    private IEnumerator DodgeCooldown()
    {
        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }
}
