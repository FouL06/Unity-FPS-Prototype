using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class JoinGame : MonoBehaviour
{

    List<GameObject> roomList = new List<GameObject>();

    private GameNetworkManager networkManager;

    public Text status;

    public GameObject serverListItemPrefab;
    public Transform serverListParent;

    void Start()
    {
        networkManager = GetComponent<GameNetworkManager>();

        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }


        Refresh();
    }

    public void Refresh()
    {
        ClearServerList();
        networkManager = GetComponent<GameNetworkManager>();

        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }

        networkManager.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);
        status.text = "Loading...";
    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        status.text = "";

        if (!success || matchList == null)
        {
            status.text = "Couldnt Get Server List...";
            return;
        }

        foreach (MatchInfoSnapshot match in matchList)
        {
            GameObject serverListItem = Instantiate(serverListItemPrefab);
            serverListItem.transform.SetParent(serverListParent);

            ServerListItem _ServerListItem = serverListItem.GetComponent<ServerListItem>();
            if (_ServerListItem != null)
            {
                _ServerListItem.Setup(match, JoinServer);
            }

            //callback to tell us if we join the right room

            roomList.Add(serverListItem);
        }

        if (roomList.Count == 0)
        {
            status.text = "No servers up...";
        }
    }

    public void ClearServerList()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            Destroy(roomList[i]);
        }

        roomList.Clear();
    }

    public void JoinServer(MatchInfoSnapshot _match)
    {
        networkManager.matchMaker.JoinMatch(_match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
        StartCoroutine(WaitForJoin());
    }

    IEnumerator WaitForJoin()
    {
        ClearServerList();

        int countDown = 10;
        while (countDown > 0)
        {
            status.text = "Joining... (" + countDown + ")";

            yield return new WaitForSeconds(1);

            countDown--;
        }

        //Failed to connect
        status.text = "Failed to connect.";
        yield return new WaitForSeconds(1);

        MatchInfo matchInfo = networkManager.matchInfo;
        if (matchInfo != null)
        {
            networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, networkManager.OnDropConnection);
            networkManager.StopHost();
        }

        Refresh();
    }
}