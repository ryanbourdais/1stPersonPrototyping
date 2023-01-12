using UnityEngine;

public class Turret : MonoBehaviour
{

    public Camera turretCam;
    // float rotationX = 0;
    public float lookSpeed = 2.0f;
    public float turnSpeed = 6.0f;
    public GameObject gun;
    public float bottomLookXLimit = 90.0f;
    public float lookXLimit = 90.0f;
    public float lookYLimit = 90.0f;
    private Vector3 currentRotation;
    private Vector3 smoothVelocity = Vector3.zero;
    public float smoothTime = 0.2f;
    public Transform turret;
    float initialRotationX;
    private float rotationY;
    float rotationX;
    public float cameraSmoothness = 5f;
    public Cannon[] cannons = new Cannon[0];
    private int count = 0;

    void Start() {
        initialRotationX = gun.transform.rotation.x;
    }

    void Update()
    {
        float h = -Input.GetAxis("Mouse Y") * lookSpeed;
        float v = Input.GetAxis("Mouse X") * lookSpeed;
   
        Vector3 eulers = transform.localEulerAngles;
    
        rotationX += h;
        rotationY += v;

        rotationX = Mathf.Clamp(rotationX, -lookYLimit, lookYLimit);
        rotationY = Mathf.Clamp(rotationY, -lookXLimit, lookXLimit);

        var targetRotation = Quaternion.Euler(Vector3.up * rotationY) * Quaternion.Euler(Vector3.right * rotationX);

        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, cameraSmoothness * Time.deltaTime);

        if(Input.GetButtonDown("Fire1"))
        {
            if(Time.time >= cannons[count].nextTimeToFire + cannons[count].readyTime)
            {
                cannons[count].shoot = true;
                if(count < cannons.Length - 1)
                    count++;
                else
                    count = 0; 
            }
        }
    }
}
