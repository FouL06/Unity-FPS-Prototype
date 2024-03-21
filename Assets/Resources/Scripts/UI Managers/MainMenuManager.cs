using UnityEngine;

public class MainMenuManager : MonoBehaviour
{

    void Start()
    {
        Screen.SetResolution(800, 600, false);
    }

    public void quitGame()
    {
        Application.Quit();

        Debug.Log("Exiting Game...");
    }

    public void multiplayer(int scene)
    {
        Application.LoadLevel(scene);

        Debug.Log("Loading Level");
    }

    public void options(int scene)
    {
        Application.LoadLevel(scene);
    }

    public void back(int scene)
    {
        Application.LoadLevel(scene);
    }
}
