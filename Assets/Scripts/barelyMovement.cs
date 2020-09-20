using UnityEngine;

public class barelyMovement : MonoBehaviour
{
    // The player rigidbody, player animator, previous platform stood on.
    Rigidbody2D rb;
    Animator anim;
    GameObject prevPlatform = null;

    public GameObject deathEffect, dashEffect;
    SpriteRenderer character;

    // Audio bgm.
    public AudioSource gameBgm, deadBgm;

    // Audio for different actions.
    public AudioSource sfxSource;
    public AudioClip jumpSound, dashSound, windSound, deathSound;

    // Gravity scale, jump velocity and movement speed magnitude.
    public float g;
    public float jumpVel;
    float rRunSpeed = 120;
    float lRunSpeed = 120;
    public float step = 0, speed;
    float maxSpeed = 6;
    Vector2 pushPos;

    /** Keeps track of whether player is grounded, currently dashing,
      * is able to dash, and is ascending. */
    bool grounded, moving, dashing, canDash, ascending;

    // X and Y velocities.
    float vy0, vy, vx;
    float topY = 5.4f;

    // Variables for dashing.
    public Vector2 pos, targetPos;
    float x, y;
    float dashLen = Mathf.Sqrt(8f);
    float dashLenComponent = 2f;
    float dashStep = Mathf.Sqrt(0.18f);
    float dashStepComponent = 0.3f;
    float belowPlatformThreshold = 1.45f;

