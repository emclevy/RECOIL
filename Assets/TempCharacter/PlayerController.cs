using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //Constants
    [SerializeField] private float playerSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravity;
    [SerializeField] private float termVelo;
    [SerializeField] private LayerMask ground;
    [SerializeField] private int respawn;
    private enum State { idle, running, jumping, falling };
    private State playerState = State.idle;

    //public delegate void MyDelegate();
    //public event MyDelegate onDeath;
    //public event MyDelegate onRespawn;


    // Variables
    Rigidbody2D rb;
    Animator anim;
    Collider2D coll;
    bool grounded = false;
    bool jump_debounce = false;
    bool facingRight = true;
    bool PlayerControl = true;

    // Input bool x. If x is true, return 1.0f; otherwise return 0.0f
    float Bool2Float(bool x)
    {
        return x ? 1.0f : 0.0f;
    }

    // Awake
    private void Awake()
    {
        rb = transform.GetComponent<Rigidbody2D>();
        anim = transform.GetComponent<Animator>();
        coll = transform.GetComponent<Collider2D>();
    }

    // Fixed update, 50 physics fps
    void FixedUpdate()
    {
        if (PlayerControl)
        {
            Movement();
            AnimationState();
            anim.SetInteger("playerState", (int)playerState);
        }
    }

    void Movement()
    {
        // Input
        float x_input = Bool2Float(Input.GetKey(KeyCode.D)) - Bool2Float(Input.GetKey(KeyCode.A));
        bool space_input = Input.GetKey(KeyCode.Space);

        // Horizontal movement
        rb.velocity = new Vector2(x_input * Time.fixedDeltaTime * playerSpeed, rb.velocity.y);

        // Ground collision
        RaycastHit2D down_hit = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, ~(1 << 8));

        if (down_hit.collider != null)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0.0f);     // Kill vertical velocity
            grounded = true;
        }

        // Jump
        if (space_input && !jump_debounce && grounded)
        {
            jump_debounce = true;
            grounded = false;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            playerState = State.jumping;
        }

        if (!space_input)
        {
            jump_debounce = false;      // Allows player to jump again when space bar is let go
        }

        // Gravity
        rb.velocity += new Vector2(0.0f, gravity * Time.fixedDeltaTime);

        if (rb.velocity.y < termVelo)
        {
            rb.velocity = new Vector2(rb.velocity.x, termVelo);      // Terminal velocity
        }
    }

    void AnimationState()
    {
        if (playerState == State.jumping)
        {
            if (rb.velocity.y < .1f)
            {
                playerState = State.falling;
                Flip(rb.velocity.x);
            }

        }
        else if (playerState == State.falling)
        {
            Flip(rb.velocity.x);
            if (coll.IsTouchingLayers(ground))
                playerState = State.idle;
        }
        else if (Mathf.Abs(rb.velocity.x) > Mathf.Epsilon)
        {
            //Player is moving
            playerState = State.running;
            Flip(rb.velocity.x);
        }
        else
        {
            //Player stopped
            playerState = State.idle;
        }
    }

    private void Flip(float moveHorizontal)
    {
        if (moveHorizontal > 0 && !facingRight || moveHorizontal < 0 && facingRight)
        {
            facingRight = !facingRight;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

     public void Respawn()
    {
        //Respawn the player at checkpoint (last hit)
        //Reset all elements in that checkpoint "area"
        PlayerControl = !PlayerControl;
        Destroy(gameObject);
        //onDeath.Invoke();
    }   
}
