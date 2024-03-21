using UnityEngine;
using UnityEngine.Networking;

public class PlayerInput : NetworkBehaviour
{
    public float mouseSensitivity = 2.5f;
    public InputState currentInput;
    public Transform playerCharacter;
    public Transform playerMouseLook;

    void Update()
    {
        //currentInput.setMovementInput(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")));
        //currentInput.setMouseInput(new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")));

        //currentInput.inputJump = Input.GetButtonDown("Jump");
        //currentInput.inputFireTap = Input.GetButtonDown("Fire1");
        //currentInput.inputFireHold = Input.GetButton("Fire1");
        //currentInput.inputAim = Input.GetButton("Fire2");
        //currentInput.inputRun = Input.GetButton("Run");
    }

    public void Parse(int inputState)
    {
        if (isLocalPlayer)
        {
            currentInput.inputState = inputState;

            if (Cursor.lockState != CursorLockMode.Locked)
            {
                currentInput.movementInput = Vector3.zero;
                currentInput.inputJump = false;
                currentInput.inputFireTap = false;
                currentInput.inputFireHold = false;
                currentInput.inputAim = false;
                currentInput.inputRun = false;
                currentInput.inputReload = false;
                currentInput.inputGrenade = false;
            }
            else
            {
                currentInput.movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
                currentInput.rotationX = playerMouseLook.rotation;
                currentInput.rotationY = playerCharacter.rotation;

                currentInput.inputJump = Input.GetButtonDown("Jump");
                currentInput.inputFireTap = Input.GetButtonDown("Fire1");
                currentInput.inputFireHold = Input.GetButton("Fire1");
                currentInput.inputAim = Input.GetButton("Fire2");
                currentInput.inputRun = Input.GetButton("Run");
                currentInput.inputReload = Input.GetButtonDown("Reload");
                currentInput.inputGrenade = Input.GetButtonDown("Grenade");
            }
        }
    }

    [System.Serializable]
    public struct InputState
    {
        public int inputState;

        public Vector3 movementInput;
        public Quaternion rotationX;
        public Quaternion rotationY;
        public bool inputJump;
        public bool inputFireTap;
        public bool inputFireHold;
        public bool inputAim;
        public bool inputRun;
        public bool inputReload;
        public bool inputGrenade;
    }
}
