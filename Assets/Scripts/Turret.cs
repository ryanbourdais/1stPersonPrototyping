using UnityEngine;

public class Turret : MonoBehaviour
{

    public Camera turretCam;
    float rotationX = 0;
    public float lookSpeed = 2.0f;
    public float turnSpeed = 6.0f;
    public GameObject gun;
    public float bottomLookXLimit = 90.0f;
    public float topLookXLimit = 90.0f;
    private Vector3 currentRotation;
    private Vector3 smoothVelocity = Vector3.zero;
    public float smoothTime = 0.2f;
    public Transform turret;
    float initialRotationX;

    void Start() {
        initialRotationX = gun.transform.rotation.x;
    }

    void Update()
    {
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        // gun.transform.rotation = Quaternion.Euler(rotationX, 0, 0); 
    }
}
