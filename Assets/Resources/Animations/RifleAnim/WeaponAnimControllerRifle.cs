using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimControllerRifle : MonoBehaviour {

    public Animation RifleAnim;
    public PlayerInput input;

    void Update()
    {
        if (input.currentInput.movementInput.magnitude != 0)
        {
            if (input.currentInput.inputRun == true)
            {
                RifleAnim.Play("RifleRun");
            }
            else
            {
                RifleAnim.Play("RifleWalk");
            }
        }

        if (input.currentInput.inputReload == true)
        {
            RifleAnim.Play("RifleReload");
        }
    }
}
