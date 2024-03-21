using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimControllerLMG : MonoBehaviour {

    public Animation LMGAnim;
    public PlayerInput input;

    void Update()
    {
        if (input.currentInput.movementInput.magnitude != 0)
        {
            if (input.currentInput.inputRun == true)
            {
                LMGAnim.Play("LMGRun");
            }
            else
            {
                LMGAnim.Play("LMGWalk");
            }
        }

        if (input.currentInput.inputReload == true)
        {
            LMGAnim.Play("LMGReload");
        }
    }
}
