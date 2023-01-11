using UnityEngine;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public float impactForce = 30f;
    public float roundsPerMinute = 15f;
    private float fireRate;
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
    public float movementMod;

    private float nextTimeToFire = 0f;
    private float count = 0f;
    public float blastRadius = 5f;
    private Collider[] hitTargets = new Collider[0];
    public ParticleSystem muzzleFlashSecondary;
    public GameObject explosionEffect;
    public float explosionDamage;
    public float explosionForce;


    void Start() 
    {
        accuracy = readyAccuracy;
        fireRate = roundsPerMinute * 0.017f;
    }

    void Update()
    {
        
        float velocity = ((player.transform.position - previousPos).magnitude) / Time.deltaTime;
        previousPos = player.transform.position;

        float accuracyMod = velocity / (movementMod * 5);

        if(Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            count++;
            accuracy = autoAccuracy - accuracyMod;
            nextTimeToFire = Time.time + 1f/fireRate;
            Shoot();
        }
        if(Input.GetButton("Fire1") && Time.time >= nextTimeToFire + readyTime)
        {
            accuracy = readyAccuracy - accuracyMod;
            Shoot();
        }
        if(Input.GetButton("Fire2"))
        {
            ShootSecondary();
        }
    }

    void Shoot()
    {
        muzzleFlash.Play();

        RaycastHit hit;

        Vector3 shotDirection = new Vector3(fpsCam.transform.forward.x + Random.Range(accuracy - 1, Mathf.Abs(accuracy - 1)), fpsCam.transform.forward.y + Random.Range(accuracy - 1, Mathf.Abs(accuracy - 1)), fpsCam.transform.forward.z + Random.Range(accuracy - 1, Mathf.Abs(accuracy - 1)));
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

    void ShootSecondary()
    {
        muzzleFlashSecondary.Play();

        RaycastHit hit;

        Vector3 shotDirection = new Vector3(fpsCam.transform.forward.x + Random.Range(accuracy - 1, Mathf.Abs(accuracy - 1)), fpsCam.transform.forward.y + Random.Range(accuracy - 1, Mathf.Abs(accuracy - 1)), fpsCam.transform.forward.z + Random.Range(accuracy - 1, Mathf.Abs(accuracy - 1)));
        if (Physics.Raycast(fpsCam.transform.position, shotDirection, out hit, range))
        {
            hitTargets = Physics.OverlapSphere(hit.point, blastRadius);
            foreach(var obj in hitTargets)
            {
                Target target = obj.transform.GetComponent<Target>();                
                if(target != null)
                {
                    float distance = Vector3.Distance(hit.point, target.transform.position);
                    target.TakeDamage(explosionDamage / distance);
                    obj.GetComponentInParent<Rigidbody>().AddForce((obj.transform.position - hit.point).normalized * explosionForce / (distance * 2));
                }
            }

            GameObject impactGO = Instantiate(explosionEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 2);
        }
    }
}
