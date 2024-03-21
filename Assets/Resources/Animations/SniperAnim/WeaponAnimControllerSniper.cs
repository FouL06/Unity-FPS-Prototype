using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimControllerSniper : MonoBehaviour {

    public Animation SniperAnim;
    public PlayerInput input;

    void Update()
    {
        if (input.currentInput.movementInput.magnitude != 0)
        {
            if (input.currentInput.inputRun == true)
            {
                SniperAnim.Play("SniperRun");
            }
            else
            {
                SniperAnim.Play("SniperWalk");
            }
        }

        if (input.currentInput.inputReload == true)
        {
            SniperAnim.Play("SniperReload");
        }
    }
}
