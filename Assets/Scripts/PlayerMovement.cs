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
    [Header("Player Objects")]
    public GameObject weaponObject;

    private bool isDodging = false;
    private bool canDodge = true;
    private Vector2 dodgeStart;
    private Vector2 dodgeEnd;
    private float dodgeTimer;

    /* ----------------------------------------------
 * DESIGNERS:
 * To add walking sound effects:
 * 1. Select the Player in the Hierarchy.
 * 2. Find AudioSource component at the bottom.
 * 3. Assign a looping footstep clip in Audio Resource.
 * 4. Uncheck "Play On Awake" and check "Loop".
 * 5. Adjust volume and clip as needed.
 * The sound will play automatically when the player moves
 * and stop when idle or dodging.
 * ---------------------------------------------- */

    // Sound effects
    public AudioClip footstepAudio;
    private AudioSource playerAudio;

    [Header("Aim")]
    public Vector2 aimPos;

    void Start()
    {
        myPlayer.linearDamping = stopSpeed;
        myPlayer.gravityScale = 0;
        playerSprite = GetComponent<SpriteRenderer>().sprite;
        playerAudio = GetComponent<AudioSource>();
        playerAudio.clip = footstepAudio;
    }

    void FixedUpdate()
    {
        if (isDodging)
        {

            dodgeTimer += Time.fixedDeltaTime;
            float t = dodgeTimer / dodgeDuration;
            myPlayer.MovePosition(Vector2.Lerp(dodgeStart, dodgeEnd, t));
            GetComponent<SpriteRenderer>().sprite = dodgeSprite;
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

        // Footstep SFX
        if (!isDodging && direction.magnitude > 0.1f)
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
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        aimPos = mousePos;
        Vector2 aimDir = mousePos - (Vector2)transform.position;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        //Debug.Log(angle);
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

        // Player presses shift to dodge towards mouse.
        if (canDodge)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
                StartDodge(direction); // Dodge
        }
    }

    /* Bewy Pwetty Funcshun. sowwy :,(
    private Vector2 PerpendicularToFacing()
    {
        Vector2 facingDir = transform.right; 
        return new Vector2(-facingDir.y, facingDir.x).normalized; 
    }
    */
    private void StartDodge(Vector2 dir)
    {
        isDodging = true;
        canDodge = false;
        dodgeTimer = 0f;
        dodgeStart = myPlayer.position;
        dodgeEnd = dodgeStart + dir * dodgeDistance;
    }

    private IEnumerator DodgeCooldown()
    {
        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }
}