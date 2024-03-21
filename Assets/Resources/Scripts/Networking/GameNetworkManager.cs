using UnityEngine.Networking;

/// <summary>
/// Add PlayerManagement on top of NetworkManager
/// </summary>
public class GameNetworkManager : NetworkManager
{
    private static GameNetworkManager instance;
    public static GameNetworkManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameNetworkManager>();
            }
            return instance;
        }
    }

}