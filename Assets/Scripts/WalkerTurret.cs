using UnityEngine;

public class WalkerTurret : MonoBehaviour
{

    public Camera turretCam;
    public GameObject walker;
    public float lookSpeed = 2.0f;
    public float turnSpeed = 6.0f;
    public float lookXLimit = 90.0f;
    public float lookYLimit = 90.0f;
    private Vector3 currentRotation;
    float rotationX;
    float rotationY;
    public float cameraSmoothness = 5f;

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
    }
}

