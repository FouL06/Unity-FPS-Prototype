using System.Collections;
using UnityEngine;

public class Target : MonoBehaviour
{
    private int count;
    private bool started;
    private bool flashing;
    private float startTime;
    private float lastChange;

    void Start()
    {
        count = 0;
        started = false;
    }

    void FixedUpdate()
    {
        if (!started)
        {
            startTime = Time.realtimeSinceStartup;
        }
        else
        {

            if (Time.realtimeSinceStartup - lastChange >= 1)
            {
                transform.localPosition = new Vector3(Random.Range(-0.9f, 0.9f), Random.Range(1.0f, 12.0f), 77);
                lastChange = Time.realtimeSinceStartup;
            }

            if (Time.realtimeSinceStartup - startTime >= 30)
            {
                started = false;
                StartCoroutine("Flash");
            }
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(0, Screen.height - 20, 60, 60), " Score: " + count);
    }

    IEnumerator Flash()
    {
        flashing = true;
        transform.localPosition = new Vector3(0, 6.5f, 77);
        yield return new WaitForSeconds(1);
        transform.localPosition = new Vector3(0, 6.5f, 80);
        yield return new WaitForSeconds(1);
        transform.localPosition = new Vector3(0, 6.5f, 77);
        yield return new WaitForSeconds(1);
        transform.localPosition = new Vector3(0, 6.5f, 80);
        yield return new WaitForSeconds(1);
        transform.localPosition = new Vector3(0, 6.5f, 77);
        flashing = false;

    }

    public void TakeDamage(int damageAmount)
    {
        if (!flashing)
        {
            if (!started)
            {
                count = 0;
            }
            started = true;
            count++;
            transform.localPosition = new Vector3(Random.Range(-0.9f, 0.9f), Random.Range(1.0f, 12.0f), 77);
            lastChange = Time.realtimeSinceStartup;
        }
    }
}
