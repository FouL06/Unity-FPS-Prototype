using UnityEngine;

public class DestroyAfter : MonoBehaviour
{

    public float destroyObject = 15f;

    void Start()
    {
        Destroy(gameObject, destroyObject);
    }
}
