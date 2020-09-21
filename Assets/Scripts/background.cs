using UnityEngine;

public class background : MonoBehaviour
{
    // Variables to help background move, creating parallax effect.
    Vector2 targetPos;
    static float step = 0, speed = 0;
    static float maxSpeed = 6.6f;
    static float startX = 49.13f;
    static float endX = -18.63f;
    static float mountainSpd = 0;
    static float treesSpd = 0;
    static float treeSpd = 0;

    // Initialize speed in 3 seconds.
    void Start()
    {
        Invoke("initialize", 3);
    }

    void Update()
    {
        // Move background at different speed depending on which background it is.
        targetPos = new Vector2(transform.position.x - step, transform.position.y);
        switch (tag)
        {
            case "mountains":
                transform.position = Vector2.MoveTowards(transform.position, targetPos, mountainSpd * Time.deltaTime);
                break;
            case "trees":
                transform.position = Vector2.MoveTowards(transform.position, targetPos, treesSpd * Time.deltaTime);
                break;
            case "tree":
                transform.position = Vector2.MoveTowards(transform.position, targetPos, treeSpd * Time.deltaTime);
                break;
        }

        // When background is off camera, put it back at start x position to allow for repeated background.
        if (transform.position.x < endX)
            transform.position = new Vector2(startX, transform.position.y);
    }

    void initialize()
    {
        speed = 2.4f;
        step = 0.1f;
        getSpeed();
    }

    public void incrementSpeed()
    {
        if (speed < maxSpeed)
            speed += 0.2f;
        getSpeed();
    }

    // Different speeds for different parts of background
    void getSpeed()
    {
        switch (tag)
        {
            case "mountains":
                mountainSpd = 0.15f * speed;
                break;
            case "trees":
                treesSpd = 0.4f * speed;
                break;
            case "tree":
                treeSpd = 0.8f * speed;
                break;
        }
    }

    public void resetSpeed()
    {
        mountainSpd = 0;
        treesSpd = 0;
        treeSpd = 0;
        speed = 0;
        step = 0;
    }
}
