using UnityEngine;
using TMPro;

// Keeps track of the score, displays it and increments game speed everytime score increases by 5.
public class score : MonoBehaviour
{
    // Keeps track of whether speeds were incremented.
    public bool incrementCalled = false;
    
    // Integer and text to store the score.
    int scor = 0;
    TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.text = "SCORE: " + scor.ToString();
    }

    // Increment speed everytime score increases by 5.
    void Update()
    {
        text.text = "SCORE: " + scor.ToString();
        if (scor % 5 == 0 && scor != 0 && !incrementCalled)
        {
            increment();
            incrementCalled = true;
            Invoke("incrementNotCalled", 3);
        }
    }

    public void incrementScore()
    {
        scor++;
    }

    // Increments game speed.
    void increment()
    {
        FindObjectOfType<barelyMovement>().incrementSpeed();
        FindObjectOfType<platforms>().incrementSpeed();
        FindObjectOfType<spawner>().decrementSpawnTime();
        FindObjectOfType<background>().incrementSpeed();
    }

    void incrementNotCalled()
    {
        incrementCalled = false;
    }
}