    // Eight directions to dash.
    enum direction { N, NE, E, SE, S, SW, W, NW };
    direction dir = direction.E;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        character = GetComponent<SpriteRenderer>();
        deadBgm.Stop();
        ascending = false;
        dashing = false;
        Invoke("initialize", 3);
        gameBgm.Play();
    }

    void Update()
    {
        // The current position of the player (in X and Y components).
        pos = rb.position;
        x = pos.x;
        y = pos.y;

        // Perform a jump.
        if (Input.GetKeyDown(KeyCode.Z) && grounded && !ascending && Time.timeScale != 0)
        {
            vy0 = jumpVel;
            rb.velocity = new Vector2(rb.velocity.x, vy0 * Time.deltaTime);
            sfxSource.PlayOneShot(jumpSound);
        }

        // Let gravity affect the player.
        else if (!grounded && !dashing && !ascending)
        {
            if (vy > -jumpVel*2)
            {
                vy = vy0 - g * Time.deltaTime;
                rb.velocity = new Vector2(rb.velocity.x, vy * Time.deltaTime);
            }
            vy0 = vy;
        }

        // Initiate a dash.
        if (canDash && Input.GetKeyDown(KeyCode.LeftShift))
            initiateDash();

        // Move in direction of dash if dash isn't completed, else stop dashing.
        if (dashing)
        {
            switch (dir)
            {
                case direction.N:
                    if (pos.y < targetPos.y)
                        y += dashStep;
                    else
                        stopDashing();
                    break;
                case direction.NE:
                    if (pos.x < targetPos.x && pos.y < targetPos.y)
                    {
                        x += dashStepComponent;
                        y += dashStepComponent;
                    }
                    else
                        stopDashing();
                    break;
                case direction.E:
                    if (pos.x < targetPos.x)
                        x += dashStep;
                    else
                        stopDashing();
                    break;
                case direction.SE:
                    if (pos.x < targetPos.x && pos.y > targetPos.y)
                    {
                        x += dashStepComponent;
                        y -= dashStepComponent;
                    }
                    else
                        stopDashing();
                    break;
                case direction.S:
                    if (pos.y > targetPos.y)
                        y -= dashStep;
                    else
                        stopDashing();
                    break;
                case direction.SW:
                    if (pos.x > targetPos.x && pos.y > targetPos.y)
                    {
                        x -= dashStepComponent;
                        y -= dashStepComponent;
                    }
                    else
                        stopDashing();
                    break;
                case direction.W:
                    if (pos.x > targetPos.x)
                        x -= dashStep;
                    else
                        stopDashing();
                    break;
                case direction.NW:
                    if (pos.x > targetPos.x && pos.y < targetPos.y)
                    {
                        x -= dashStepComponent;
                        y += dashStepComponent;
                    }
                    else
                        stopDashing();
                    break;
            }
        }

        // If not dashing, player can move left and right.
        else if (!dashing)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                moving = true;
                vx = rRunSpeed;
                GetComponent<SpriteRenderer>().flipX = true;
                dir = direction.E;
                rb.velocity = new Vector2(vx * Time.deltaTime, rb.velocity.y);
                if (grounded)
                    anim.SetBool("running", true);
                else
                    anim.SetBool("running", false);
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                moving = true;
                vx = lRunSpeed;
                GetComponent<SpriteRenderer>().flipX = false;
                dir = direction.W;
                rb.velocity = new Vector2(-vx * Time.deltaTime, rb.velocity.y);
                if (grounded)
                    anim.SetBool("running", true);
                else
                    anim.SetBool("running", false);
            }
            else
            {
                moving = false;
                vx = 0;
                rb.velocity = new Vector2(vx, rb.velocity.y);
                anim.SetBool("running", false);
            }
        }

        // If player triggers a tornado, move up.
        if (ascending)
        {
            if (pos.y < topY)
                y += dashStep;
            else
            {
                ascending = false;
                x = pos.x;
                y = pos.y;
                vy0 = 0;
                vy = vy0;
            }
        }

        // Move backwards with platform if player is not moving.
        if (!moving)
        {
            pushPos = new Vector2(transform.position.x - step, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, pushPos, speed * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        // Move to the calculated X and Y positions if dashing or ascending from a tornado.
        if (dashing || ascending)
            rb.MovePosition(new Vector2(x, y));
    }

    // Executed when player lands on a platform.
    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.collider.tag == "ground")
        {
            float d = c.collider.GetComponent<platforms>().platformYpos;
            //print("Platform: " + d + "\n" + "Player: " + pos.y + "\n" + "Difference:" + (pos.y - d));
            if (pos.y - d < belowPlatformThreshold)
                ascending = false;
            else
            {
                vy0 = 0;
                vy = 0;
                grounded = true;
            }
            canDash = true;
            dashing = false;

            // Only increment score if player lands on a different platform.
            if (prevPlatform == null)
                prevPlatform = c.collider.gameObject;
            else if (!GameObject.ReferenceEquals(prevPlatform, c.collider.gameObject))
            {
                FindObjectOfType<score>().incrementScore();
                prevPlatform = c.collider.gameObject;
            }
        }
    }

    // Player can dash as long as they're grounded.
    void OnCollisionStay2D(Collision2D c)
    {
        if (c.collider.tag == "ground" && !dashing)
            canDash = true;
    }

    // When leaving a platform, grounded is set to false.
    void OnCollisionExit2D(Collision2D c)
    {
        if (c.collider.tag == "ground")
            grounded = false;
    }

    // If player hits trigger (tornado), start ascending.
    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.tag == "Finish")
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            sfxSource.PlayOneShot(deathSound);
            character.enabled = false;
            Invoke("gameOver", 1);
        }
        else
        {
            x = pos.x;
            y = pos.y;
            ascending = true;
            sfxSource.PlayOneShot(windSound);
        }

    }

    // Set default values.
    void initialize()
    {
        lRunSpeed = 280;
        speed = 2.4f;
        step = 0.1f;
    }

    void gameOver()
    {
        FindObjectOfType<pause>().gameOver();
        gameBgm.Stop();
        deadBgm.Play();
    }

    // Initiate a dash depending on inputted direction.
    void initiateDash()
    {
        canDash = false;
        dashing = true;
        moving = false;
        Instantiate(dashEffect, transform.position, Quaternion.identity);
        sfxSource.PlayOneShot(dashSound);
        if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftArrow))
        {
            dir = direction.NW;
            targetPos = new Vector2(pos.x - dashLenComponent, pos.y + dashLenComponent);
        }
        else if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightArrow))
        {
            dir = direction.NE;
            targetPos = new Vector2(pos.x + dashLenComponent, pos.y + dashLenComponent);
        }
        else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow))
        {
            dir = direction.SW;
            targetPos = new Vector2(pos.x - dashLenComponent, pos.y - dashLenComponent);
        }
        else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow))
        {
            dir = direction.SE;
            targetPos = new Vector2(pos.x + dashLenComponent, pos.y - dashLenComponent);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            dir = direction.N;
            targetPos = new Vector2(pos.x, pos.y + dashLen);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            dir = direction.W;
            targetPos = new Vector2(pos.x - dashLen, pos.y);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            dir = direction.E;
            targetPos = new Vector2(pos.x + dashLen, pos.y);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            dir = direction.S;
            targetPos = new Vector2(pos.x, pos.y - dashLen);
        }
        else if (dir == direction.W)
            targetPos = new Vector2(pos.x - dashLen, pos.y);
        else if (dir == direction.E)
            targetPos = new Vector2(pos.x + dashLen, pos.y);
    }

    // Ends a dash, sets velocities to zero.
    void stopDashing()
    {
        dashing = false;
        vy0 = 0;
        vy = vy0;
        vx = 0;
    }

    public void incrementSpeed()
    {
        if (speed < maxSpeed)
            speed += 0.2f;
    }

    public float getSpeed()
    {
        return speed;
    }
}