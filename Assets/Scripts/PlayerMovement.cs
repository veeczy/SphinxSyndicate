using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D myPlayer;
    public float speed = 1.0f;
    public float stopSpeed = 1.0f;
    public Vector2 aimPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myPlayer.linearDamping = stopSpeed;
        myPlayer.gravityScale = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //MOVEMENT
        
        Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        direction.Normalize();
            myPlayer.linearVelocity = direction * speed;
        
        //AIM
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit.collider != null)
        {
            aimPos = new Vector2(hit.point.x, hit.point.y);
        }
       else
       {
           Debug.Log("OUT OF BOUNDS");
       }
       Vector2 targetRot = mousePos - (Vector2)transform.position;
       float angle = Mathf.Atan2(targetRot.y, targetRot.x) * Mathf.Rad2Deg;
       transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
