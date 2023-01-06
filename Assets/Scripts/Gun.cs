using UnityEngine;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public float impactForce = 30f;
    public float fireRate = 15f;
    public float autoAccuracy = 0.9f;
    public float readyAccuracy = 0.98f;
    public float readyTime = 0.6f;
    private Vector3 previousPos = new Vector3(0f,0f,0f);
    private float accuracy;

    public GameObject player;
    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public bool debugMode;
    public GameObject debugHitMarker;

    private float nextTimeToFire = 0f;


    void Start() 
    {
        accuracy = readyAccuracy;
    }

    void Update()
    {
        
        float velocity = ((player.transform.position - previousPos).magnitude) / Time.deltaTime;
        previousPos = player.transform.position;

        float accuracyMod = velocity / 50; 

        if(Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            accuracy = autoAccuracy - accuracyMod;
            nextTimeToFire = Time.time + 1f/fireRate;
            Shoot();
        }
        if(Time.time >= nextTimeToFire + readyTime)
        {
            accuracy = readyAccuracy - accuracyMod;
        }
    }

    void Shoot()
    {
        muzzleFlash.Play();

        RaycastHit hit;

        Vector3 shotDirection = new Vector3(fpsCam.transform.forward.x * Random.Range(accuracy, 1), fpsCam.transform.forward.y * Random.Range(accuracy, 1), fpsCam.transform.forward.z * Random.Range(accuracy, 1));

        if (Physics.Raycast(fpsCam.transform.position, shotDirection, out hit, range))
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
            if(debugMode)
            {
                Instantiate(debugHitMarker, hit.point, Quaternion.LookRotation(hit.normal));
            }

            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 2);
        }
    }
}