using UnityEngine.Networking;

/// <summary>
/// Central Character script for simple behavior common to all characters
/// </summary>
public class Player : NetworkBehaviour
{
    void Start()
    {
        GetComponent<PlayerMovement>().enabled = false;
        if (isLocalPlayer)
        {
            //Make the camera start following this character
            //GetComponent<CharacterMovement>().enabled = true;
            //GetComponent<CharacterRotation>().enabled = true;
        }

        if (isServer)
        {
            GetComponent<PlayerMovement>().enabled = true;
        }
    }
}
