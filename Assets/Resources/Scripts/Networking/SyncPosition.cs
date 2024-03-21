using UnityEngine;
using UnityEngine.Networking;

public class SyncPosition : NetworkBehaviour
{
    public int sendRatePerSec = 10;
    private float lastSentTime;

    private Transform myTransform;
    [SerializeField]
    float lerpRate = 40;
    [SyncVar]
    private Vector3 syncPos;
    [SyncVar]
    private Vector3 lastSyncedPos;

    void Start()
    {
        myTransform = GetComponent<Transform>();
        syncPos = GetComponent<Transform>().position;
        lastSentTime = Time.time;
        Network.sendRate = 1 / sendRatePerSec;
    }


    void FixedUpdate()
    {
        TransmitPosition();
        LerpPosition();
    }

    void LerpPosition()
    {
        if (!isLocalPlayer)
        {
            myTransform.position = Vector3.Lerp(lastSyncedPos, syncPos, Time.deltaTime * lerpRate);
        }
    }

    [Command]
    void CmdProvidePositionToServer(Vector3 pos)
    {
        lastSyncedPos = syncPos;
        syncPos = pos;
    }

    [ClientCallback]
    void TransmitPosition()
    {
        if (hasAuthority && Network.sendRate <= Time.time - lastSentTime)
        {
            CmdProvidePositionToServer(myTransform.position);
            lastSentTime = Time.time;
        }
    }
}