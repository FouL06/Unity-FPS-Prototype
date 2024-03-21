using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimControllerPistol : MonoBehaviour {

    public Animation PistolAnim;
    public PlayerInput input;

    void Update()
    {
        if (input.currentInput.movementInput.magnitude != 0)
        {
            if (input.currentInput.inputRun == true)
            {
                PistolAnim.Play("PistolRun");
            }
            else
            {
                PistolAnim.Play("PistolWalk");
            }
        }

        if (input.currentInput.inputReload == true)
        {
            PistolAnim.Play("PistolReload");
        }
    }
}
