using UnityEngine;

public class platforms : MonoBehaviour
{
    public GameObject platform;
    Vector2 targetPos;
    static float speed, step = 0;
    float lifetime = 12;
    int platformIndex;
    public float platformYpos;
    
    void Start()
    {
        // Set the platform to automatically be destroyed to save memory.
        Destroy(platform, lifetime);

        // Initialize speeds in 3 seconds.
        Invoke("initialize", 3);

        // Platform index is last character of platform object name.
        platformIndex = platform.name[8] - '0';

        // platformYpos depends on the platform index.
        switch (platformIndex)
        {
            case 1:
                platformYpos = 3;
                break;
            case 2:
                platformYpos = 1;
                break;
            case 3:
                platformYpos = -1;
                break;
            case 4:
                platformYpos = -3;
                break;
            case 5:
                platformYpos = -3;
                break;
        }
    }

    // Move the platforms left.
    void Update()
    {
        targetPos = new Vector2(transform.position.x - step, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed*Time.deltaTime);
    }

    void initialize()
    {
        speed = FindObjectOfType<barelyMovement>().getSpeed();
        step = 0.1f;
    }

    public void incrementSpeed()
    {
        speed = FindObjectOfType<barelyMovement>().getSpeed();
    }

    public void resetSpeed()
    {
        speed = 0;
        step = 0;
    }
}