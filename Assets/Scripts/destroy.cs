using UnityEngine;

public class destroy : MonoBehaviour
{
    public GameObject effect;
    void Start()
    {
        Destroy(effect, 1);
    }
}
