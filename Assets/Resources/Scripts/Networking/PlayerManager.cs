﻿using System;
using UnityEngine.Networking;

[NetworkSettings(channel = 0, sendInterval = 2f)]
public class PlayerManager : NetworkBehaviour
{
    private static PlayerManager instance;
    public static PlayerManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerManager>();
            }
            return instance;
        }
    }

    public RemotePlayer GetLocalPlayer()
    {
        foreach (RemotePlayer player in FindObjectsOfType<RemotePlayer>())
        {
            if (player.isLocalPlayer)
            {
                return player;
            }
        }

        throw new Exception("Can't find local player");
    }
}
