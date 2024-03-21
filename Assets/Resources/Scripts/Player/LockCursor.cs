using UnityEngine;
using UnityEngine.Networking;

public class LockCursor : NetworkBehaviour
{
    private LoadoutMenuManager loadoutScript;
    private PauseMenuManager pauseScript;


    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        loadoutScript = GetComponent<LoadoutMenuManager>();
        pauseScript = GetComponent<PauseMenuManager>();
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (pauseScript.isPaused || loadoutScript.isSelecting)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (!pauseScript.isPaused && !loadoutScript.isSelecting)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
