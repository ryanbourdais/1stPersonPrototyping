using UnityEngine;

public class QuakeController : MonoBehaviour
{
	public CharacterController controller;
	public Transform GroundCheck;
    public Transform player;
    public Camera playerCamera;
    public LayerMask groundLayer;

	private float wishspeed2;
	private float gravity = 20f;
	float wishspeed;

	public float GroundDistance = 0.4f;
	public float moveSpeed = 7.0f;  // Ground move speed
	public float runSpeed = 14.0f;
	public float runAcceleration = 14f;   // Ground accel
	public float runDeacceleration = 10f;   // Deacceleration that occurs when running on the ground
	public float airAcceleration = 2.0f;  // Air accel
	public float airDeacceleration = 2.0f;    // Deacceleration experienced when opposite strafing
	public float airControl = 0.3f;  // How precise air control is
	public float strafeAcceleration = 50f;   // How fast acceleration occurs to get up to strafeSpeed when side strafing
	public float strafeSpeed = 1f;    // What the max speed to generate when side strafing
	public float jumpSpeed = 8.0f;
	public float friction = 6f;
	private float playerTopVelocity = 0;
	public float playerFriction = 0f;

	private float addspeed;
	private float accelspeed;
	private float currentspeed;
	private float zspeed;
	private float speed;
	private float dot;
	private float k;
	private float accel;
	private float newspeed;
	private float control;
	private float drop;

	private bool JumpQueue = false;
	private bool wishJump = false;

	private Vector3 moveDirection;
	private Vector3 moveDirectionNorm;
	private Vector3 playerVelocity;
	Vector3 wishdir;
	Vector3 vec;

	private bool IsGrounded;
	
	Vector3 udp;
    float rotationX = 0;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 90.0f;
	public bool sprint = false;
	public float sprintTime = 0f;

    void Start()
    {
        //Cursor Lock
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
	{
		if(Input.GetKeyDown(KeyCode.LeftShift) && !sprint)
        {
			sprint = true;
			sprintTime = 5.0f;
        }
		if (sprintTime > 0)
        {
            sprintTime -= Time.deltaTime;
        }
		if(controller.isGrounded && sprintTime <= 0)
		{
			sprint = false;
			wishJump = false;
		}

        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);

		IsGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, groundLayer);
		QueueJump();
		/* Movement, here's the important part */
		if(sprint)
		{
			
			if (controller.isGrounded)
				groundMove();
			else if (!controller.isGrounded)
				AirMove();
		}
		else
			standardMove();

		// Move the controller
		controller.Move(playerVelocity * Time.deltaTime);

