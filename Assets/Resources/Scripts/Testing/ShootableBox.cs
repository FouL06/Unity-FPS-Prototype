using UnityEngine;

public class ShootableBox : MonoBehaviour
{
    public Material material1;
    public Material material2;
    private Renderer rend;
    private bool test;

    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.enabled = true;
    }

    public void TakeDamage(int damageAmount)
    {
        if (!test)
        {
            rend.material = material1;
            test = true;
        }
        else
        {
            rend.material = material2;
            test = false;
        }
    }
}
