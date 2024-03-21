using UnityEngine;
using UnityEngine.UI;

public class GrenadeManager : MonoBehaviour
{
    public GameObject grenade;
    public float force = 20f;

    public GameObject dropPoint;
    public GameObject MouseLook;
    private PlayerInput input;

    public int grenadeCount = 2;
    public Text grenadeText;

    void Start()
    {
        input = GetComponent<PlayerInput>();
    }

    void Update()
    {
        if (input.currentInput.inputGrenade)
        {
            Vector3 dropLocation = dropPoint.transform.position;
            GameObject tempGrenade = (GameObject)Instantiate(grenade, dropLocation, dropPoint.transform.rotation);
            tempGrenade.GetComponent<Rigidbody>().AddForce(MouseLook.transform.forward * force, ForceMode.Impulse);
            tempGrenade.SetActive(true);
            grenadeCount--;
        }
    }
}
