using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoadoutMenuManager : NetworkBehaviour
{

    public bool isSelecting = false;
    private bool unSelect;
    public GameObject loadoutMenu;
    private PauseMenuManager pauseScript;

    public Button LMGButton;
    public Button RifleButton;

    private void Start()
    {
        isSelecting = false;
        unSelect = false;
        pauseScript = GetComponent<PauseMenuManager>();
    }

    public void Update()
    {
        if (!isLocalPlayer)
            return;

        if ((Input.GetButtonDown("Buy") && !pauseScript.isPaused) || (isSelecting && Input.GetButtonDown("Cancel")))
        {
            isSelecting = !isSelecting;

            if (isSelecting == true)
            {
                pauseScript.enabled = false;
                loadoutMenu.SetActive(true);
            }
            else
            {
                pauseScript.enabled = true;
                loadoutMenu.SetActive(false);
            }
        }
    }
}
