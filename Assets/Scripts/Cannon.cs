using UnityEngine;

public class Cannon : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public float impactForce = 30f;
    public float roundsPerMinute = 15f;
    private float fireRate;
    public float readyTime = 0.6f;
    private Vector3 previousPos = new Vector3(0f,0f,0f);
    public float accuracy = 1.0f;
    public Camera Camera;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public GameObject firePoint;
    public bool debugMode;
    public GameObject debugHitMarker;
    public float movementMod;

    public float nextTimeToFire = 0f;
    public float blastRadius = 5f;
    private Collider[] hitTargets = new Collider[0];

    public bool shoot = false;


    void Start() 
    {
        fireRate = roundsPerMinute * 0.017f;
    }

    void Update()
    {
        if(Time.time >= nextTimeToFire + readyTime && shoot)
        {
            nextTimeToFire = Time.time + 1f/fireRate;
            Shoot();
        }
    }

    public void Shoot()
    {
        muzzleFlash.Play();

        RaycastHit hit;

        Vector3 shotDirection = new Vector3(firePoint.transform.forward.x + Random.Range(accuracy - 1, Mathf.Abs(accuracy - 1)), firePoint.transform.forward.y + Random.Range(accuracy - 1, Mathf.Abs(accuracy - 1)), firePoint.transform.forward.z + Random.Range(accuracy - 1, Mathf.Abs(accuracy - 1)));

        if (Physics.Raycast(firePoint.transform.position, shotDirection, out hit, range))
        {
            hitTargets = Physics.OverlapSphere(hit.point, blastRadius);
            foreach(var obj in hitTargets)
            {
                Target target = obj.transform.GetComponent<Target>();                
                if(target != null)
                {
                    float distance = Vector3.Distance(hit.point, target.transform.position);
                    target.TakeDamage(damage / distance);
                    obj.GetComponentInParent<Rigidbody>().AddForce((obj.transform.position - hit.point).normalized * impactForce / distance);
                }

                if(hit.rigidbody != null)
                {

                    hit.rigidbody.AddForce(-hit.normal * impactForce);
                }
            }

            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 2);
            shoot = false;
        }
    }
}
