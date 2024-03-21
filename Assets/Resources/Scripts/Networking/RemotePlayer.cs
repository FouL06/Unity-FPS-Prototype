using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(sendInterval = 1, channel = 0)]
public class RemotePlayer : NetworkBehaviour
{

    private bool isInit = false;
    private float nextUpdate = 0;

    [SerializeField]
    private GameObject characterPrefab;

    [SyncVar]
    public string displayName = "unnamed";

    [SyncVar]
    public short ping = 999;

    [SyncVar]
    public NetworkInstanceId spawnedCharacterID;

    public Transform spawn;

    private int hostID;
    private int connID;

    void Start()
    {
        transform.parent = PlayerManager.Instance.transform;
        if (isLocalPlayer)
        {
            //Here we set a name for this player (from PlayerPref for example)
            CmdSetDisplayName("SomeName");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Init some values (first frame only)
        if (isServer && !isInit)
        {
            NetworkIdentity identity = GetComponent<NetworkIdentity>();
            if (identity.connectionToClient != null)
            {
                hostID = identity.connectionToClient.hostId;
                connID = identity.connectionToClient.connectionId;
                isInit = true;
            }
        }
        else
        {
            isInit = true;
        }

        //Update player ping
        if (isServer && !isLocalPlayer && Time.time > nextUpdate)
        {
            nextUpdate = Time.time + GetNetworkSendInterval();

            byte error;
            this.ping = (short)NetworkTransport.GetCurrentRtt(hostID, connID, out error);
        }
        if (ClientScene.FindLocalObject(spawnedCharacterID) == null)
            CmdSpawnPlayer();
    }

    [ClientRpc]
    private void RpcSpawnPlayer(NetworkInstanceId player, Vector3 pos, Quaternion rot)
    {
        ClientScene.FindLocalObject(player).transform.position = pos;
        ClientScene.FindLocalObject(player).transform.rotation = rot;
    }

    [Command]
    void CmdSetDisplayName(string name)
    {
        this.displayName = name;
        this.gameObject.name = "Player " + name;
    }

    [Command]
    public void CmdSpawnPlayer()
    {
        if (ClientScene.FindLocalObject(spawnedCharacterID) == null)
        {
            var go = (GameObject)Instantiate(characterPrefab, transform.position, transform.rotation);
            NetworkServer.AddPlayerForConnection(GetComponent<NetworkIdentity>().connectionToClient, go, 1);
            spawnedCharacterID = go.GetComponent<NetworkIdentity>().netId;

            RpcSpawnPlayer(spawnedCharacterID, transform.position, transform.rotation);
        }
        else
        {
            Debug.LogWarning("Server: Can't spawn two character for the same player");
        }
    }

    public GameObject GetCharacterObject()
    {
        if (spawnedCharacterID == null)
        {
            return null;
        }

        return ClientScene.FindLocalObject(spawnedCharacterID);
    }
}
