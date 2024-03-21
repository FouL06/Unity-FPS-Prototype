using UnityEngine;
using UnityEngine.Networking;

public class hostGame : MonoBehaviour {

    private uint roomSize = 10;
    private string roomName;

    private GameNetworkManager networkManager;

    public void Start()
    {
        networkManager = GetComponent<GameNetworkManager>();

        if(networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }
    }

    public void SetRoomName(string serverName)
    {
        roomName = serverName;
    }

    public void SetRoomSize(uint serverSize)
    {
        roomSize = serverSize;
    }

    public void CreateRoom()
    {
        if(roomName != "" && roomName != null)
        {
            Debug.Log("Creating Room: " + roomName + " with room size " + roomSize + " players.");

            networkManager.matchMaker.CreateMatch(roomName, roomSize, true, "", "", "", 0, 0, networkManager.OnMatchCreate);
        }
    }
}
