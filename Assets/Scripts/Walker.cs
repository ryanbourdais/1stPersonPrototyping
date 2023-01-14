using UnityEngine;

public class Walker : MonoBehaviour
{
    
    public float playerDistance = 1f;
    public GameObject car;
    public GameObject player;
    public GameObject turret;
    public Camera playerCam;
    public Camera turretCam;
    public GameObject seatPos;
    public GameObject exitPos;
    private AudioListener tankListener;
    private AudioListener playerListener;
    public GameObject gun;
    public WalkerCannon cannon;
    public Canvas mainHUD;
    public float driveSpeed = 30f;
    public float turnSpeed = 1f;
    Vector3 moveDirection = Vector3.zero;
    private bool playerInCar;
    QuakeController playerCon;
    Gun playerWeapon;
    WalkerTurret walkerTurret;
    public WalkerMachineGun walkerMachineGun;
    public CharacterController carController;
    public float lookSpeed = 2.0f;
    float carRotationX = 0;
    public float timeElapsed = 0f;
    void Start() 
    {
        tankListener = turretCam.GetComponent<AudioListener>();
        playerListener = playerCam.GetComponent<AudioListener>();
        playerCon = player.GetComponent<QuakeController>();
        playerWeapon = gun.GetComponent<Gun>();
        walkerTurret = turret.GetComponent<WalkerTurret>();
    }


    void Update()
    {
        float distance = Vector3.Distance(car.transform.position, player.transform.position);
        if(distance < playerDistance && Input.GetKeyDown(KeyCode.E) && !playerInCar)
        {
            walkerTurret.enabled = true;
            player.transform.position = seatPos.transform.position;
            playerInCar = true;
            cannon.enabled = true;
            // mainHUD.enabled = false;
            walkerMachineGun.enabled = true;
            tankListener.enabled = true;
            playerWeapon.enabled = false;
            playerCon.enabled = false;
            turretCam.enabled = true;
            playerCam.enabled = false;
            playerListener.enabled = false;
            player.transform.parent = car.transform;
            carController.enabled = true;
        }
        if(playerInCar)
        {
            timeElapsed += timeElapsed + Time.deltaTime;
            Vector3 forward = transform.TransformDirection(Vector3.forward);

            float curSpeedX = (driveSpeed) * Input.GetAxis("Vertical");
            moveDirection = (forward * curSpeedX);
            carController.Move(moveDirection * Time.deltaTime);

            carRotationX += Input.GetAxis("Horizontal") * turnSpeed;
            carController.transform.localRotation = Quaternion.Euler(0, carRotationX, 0);

            if(Input.GetKeyDown(KeyCode.E) && timeElapsed >= 1)
            {
                walkerTurret.enabled = false;
                player.transform.position = exitPos.transform.position;
                playerInCar = false;
                player.transform.parent = null;
                cannon.enabled = false;
                mainHUD.enabled = true;
                walkerMachineGun.enabled = false;
                tankListener.enabled = false;
                playerWeapon.enabled = true;
                playerCon.enabled = true;
                turretCam.enabled = false;
                playerCam.enabled = true;
                playerListener.enabled = true;
                carController.enabled = false;
                timeElapsed = 0;
            }
        }
    }
}
