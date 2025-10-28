using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public Rigidbody2D myPlayer;
    public float speed = 1.0f;
    public float stopSpeed = 1.0f;

    [Header("Dodge Settings")]
    public float dodgeDistance = 5f;      
    public float dodgeDuration = 0.15f;   
    public float dodgeCooldown = 0.6f;    

    private bool isDodging = false;
    private bool canDodge = true;
    private Vector2 dodgeStart;
    private Vector2 dodgeEnd;
    private float dodgeTimer;

    [Header("Aim")]
    public Vector2 aimPos;

    void Start()
    {
        myPlayer.linearDamping = stopSpeed;
        myPlayer.gravityScale = 0;
    }

    void FixedUpdate()
    {
        if (isDodging)
        {
            
            dodgeTimer += Time.fixedDeltaTime;
            float t = dodgeTimer / dodgeDuration;
            myPlayer.MovePosition(Vector2.Lerp(dodgeStart, dodgeEnd, t));

            if (t >= 1f)
            {
                isDodging = false;
                StartCoroutine(DodgeCooldown());
            }

            return;
        }

        // Movement
        Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        myPlayer.linearVelocity = direction * speed;

        // Aim Rotation
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        aimPos = mousePos;
        Vector2 aimDir = mousePos - (Vector2)transform.position;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

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

    private System.Collections.IEnumerator DodgeCooldown()
    {
        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }
}