		// Calculate top velocity
		udp = playerVelocity;
		udp.y = 0;
		if (udp.magnitude > playerTopVelocity)
			playerTopVelocity = udp.magnitude;
	}
    
	//Queues the next jump
	void QueueJump()
	{
		if (Input.GetButtonDown("Jump") && IsGrounded)
		{
			wishJump = true;
		}

		if (!IsGrounded && Input.GetButtonDown("Jump"))
		{
			JumpQueue = true;
		}
		if (IsGrounded && JumpQueue)
		{
			wishJump = true;
			JumpQueue = false;
		}
	}

	//Calculates wish acceleration
	public void Accelerate(Vector3 wishdir, float wishspeed, float accel)
	{
		currentspeed = Vector3.Dot(playerVelocity, wishdir);
		addspeed = wishspeed - currentspeed;
		if (addspeed <= 0)
			return;
		accelspeed = accel * Time.deltaTime * wishspeed;
		if (accelspeed > addspeed)
			accelspeed = addspeed;

		playerVelocity.x += accelspeed * wishdir.x;
		playerVelocity.z += accelspeed * wishdir.z;
	}

	//Execs when the player is in the air
	public void AirMove()
	{
		// SetMovementDir();

		wishdir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		wishdir = transform.TransformDirection(wishdir);

		wishspeed = wishdir.magnitude;

		wishspeed *= 7f;

		wishdir.Normalize();
		moveDirectionNorm = wishdir;

		// Aircontrol
		wishspeed2 = wishspeed;
		if (Vector3.Dot(playerVelocity, wishdir) < 0)
			accel = airDeacceleration;
		else
			accel = airAcceleration;

		// If the player is ONLY strafing left or right
		if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") != 0)
		{
			if (wishspeed > strafeSpeed)
				wishspeed = strafeSpeed;
			accel = strafeAcceleration;
		}

		Accelerate(wishdir, wishspeed, accel);

		AirControl(wishdir, wishspeed2);

		// !Aircontrol

		// Apply gravity
		playerVelocity.y -= gravity * Time.deltaTime;

		/**
			* Air control occurs when the player is in the air, it allows
			* players to move side to side much faster rather than being
			* 'sluggish' when it comes to cornering.
			*/

		void AirControl(Vector3 wishdir, float wishspeed)
		{
			// Can't control movement if not moving forward or backward
			if (Input.GetAxis("Horizontal") == 0 || wishspeed == 0)
				return;

			zspeed = playerVelocity.y;
			playerVelocity.y = 0;
			/* Next two lines are equivalent to idTech's VectorNormalize() */
			speed = playerVelocity.magnitude;
			playerVelocity.Normalize();

			dot = Vector3.Dot(playerVelocity, wishdir);
			k = 32;
			k *= airControl * dot * dot * Time.deltaTime;

			// Change direction while slowing down
			if (dot > 0)
			{
				playerVelocity.x = playerVelocity.x * speed + wishdir.x * k;
				playerVelocity.y = playerVelocity.y * speed + wishdir.y * k;
				playerVelocity.z = playerVelocity.z * speed + wishdir.z * k;

				playerVelocity.Normalize();
				moveDirectionNorm = playerVelocity;
			}

			playerVelocity.x *= speed;
			playerVelocity.y = zspeed; // Note this line
			playerVelocity.z *= speed;

		}
	}
	/**
		* Called every frame when the engine detects that the player is on the ground
		*/
	public void groundMove()
	{
		// Do not apply friction if the player is queueing up the next jump
		if (!wishJump)
			ApplyFriction(1.0f);
		else
			ApplyFriction(0);

		// SetMovementDir();

		wishdir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		wishdir = transform.TransformDirection(wishdir);
		wishdir.Normalize();
		moveDirectionNorm = wishdir;

		wishspeed = wishdir.magnitude;
		wishspeed *= runSpeed;

		Accelerate(wishdir, wishspeed, runAcceleration);

		// Reset the gravity velocity
		playerVelocity.y = 0;

		if (wishJump)
		{
			playerVelocity.y = jumpSpeed;
			wishJump = false;
		}

		/**
			* Applies friction to the player, called in both the air and on the ground
			*/
		void ApplyFriction(float t)
		{
			vec = playerVelocity; // Equivalent to: VectorCopy();
			vec.y = 0f;
			speed = vec.magnitude;
			drop = 0f;

			/* Only if the player is on the ground then apply friction */
			if (controller.isGrounded)
			{
				control = speed < runDeacceleration ? runDeacceleration : speed;
				drop = control * friction * Time.deltaTime * t;
			}   

			newspeed = speed - drop;
			playerFriction = newspeed;
			if (newspeed < 0)
				newspeed = 0;
			if (speed > 0)
				newspeed /= speed;

			playerVelocity.x *= newspeed;
			playerVelocity.z *= newspeed;
		}
	}
	void standardMove()
	{
		{
			playerFriction = 0;
			playerVelocity.x = 0;
			playerVelocity.z = 0;

			Vector3 forward = transform.TransformDirection(Vector3.forward);
        	Vector3 right = transform.TransformDirection(Vector3.right);

        	float curSpeedX = (moveSpeed) * Input.GetAxis("Vertical");
        	float curSpeedY = (moveSpeed) * Input.GetAxis("Horizontal");
        	float movementDirectionY = moveDirection.y;
        	moveDirection = (forward * curSpeedX) + (right * curSpeedY);

			if (wishJump)
			{
				playerVelocity.y = jumpSpeed;
				wishJump = false;
			}
			if (!IsGrounded)
        	{
            	playerVelocity.y -= gravity * Time.deltaTime;
        	}
        	controller.Move(moveDirection * Time.deltaTime);
		}
	}
}
