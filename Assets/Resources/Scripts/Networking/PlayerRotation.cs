using UnityEngine;

public class PlayerRotation : MonoBehaviour
{

    private PlayerInput input;

    [SerializeField]
    private Transform player;
    [SerializeField]
    private Transform mouseLook;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
    }

    public void RunUpdate(float delta)
    {
        player.rotation = input.currentInput.rotationY;
        mouseLook.rotation = input.currentInput.rotationX;
    }
}
