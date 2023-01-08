using UnityEngine;

public class Vehicle : MonoBehaviour
{
    
    public float playerDistance = 1f;
    public GameObject car;
    public GameObject player;
    public Camera carCam;
    public Camera playerCam;
    public GameObject seatPos;
    public GameObject exitPos;
    private AudioListener carListener;
    private AudioListener playerListener;
    public GameObject gun;
    public Canvas mainHUD;
    public float driveSpeed = 30f;
    public float turnSpeed = 1f;
    Vector3 moveDirection = Vector3.zero;
    private bool playerInCar;
    QuakeController playerCon;
    Gun playerWeapon;
    public CharacterController carController;
    public float lookSpeed = 2.0f;
    float rotationX = 0;
    float carRotationX = 0;
    float rotationY = 0;
    public float timeElapsed = 0f;
    void Start() 
    {
        carListener = carCam.GetComponent<AudioListener>();
        playerListener = playerCam.GetComponent<AudioListener>();
        playerCon = player.GetComponent<QuakeController>();
        playerWeapon = gun.GetComponent<Gun>();
    }


    void Update()
    {
        float distance = Vector3.Distance(car.transform.position, player.transform.position);
        if(distance < playerDistance && Input.GetKeyDown(KeyCode.E) && !playerInCar)
        {
            player.transform.position = seatPos.transform.position;
            playerInCar = true;
            mainHUD.enabled = false;
            carCam.enabled = true;
            carListener.enabled = true;
            playerWeapon.enabled = false;
            playerCon.enabled = false;
            playerCam.enabled = false;
            playerListener.enabled = false;
            player.transform.parent = car.transform;
        }
        if(playerInCar)
        {
            timeElapsed += timeElapsed + Time.deltaTime;
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            float curSpeedX = (driveSpeed) * Input.GetAxis("Vertical");
            float curSpeedY = (driveSpeed) * Input.GetAxis("Horizontal");
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);
            carController.Move(moveDirection * Time.deltaTime);

            carRotationX += Input.GetAxis("Horizontal") * turnSpeed;
            carController.transform.localRotation = Quaternion.Euler(0, carRotationX, 0);

            rotationY += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX += -Input.GetAxis("Mouse X") * lookSpeed;
            carCam.transform.localRotation = Quaternion.Euler(rotationY, -rotationX, 0);
            if(Input.GetKeyDown(KeyCode.E) && timeElapsed >= 1)
            {
                player.transform.position = exitPos.transform.position;
                playerInCar = false;
                mainHUD.enabled = true;
                carCam.enabled = false;
                carListener.enabled = false;
                playerWeapon.enabled = true;
                playerCon.enabled = true;
                playerCam.enabled = true;
                playerListener.enabled = true;
                timeElapsed = 0;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, playerDistance);
    }
}
