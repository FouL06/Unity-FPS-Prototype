using UnityEngine;

public class ScoreBoardManager : MonoBehaviour
{

    public bool isShowingScore = false;
    private GameNetworkManager networkManager;

    [SerializeField]
    private GameObject scoreBoard;
    private bool unScoreBoard;
    private KeyCode scoreKey = KeyCode.Tab;

    void Start()
    {
        isShowingScore = false;
        networkManager = GetComponent<GameNetworkManager>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if ((Input.GetKeyDown(scoreKey) || unScoreBoard))
        {
            isShowingScore = !isShowingScore;
            unScoreBoard = false;

            if (isShowingScore == true)
            {
                scoreBoard.SetActive(true);
            }
        }
        else
        {
            if (isShowingScore == false)
            {
                scoreBoard.SetActive(false);
            }
        }
    }
}
