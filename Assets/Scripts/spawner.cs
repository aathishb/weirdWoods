using UnityEngine;

public class spawner : MonoBehaviour
{
    // Stores different platforms that can be spawned.
    public GameObject[] platforms;

    // The current elapsed time and the total time to wait before spawning.
    float currentTime;
    float spawnTime = 2.6f;

    // Minimum spawn time.
    float minTime = 0.88f;

    // Stores previous and current spawned objects (indexes of).
    int i0, i1;
    int count = 0;

    void Start()
    {
        currentTime = spawnTime;
    }
    void Update()
    {
        // Decrement currentTime until it's < 0, then spawn a platform.
        if (currentTime <= 0)
        {
            spawn();
            currentTime = spawnTime;
        }
        else
            currentTime -= Time.deltaTime;
    }

    // Spawns (instantiates) a random platform object from the array.
    void spawn()
    {
        if (count == 0)
        {
            i0 = Random.Range(0, 4);
            i1 = Random.Range(0, 4);
            count++;
            spawnPlatform();
        }
        else
        {
            i1 = Random.Range(0, 4);
            spawnPlatform();
        }
        i0 = i1;
    }

    // Spawns the appropriate platform. Spawns a tornado if a high platform directly follows a low platform.
    void spawnPlatform()
    {
        if (i1 == 0 && i0 == 3)
            Instantiate(platforms[4], transform.position, Quaternion.identity);
        else
            Instantiate(platforms[i0], transform.position, Quaternion.identity);
    }

    public void decrementSpawnTime()
    {
        if (spawnTime > minTime)
            spawnTime -= 0.12f;
    }
}