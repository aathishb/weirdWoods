using UnityEngine;

// This script isn't being used. Is just a template for the actual movement script: barelyMovement.cs
public class movement : MonoBehaviour
{
    public Rigidbody2D r;

    // Gravity scale, jump velocity and movement step magnitude
    public float g, jumpVel, step;

    // Keeps track of whether player is grounded, currently dashing,
    // is able to dash, is ascending and if they fell off a platform.
    bool grounded, dashing, canDash, ascending, dead;

    float y0, y, vy0, vy;
    float x, d, topY = 5f;
    Vector2 pos, targetPos;
    float dashLen = Mathf.Sqrt(8);
    float dashLenComponent = 2;
    float dashStep = Mathf.Sqrt(80000);
    float dashStepComponent = 200;
    //float belowPlatformThreshold = 1.48f;
    enum direction { N, NE, E, SE, S, SW, W, NW };
    direction dir;

    void start()
    {
        vy0 = jumpVel;
        y0 = -0.2f;
        y = y0;
        ascending = false;
        dead = false;
    }
    void Update()
    {
        pos = transform.position;
        if (!dead && Input.GetKeyDown(KeyCode.Z) && grounded && !ascending)
        {
            vy0 = jumpVel;
            y = y0 + Time.deltaTime*vy0;
        }
        else if (!grounded && !dashing && !ascending)
        {
            y = y0 + Time.deltaTime*vy0;
            if (vy > -jumpVel)
            {
                vy = vy0 - g * Time.deltaTime;
            }
            y0 = y;
            vy0 = vy;
        }
        else if (dead)
        {
            y = y0 + Time.deltaTime * vy0;
            if (vy > -jumpVel)
            {
                vy = vy0 - g * Time.deltaTime;
            }
            y0 = y;
            vy0 = vy;
        }
        
        if (!dead && canDash && Input.GetKeyDown(KeyCode.LeftShift))
        {
            canDash = false;
            dashing = true;
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

        if (!dead && dashing)
        {
            switch (dir)
            {
                case direction.N:
                    if (pos.y < targetPos.y)
                        y0 += dashStep;
                    else
                        dashing = false;
                    break;
                case direction.NE:
                    if (pos.x < targetPos.x && pos.y < targetPos.y)
                    {
                        x += dashStepComponent;
                        y0 += dashStepComponent;
                    }
                    else
                        dashing = false;
                    break;
                case direction.E:
                    if (pos.x < targetPos.x)
                        x += dashStep;
                    else
                        dashing = false;
                    break;
                case direction.SE:
                    if (pos.x < targetPos.x && pos.y > targetPos.y)
                    {
                        x += dashStepComponent;
                        y0 -= dashStepComponent;
                    }
                    else
                        dashing = false;
                    break;
                case direction.S:
                    if (pos.y > targetPos.y)
                        y0 -= dashStep;
                    else
                        dashing = false;
                    break;
                case direction.SW:
                    if (pos.x > targetPos.x && pos.y > targetPos.y)
                    {
                        x -= dashStepComponent;
                        y0 -= dashStepComponent;
                    }
                    else
                        dashing = false;
                    break;
                case direction.W:
                    if (pos.x > targetPos.x)
                        x -= dashStep;
                    else
                        dashing = false;
                    break;
                case direction.NW:
                    if (pos.x > targetPos.x && pos.y < targetPos.y)
                    {
                        x -= dashStepComponent;
                        y0 += dashStepComponent;
                    }
                    else
                        dashing = false;
                    break;
            }
            y = y0;
        }
        else if (!dead && Input.GetKey(KeyCode.RightArrow) && !dashing)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            dir = direction.E;
            x = x + step;
        }
        else if (!dead && Input.GetKey(KeyCode.LeftArrow) && !dashing)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            dir = direction.W;
            x = x - step;
        }

        if (ascending)
        {
            if (pos.y < topY)
            {
                y0 += dashStep;
                y = y0;
            }
            else
                ascending = false;
        }
    }

    void FixedUpdate()
    {
        r.MovePosition(new Vector2(x, y) * Time.deltaTime);
        if (pos.y < -7)
            FindObjectOfType<pause>().gameOver();
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.collider.tag == "ground")
        {
            /*d = c.collider.GetComponent<platforms>().platformYpos;
            print("Platform: " + d + "\n" + "Player: " + pos.y + "\n" + "Difference:" + (pos.y - d));
            if (pos.y - d < belowPlatformThreshold)
            {
                dead = true;
                GetComponent<Collider2D>().enabled = false;
                c.collider.GetComponent<Collider2D>().enabled = false;
            }
            else
            {*/
                vy0 = 0;
                vy = 0;
                grounded = true;
                if (!dashing)
                    canDash = true;
            //}

        }
    }

    void OnCollisionStay2D(Collision2D c)
    {
        if (c.collider.tag == "ground" && !dashing)
        {
            canDash = true;
        }
    }

    void OnCollisionExit2D(Collision2D c)
    {
        if (c.collider.tag == "ground")
        {
            grounded = false;
        }
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        ascending = true;
    }
}