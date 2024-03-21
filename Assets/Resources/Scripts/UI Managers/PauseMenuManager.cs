using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class PauseMenuManager : NetworkBehaviour
{

    public bool isPaused = false;

    private GameNetworkManager networkManager;
    [SerializeField]
    private GameObject pauseMenu;
    private LoadoutMenuManager loadoutScript;
    private bool unPause;

    void Start()
    {
        isPaused = false;
        unPause = false;
        loadoutScript = GetComponent<LoadoutMenuManager>();
        networkManager = GetComponent<GameNetworkManager>();
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (Input.GetButtonDown("Cancel") || unPause)
        {
            isPaused = !isPaused;
            unPause = false;

            if (isPaused)
                pauseMenu.SetActive(true);
            else
                pauseMenu.SetActive(false);
        }
    }

    public void Resume()
    {
        unPause = true;

        Debug.Log("Resuming");
    }

    public void Options(int scene)
    {
        Application.LoadLevel(scene);
    }

    public void ExitToMenu(int scene)
    {
        Application.LoadLevel(scene);

        Debug.Log("Exiting to Menus...");

        MatchInfo matchInfo = networkManager.matchInfo;
        networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, networkManager.OnDropConnection);
        networkManager.StopHost();
    }

    public void Quit()
    {
        Application.Quit();

        Debug.Log("Exiting...");
    }
}