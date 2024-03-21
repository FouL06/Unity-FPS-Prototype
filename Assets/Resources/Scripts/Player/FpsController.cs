using System;
using UnityEngine;

public class FpsController : MonoBehaviour
{
    public float walkSpeed = 6.0f;
    public float runSpeed = 10.0f;
    public float minSpeed = 1.2f;
    public float accelSpeed = 1.5f;
    public float slowdown = 1.0f;
    public float jumpSpeed = 8.0f;
    public float gravityMultiplier = 2.25f;
    public float directionShiftSpeed = 0.1f;
    public float fallDamageThreshold = 10.0f;
    public AudioClip[] footStep = new AudioClip[2];
    public float stepInterval;
    [Range(0f, 1f)]
    public float runstepLengthen;
    [HideInInspector]
    public float velocityMagnitude;

    private bool bumpLeft;
    private bool bumpRight;
    private bool bumpForward;
    private bool bumpBackward;
    private bool isWalking;
    private bool jump;
    private bool falling;
    private float curSpeed;
    private float fallStartLevel;
    private float curAxisY;
    private float lastAxisY;
    private float yAxisDif;
    private float stepCycle;
    private float nextStep;
    private int lastFootstep;
    private int consecutiveFootsteps;
    private CharacterController controller;
    public Animator playerAnimation;
    private PlayerHealth playerHealth;
    private AudioSource audioSource;
    private Vector2 input;
    private Vector3 moveDirection;
    private Vector3 localMoveDirection;
    private Vector3 spawnPosition;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerHealth = GetComponent<PlayerHealth>();
        audioSource = GetComponent<AudioSource>();
        moveDirection = Vector3.zero;
        localMoveDirection = Vector3.zero;

        playerAnimation = GetComponent<Animator>();

        // TODO: Move to respawn script
        // spawnPosition = spawnPoint.transform.position;

