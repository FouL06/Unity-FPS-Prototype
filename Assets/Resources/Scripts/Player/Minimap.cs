using UnityEngine;

public class Minimap : MonoBehaviour
{

    public float minimapSize = 1.25f;
    private float offsetX = 10.0f;
    private float offsetY = 10.0f;
    private float adjustSize;
    public float constantHeight = 50.0f;
    private Camera minimapCamera;
    public Camera cmra;
    private Rect baseRect;
    private Rect adjustedRect;
    //public Texture borderTexture;

    void Start()
    {
        minimapCamera = GetComponent<Camera>();
        correctMinimapViewport();
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, constantHeight, transform.position.z);
        adjustSize = Mathf.RoundToInt(Screen.width / 10);
        minimapCamera.pixelRect = new Rect(offsetX, (Screen.height - (minimapSize * adjustSize)) - offsetY, minimapSize * adjustSize, minimapSize * adjustSize);
    }
    /*
    void OnGUI()
    {
        minimapCamera.Render();
        GUI.DrawTexture(new Rect(offsetX, offsetY, minimapSize * adjustSize, minimapSize * adjustSize), borderTexture);
    }
    */

    void correctMinimapViewport()
    {
        baseRect = minimapCamera.rect;
        var correctionFactor = 1.77778f / cmra.aspect;

        adjustedRect = new Rect(baseRect.x - ((baseRect.width * correctionFactor) - baseRect.width), baseRect.y, baseRect.width * correctionFactor, baseRect.height);
        minimapCamera.rect = adjustedRect;
    }

}
