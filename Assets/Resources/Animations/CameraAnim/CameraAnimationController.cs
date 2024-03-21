using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimationController : MonoBehaviour {

    public Animation headBob;
    public PlayerInput input;

    void Update ()
    {
        if (input.currentInput.movementInput.magnitude != 0)
        {
            if (input.currentInput.inputRun == true)
            {
                headBob.Play("RunAnim");
            }
            else
            {
                headBob.Play("WalkAnim");
            }
        }
    }
}
