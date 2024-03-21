using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeExplosion : MonoBehaviour
{
    private float startTime;
    public float force = 20f;
    public float explosionTime = 2.0f;
    public float explosionRadius = 5.0f;
    public float explosionPower = 300.0f;
    public float explosionDamage = 100.0f;

    private void Start()
    {
        startTime = Time.time;
    }

    private void FixedUpdate()
    {
        if (Time.time - startTime >= explosionTime)
        {
            Explode();
        }
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        explosionTime -= Time.deltaTime;
        if(explosionTime < 0)
        {
            Exploding(collision.contacts[0].point);
        }
    }
    */

    public void Explode()
    {
        Vector3 explosionPosition = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);
        foreach (Collider hit in colliders)
        {
            if (hit.GetComponent<Rigidbody>() != null)
            {
                hit.GetComponent<Rigidbody>().isKinematic = false;
                hit.GetComponent<Rigidbody>().AddExplosionForce(explosionPower, explosionPosition, explosionRadius, 1.0f);
            }
            Destroy(gameObject);
        }
    }
}
