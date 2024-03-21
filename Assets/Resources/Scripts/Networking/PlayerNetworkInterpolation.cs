﻿using UnityEngine;

public class PlayerNetworkInterpolation : MonoBehaviour
{

    //keep at least 20 buffered state to interpolate from
    int bufferedStatesCount = 0;
    PlayerNetworkSync.CharacterState[] bufferedStates = new PlayerNetworkSync.CharacterState[20];
    float lastBufferedStateTime = 0;

    //Add an extra lag of 200 ms (4 commands back in time)
    //This way we always have a good chance to have received one of these 4 commands
    //TODO make this based on ping
    [SerializeField]
    private float interpolationBackTime = 0.4f;
    [SerializeField]
    private float updateRate = 0.1f;

    public Transform mouseLook;

    /// <summary>
    /// Called when a state is received from server
    /// </summary>
    /// <param name="newState"></param>
    public void ReceiveState(PlayerNetworkSync.CharacterState newState)
    {
        //Other Clients: Shift buffer and store at first position
        for (int i = bufferedStates.Length - 1; i >= 1; i--)
        {
            bufferedStates[i] = bufferedStates[i - 1];
        }
        bufferedStates[0] = newState;
        bufferedStatesCount = Mathf.Min(bufferedStatesCount + 1, bufferedStates.Length);

        //Other Clients: Check that states are in good order
        for (int i = 0; i < bufferedStatesCount - 1; i++)
        {
            if (bufferedStates[i].state < bufferedStates[i + 1].state)
            {
                Debug.LogWarning("Warning, State are in wrong order");

            }
        }

        lastBufferedStateTime = Time.time;
    }

    // We interpolate only on other clients, not on the server, and not on the local client)
    void Update()
    {

        //Loop all states
        for (int i = 0; i < bufferedStatesCount; i++)
        {
            //State time is local time - state counter * interval between states
            float stateTime = lastBufferedStateTime - i * updateRate;
            //Find the first state that match now - interpTime (or take the last buffer entry)
            if (stateTime <= Time.time - interpolationBackTime || i == bufferedStatesCount - 1)
            {
                //Get one step after and one before the time
                PlayerNetworkSync.CharacterState afterState = bufferedStates[Mathf.Max(i - 1, 0)];
                float afterStateTime = lastBufferedStateTime - (i - 1) * updateRate;
                PlayerNetworkSync.CharacterState beforeState = bufferedStates[i];
                float beforeStateTime = lastBufferedStateTime - i * updateRate;
                ;

                // Use the time between the two slots to determine if interpolation is necessary
                double length = afterStateTime - beforeStateTime;
                float t = 0.0F;
                // As the time difference gets closer to 100 ms t gets closer to 1 in
                // which case rhs is only used
                if (length > 0.0001)
                {
                    t = (float)((Time.time - interpolationBackTime - beforeStateTime) / length);
                }

                //Do the actual interpolation
                transform.position = Vector3.Lerp(beforeState.position, afterState.position, t);
                transform.rotation = Quaternion.Slerp(beforeState.rotationY, afterState.rotationY, t);
                mouseLook.rotation = Quaternion.Slerp(beforeState.rotationX, afterState.rotationX, t);
                break;
            }
        }
    }

}
