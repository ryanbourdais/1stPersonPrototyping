using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerMachineGun : MonoBehaviour
{


    public GameObject firePoint;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public float accuracy = 0.98f;
    public int range = 1000;
    public float damage = 10f;
    public float impactForce = 50f;
    public float roundsPerMinute = 2000f;
    private float fireRate;
    private float nextTimeToFire = 0f;

    void Start() 
    {
        fireRate = roundsPerMinute * 0.017f;
    }
    void Update()
    {
        if(Input.GetButton("Fire2") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f/fireRate;
            Shoot();
        }
    }


    void Shoot()
        {
            muzzleFlash.Play();

            RaycastHit hit;

            Vector3 shotDirection = new Vector3(firePoint.transform.forward.x + Random.Range(accuracy - 1, Mathf.Abs(accuracy - 1)), firePoint.transform.forward.y + Random.Range(accuracy - 1, Mathf.Abs(accuracy - 1)), firePoint.transform.forward.z + Random.Range(accuracy - 1, Mathf.Abs(accuracy - 1)));
            if (Physics.Raycast(firePoint.transform.position, shotDirection, out hit, range))
            {
                Target target = hit.transform.GetComponent<Target>();

                if(target != null)
                {
                    target.TakeDamage(damage);
                }

                if(hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(-hit.normal * impactForce);
                }

                GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 2);
            }
        }
}