        bumpRight = false;
        bumpLeft = false;
        bumpForward = false;
        bumpBackward = false;
        curSpeed = 0;
        stepCycle = 0f;
        nextStep = stepCycle / 2f;
        fallStartLevel = transform.position.y;
    }

    void Update()
    {
        velocityMagnitude = controller.velocity.magnitude;
        // The jump state needs to read here to make sure it is not missed
        if (Input.GetButtonDown("Jump") && !jump && controller.isGrounded)
        {
            jump = true;
        }
    }

    void FixedUpdate()
    {
        float speed;
        GetInput(out speed);

        // Rotate movement vector with player rotation
        curAxisY = transform.localEulerAngles.y;
        yAxisDif = curAxisY - lastAxisY;
        lastAxisY = transform.localEulerAngles.y;
        moveDirection = Quaternion.Euler(0, yAxisDif, 0) * moveDirection;

        if (controller.isGrounded)
        {
            // Stop falling
            if (falling)
            {
                // Reset air control variables
                falling = false;
                bumpRight = false;
                bumpLeft = false;
                bumpForward = false;
                bumpBackward = false;

                // Keep 'inertia' from air control, and start step cycle
                moveDirection = new Vector3(controller.velocity.x, 0, controller.velocity.z);
                curSpeed = moveDirection.magnitude;
                nextStep = stepCycle + .5f;

                // Apply fall damage
                if (transform.position.y < fallStartLevel - fallDamageThreshold)
                {
                    playerHealth.TakeDamage(Mathf.RoundToInt(fallStartLevel - fallDamageThreshold) * 3);
                }
            }

            // If key press is opposite of current movement direction, set movement to zero for that movement axis
            if (((Input.GetAxisRaw("Horizontal") == -1 && Math.Sign(transform.InverseTransformDirection(controller.velocity).x) == 1)
               || (Input.GetAxisRaw("Horizontal") == 1 && Math.Sign(transform.InverseTransformDirection(controller.velocity).x) == -1))
               && transform.InverseTransformDirection(controller.velocity).x != 0)
            {
                localMoveDirection.x = 0;
            }
            if ((Input.GetAxisRaw("Vertical") == -1 && Math.Sign(transform.InverseTransformDirection(controller.velocity).z) == 1)
               || (Input.GetAxisRaw("Vertical") == 1 && Math.Sign(transform.InverseTransformDirection(controller.velocity).z) == -1)
               && transform.InverseTransformDirection(controller.velocity).z != 0)
            {
                localMoveDirection.z = 0;
            }

            // Smooth movement when changing direction
            localMoveDirection = Vector3.Lerp(localMoveDirection, new Vector3(input.x, 0, input.y), directionShiftSpeed);

            // Decelerate with no input
            if (input.magnitude == 0)
            {
                if (curSpeed > 0)
                {
                    curSpeed -= accelSpeed;
                }
            }
            else // Otherwise...
            {
                if (curSpeed < speed)   // accelerate until max speed is reached
                {
                    curSpeed += accelSpeed;
                    if (curSpeed > speed)
                    {
                        curSpeed = speed;
                    }
                }
                else if (curSpeed > speed)  // Decelerate if max speed changes from runSpeed to walkSpeed
                {
                    curSpeed -= accelSpeed;

                }
            }
            if (curSpeed < 0)
            {
                // Stop and reset movement speed and vector
                curSpeed = 0;
                localMoveDirection = Vector3.zero;
            }

            moveDirection = transform.TransformDirection(localMoveDirection);   // Convert relative direction to global
            moveDirection *= curSpeed;  // Apply speed

            // Apply jumping
            if (jump)
            {
                moveDirection.y = jumpSpeed;
                jump = false;
            }
        }
        else
        {
            // Record height at which falling begins
            if (!falling)
            {
                falling = true;
                fallStartLevel = transform.position.y;
            }
            AirControl();
        }
        // Apply gravity and move controller with calculated vectors
        moveDirection += Physics.gravity * gravityMultiplier * Time.fixedDeltaTime;
        controller.Move(moveDirection * Time.fixedDeltaTime);

        stepSpeed(curSpeed);

        // Die if you fall off the map
        if (controller.transform.position.y <= -25)
        {
            playerHealth.TakeDamage(100);
        }
    }

    private void GetInput(out float speed)
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");

        isWalking = !Input.GetKey(KeyCode.LeftShift);

        // Set the desired speed between walking and running
        speed = isWalking ? walkSpeed : runSpeed;
        input = new Vector2(horizontal, vertical);

        // Normalize directional speed
        if (input.sqrMagnitude > 1)
        {
            input.Normalize();
        }
    }

    private void AirControl()
    {
        // Get individual raw movement axes
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");

        // Create a relative vector for air control physics to manipulate
        var relativeVector = transform.InverseTransformDirection(moveDirection);

        // If you are moving sideways, and the opposite direction key is pressed, decelerate to minimum speed
        if (horizontal == -1 && Math.Sign(localMoveDirection.x) == 1 && relativeVector.x != minSpeed)
        {
            if (relativeVector.x - slowdown < minSpeed)
            {
                relativeVector.x = minSpeed;
            }
            else
            {
                relativeVector.x -= slowdown;
            }
        }
        else if (horizontal == 1 && Math.Sign(localMoveDirection.x) == -1 && relativeVector.x != -minSpeed)
        {
            if (relativeVector.x + slowdown > -minSpeed)
            {
                relativeVector.x = -minSpeed;
            }
            else
            {
                relativeVector.x += slowdown;
            }
        }

        // If you are moving forwards or backwards, and the opposite direction key is pressed, decelerate to minimum speed
        if (vertical == -1 && Math.Sign(localMoveDirection.z) == 1 && relativeVector.z != minSpeed)
        {
            if (relativeVector.z - slowdown < minSpeed)
            {
                relativeVector.z = minSpeed;
            }
            else
            {
                relativeVector.z -= slowdown;
            }
        }
        else if (vertical == 1 && Math.Sign(localMoveDirection.z) == -1 && relativeVector.z != -minSpeed)
        {
            if (relativeVector.z + slowdown > -minSpeed)
            {
                relativeVector.z = -minSpeed;
            }
            else
            {
                relativeVector.z += slowdown;
            }
        }

        // If there is no sideways momentum, and a sideways direction key is pressed, move at a fixed speed in that direction
        if (localMoveDirection.x == 0)
        {
            if (horizontal == 1)
            {
                bumpRight = true;
                bumpLeft = false;
            }
            if (horizontal == -1)
            {
                bumpLeft = true;
                bumpRight = false;
            }
        }
        if (bumpLeft)
        {
            relativeVector.x = -minSpeed;
        }
        if (bumpRight)
        {
            relativeVector.x = minSpeed;
        }

        // If there is no forward or backward momentum
        if (localMoveDirection.z == 0)
        {
            // and forwards or backwards key is pressed, set bump direction
            if (vertical == 1)
            {
                bumpForward = true;
                bumpBackward = false;
            }
            if (vertical == -1)
            {
                bumpBackward = true;
                bumpForward = false;
            }
        }

        // Apply movement to bump direction, if any
        if (bumpBackward)
        {
            relativeVector.z = -minSpeed;
        }
        if (bumpForward)
        {
            relativeVector.z = minSpeed;
        }

        // Apply air control to player's movement vector
        moveDirection = transform.TransformDirection(relativeVector);
    }

    void onStep()
    {
        if (!controller.isGrounded)
        {
            return;
        }

        int i = UnityEngine.Random.Range(0, 2);
        int curFootstep = i;

        if (curFootstep == lastFootstep)
        {
            consecutiveFootsteps++;
            if (consecutiveFootsteps == 2)
            {
                i = (curFootstep + 1) % 2;
                consecutiveFootsteps = 0;
            }
        }
        else
        {
            consecutiveFootsteps = 0;
        }
        lastFootstep = curFootstep;

        audioSource.clip = footStep[i];
        audioSource.PlayOneShot(audioSource.clip);
    }

    private void stepSpeed(float speed)
    {
        if (controller.velocity.sqrMagnitude > 0 && (input.x != 0 || input.y != 0))
        {
            stepCycle += (controller.velocity.magnitude + (speed * (isWalking ? 1f : runstepLengthen))) * Time.fixedDeltaTime;
        }

        if (!(stepCycle > nextStep))
        {
            return;
        }

        nextStep = stepCycle + stepInterval;

        onStep();
    }

    public void ResetPlayer()
    {
        bumpRight = false;
        bumpLeft = false;
        bumpForward = false;
        bumpBackward = false;
        curSpeed = 0;
        stepCycle = 0f;
        nextStep = stepCycle / 2f;
        fallStartLevel = transform.position.y;
        moveDirection = Vector3.zero;
        localMoveDirection = Vector3.zero;
        controller.Move(moveDirection);


        // TODO: Move below to respawn script
        //player.GetComponent<mouseLook>().Reset();
        //player.GetComponentInChildren<mouseLook>().Reset();
        //player.transform.position = spawnPosition;
    }
}
