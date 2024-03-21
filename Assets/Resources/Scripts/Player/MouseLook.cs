using System;
using UnityEngine;

[AddComponentMenu("Camera-Control/Mouse Look")]
[Serializable]
public class MouseLook : MonoBehaviour
{
    public float sensitivityX = 2f;
    public float sensitivityY = 2f;
    public float minimumX = -89f;
    public float maximumX = 89f;

    public Transform playerCharacter;
    public Transform playerMouseLook;

    private Quaternion playerTargetRot;
    private Quaternion cameraTargetRot;

    void Start()
    {
        playerTargetRot = playerCharacter.localRotation;
        cameraTargetRot = playerMouseLook.localRotation;
    }

    //void Update()
    //{
    //    //if (Cursor.lockState == CursorLockMode.Locked)
    //    //{
    //    float rotY = Input.GetAxisRaw("Mouse X") * sensitivityX;
    //    float rotX = Input.GetAxisRaw("Mouse Y") * sensitivityY;

    //    // Apply rotation
    //    playerTargetRot *= Quaternion.Euler(0f, rotY, 0f);
    //    cameraTargetRot *= Quaternion.Euler(-rotX, 0f, 0f);

    //    cameraTargetRot = ClampRotationAroundXAxis(cameraTargetRot);

    //    playerCharacter.rotation = playerTargetRot;
    //    playerMouseLook.localRotation = cameraTargetRot;
    //    //}
    //}

    public void RunUpdate(float delta)
    {
        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        float rotY = Input.GetAxisRaw("Mouse X") * sensitivityX;
        float rotX = Input.GetAxisRaw("Mouse Y") * sensitivityY;


        playerTargetRot *= Quaternion.Euler(0f, rotY, 0f);
        cameraTargetRot *= Quaternion.Euler(-rotX, 0f, 0f);

        cameraTargetRot = ClampRotationAroundXAxis(cameraTargetRot);

        playerCharacter.rotation = playerTargetRot;
        playerMouseLook.localRotation = cameraTargetRot;
    }


    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, minimumX, maximumX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }
}
