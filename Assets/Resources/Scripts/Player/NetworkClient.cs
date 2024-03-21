using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

[System.Serializable]
public class ToggleEvent : UnityEvent<bool> { };

public class NetworkClient : NetworkBehaviour
{
    [SerializeField]
    ToggleEvent onToggleLocal;
    [SerializeField]
    ToggleEvent onToggleRemote;
    [SerializeField]
    ToggleEvent onToggleShared;
    [SerializeField]
    ToggleEvent onToggleServer;

    void Start()
    {
        EnablePlayer();
    }

    void DisablePlayer()
    {
        onToggleShared.Invoke(false);

        if (isLocalPlayer)
            onToggleLocal.Invoke(false);
        else
            onToggleRemote.Invoke(false);

        if (isServer)
            onToggleServer.Invoke(false);
    }

    void EnablePlayer()
    {
        onToggleShared.Invoke(true);

        if (isLocalPlayer)
            onToggleLocal.Invoke(true);
        else
            onToggleRemote.Invoke(true);

        if (isServer)
            onToggleServer.Invoke(true);
    }
}