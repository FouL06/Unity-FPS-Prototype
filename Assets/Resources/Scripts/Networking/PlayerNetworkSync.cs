using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// For local player, dispatch received position to CharacterNetworkInput
/// For distant player, do interpolation between received states
/// </summary>
[NetworkSettings(channel = 1, sendInterval = 0.1f)]
public class PlayerNetworkSync : NetworkBehaviour
{

    PlayerNetworkInterpolation networkInterpolation;     //The interpolation component

    [SyncVar]
    private CharacterState serverLastState;                 //SERVER: Store last state

    public Transform mouseLook;

    void Start()
    {
        networkInterpolation = GetComponent<PlayerNetworkInterpolation>();
    }

    /// <summary>
    /// Server: Called when a state from client was received and update finished
    /// </summary>
    /// <param name="clientInputState"></param>
    void ServerStateReceived(int clientInputState)
    {
        CharacterState state = new CharacterState()
        {
            state = clientInputState,
            position = transform.position,
            rotationY = transform.rotation,
            rotationX = mouseLook.rotation
        };

        //Server: trigger the synchronisation due to SyncVar property
        serverLastState = state;

        //If server and client is local, bypass the sync and set state as ACKed
        if (isServer && isLocalPlayer)
        {
            SendMessage("ServerState", state, SendMessageOptions.DontRequireReceiver);
        }
    }

    /// <summary>
    /// Server: Serialize the state over network
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="initialState"></param>
    /// <returns></returns>
    public override bool OnSerialize(NetworkWriter writer, bool initialState)
    {
        writer.Write(serverLastState.state);
        writer.Write(serverLastState.position);
        writer.Write(serverLastState.rotationY);
        writer.Write(serverLastState.rotationX);

        return true;
    }

    /// <summary>
    /// All Clients: Deserialize the state from network
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="initialState"></param>
    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        CharacterState state = new CharacterState()
        {
            state = reader.ReadInt32(),
            position = reader.ReadVector3(),
            rotationY = reader.ReadQuaternion(),
            rotationX = reader.ReadQuaternion()
        };

        //Client: Received a new state for the local player, treat it as an ACK and do reconciliation
        if (isLocalPlayer)
        {
            SendMessage("ServerState", state, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            //Other Clients: Received a state, treat it like a new position snapshot from authority
            if (initialState)
            {
                //Others Clients: First state, just snap to new position
                transform.position = state.position;
                transform.rotation = state.rotationY;
                mouseLook.rotation = state.rotationX;
            }
            else if (networkInterpolation != null)
            {
                //Others Clients: Interpolate between received positions
                networkInterpolation.ReceiveState(state);
            }
        }
    }

    public struct CharacterState
    {
        public int state;
        public Vector3 position;
        public Quaternion rotationX;
        public Quaternion rotationY;
    }
}
