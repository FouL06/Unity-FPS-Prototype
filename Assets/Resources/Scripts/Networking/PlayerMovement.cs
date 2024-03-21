using UnityEngine;

public class PlayerMovement : SuperStateMachine
{
    enum PlayerStates { Idle, Walk, Jump, Fall };

    public float walkSpeed = 6.0f;
    public float runSpeed = 10.0f;
    private float maxSpeed;
    public float acceleration = 1.5f;
    public float jumpHeight = 8.0f;
    public float gravity = 25.0f;
    public float directionSharpnessFactor = 45f;
    public float fallDamageThreshold = 10.0f;
    public AudioClip[] footStep = new AudioClip[2];
    public float stepInterval;
    [Range(0f, 1f)]
    public float runstepLengthen;
    public float minAngle = -89;
    public float maxAngle = 89;

    private bool bumpLeft;
    private bool bumpRight;
    private bool bumpForward;
    private bool bumpBackward;
    private float currentSpeed;
    private float fallStartLevel;
    private float stepCycle;
    private float nextStep;
    private int lastFootstep;
    private int consecutiveFootsteps;
    private PlayerFixedController controller;
    private PlayerHealth playerHealth;
    private AudioSource audioSource;
    public Vector3 moveDirection;
    private Vector3 localJumpDirection;
    private Vector3 spawnPosition;
    private Vector3 lookDirection;

    private bool jumpForward;
    private bool jumpBackward;

    public Transform PlayerBody;
    public Transform MouseLook;

    private PlayerInput input;
    private int lastGround;     //Represent last tick the controller touched ground
    public float friction = 10.0f;

    void Start()
    {
        controller = GetComponent<PlayerFixedController>();
        input = GetComponent<PlayerInput>();
        currentState = PlayerStates.Idle;

        playerHealth = GetComponent<PlayerHealth>();
        audioSource = GetComponent<AudioSource>();
        moveDirection = Vector3.zero;

        // TODO: Move to respawn script
        // spawnPosition = spawnPoint.transform.position;

        bumpRight = false;
        bumpLeft = false;
        bumpForward = false;
        bumpBackward = false;
        currentSpeed = 0;
        maxSpeed = walkSpeed;
        stepCycle = 0f;
        nextStep = stepCycle / 2f;
        fallStartLevel = transform.position.y;
        lookDirection = transform.forward;
    }

    void Awake()
    {
        input = GetComponent<PlayerInput>();
        controller = GetComponent<PlayerFixedController>();
        lookDirection = transform.forward;
    }

    public void RunUpdate(float delta)
    {
        controller.DoUpdate(delta);
    }

    protected override void EarlyGlobalSuperUpdate()
    {
        lookDirection = transform.forward;

        lastGround++;
        if (AcquiringGround())
            lastGround = 0;
    }

    protected override void LateGlobalSuperUpdate()
    {
        transform.position += moveDirection * controller.deltaTime;
    }

    private bool AcquiringGround()
    {
        return controller.currentGround.IsGrounded(false, 0.01f);
    }

    private bool MaintainingGround()
    {
        return controller.currentGround.IsGrounded(true, 0.5f);
    }

    private Vector3 LocalMovement()
    {
        Vector3 right = Vector3.Cross(controller.up, lookDirection);

        Vector3 local = Vector3.zero;

        if (input.currentInput.movementInput.x != 0)
        {
            local += right * input.currentInput.movementInput.x;
        }

        if (input.currentInput.movementInput.z != 0)
        {
            local += lookDirection * input.currentInput.movementInput.z;
        }

        return local.normalized;
    }

    private float CalculateJumpVelocity()
    {
        return Mathf.Sqrt(0.5f * jumpHeight * gravity);
    }

    void Idle_EnterState()
    {
        controller.EnableSlopeLimit();
        controller.EnableClamping();
    }

    void Idle_SuperUpdate()
    {
        if (currentSpeed > 0)
            currentSpeed -= acceleration;

        if (currentSpeed < 0)
            currentSpeed = 0;

        if (input.currentInput.inputJump)
        {
            currentState = PlayerStates.Jump;
            return;
        }

        if (!MaintainingGround())
        {
            currentState = PlayerStates.Fall;
            return;
        }

        if (input.currentInput.movementInput != Vector3.zero)
        {
            currentState = PlayerStates.Walk;
            return;
        }

        // Apply friction to slow us to a halt
        moveDirection = Vector3.MoveTowards(moveDirection, Vector3.zero, friction * controller.deltaTime);
    }

    void Idle_ExitState()
    {
    }

    void Walk_EnterState()
    {

    }

    void Walk_SuperUpdate()
    {
        if (input.currentInput.inputJump)
        {
            currentState = PlayerStates.Jump;
            return;
        }

        if (!MaintainingGround())
        {

            currentState = PlayerStates.Fall;
            return;

        }

        if (currentSpeed < maxSpeed)
        {
            currentSpeed += acceleration;
            if (currentSpeed > maxSpeed)
                currentSpeed = maxSpeed;
        }
        else if (currentSpeed > maxSpeed)
        {
            currentSpeed -= acceleration;
        }

        if (input.currentInput.movementInput != Vector3.zero)
        {
            maxSpeed = input.currentInput.inputRun ? runSpeed : walkSpeed;

            moveDirection = Vector3.MoveTowards(moveDirection, LocalMovement() * currentSpeed, directionSharpnessFactor * Time.deltaTime);
        }
        else
        {
            currentState = PlayerStates.Idle;
            return;
        }
    }

    void Jump_EnterState()
    {
        controller.DisableClamping();
        controller.DisableSlopeLimit();
        localJumpDirection = LocalMovement();

        if (localJumpDirection == lookDirection)
            jumpForward = true;
        else if (localJumpDirection == -lookDirection)
            jumpBackward = true;

        moveDirection += controller.up * CalculateJumpVelocity();
    }

    void Jump_SuperUpdate()
    {
        Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
        Vector3 verticalMoveDirection = moveDirection - planarMoveDirection;


        if (jumpForward)
        {
            localJumpDirection += lookDirection;
        }
        else if (jumpBackward)
        {
            localJumpDirection -= lookDirection;
        }

        localJumpDirection.Normalize();

        if (Vector3.Angle(verticalMoveDirection, controller.up) > 90 && AcquiringGround())
        {
            moveDirection = planarMoveDirection;
            currentState = PlayerStates.Idle;
            return;
        }

        planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, localJumpDirection * currentSpeed, Mathf.Infinity);
        verticalMoveDirection -= controller.up * gravity * controller.deltaTime;

        moveDirection = planarMoveDirection + verticalMoveDirection;
    }

    void Jump_ExitState()
    {
        jumpForward = false;
        jumpBackward = false;
    }

    void Fall_EnterState()
    {
        controller.DisableClamping();
        controller.DisableSlopeLimit();
    }

    void Fall_SuperUpdate()
    {
        if (AcquiringGround())
        {
            moveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
            currentState = PlayerStates.Idle;
            return;
        }

        moveDirection -= controller.up * gravity * controller.deltaTime;
    }

    void Fall_ExitState()
    {
        nextStep = stepCycle + .5f;
    }

    // TODO: Get AirControl() working with this script
    /*private Vector3 AirControl()
    {
        Vector3 right = Vector3.Cross(controller.up, lookDirectionY);
        Vector3 local = transform.InverseTransformDirection(moveDirection);

        float horizontal = input.currentInput.movementInput.x;
        float vertical = input.currentInput.movementInput.x;

        // If you are moving sideways, and the opposite direction key is pressed, decelerate to minimum speed
        if (horizontal == -1 && Math.Sign(localMoveDirection.x) == 1 && local.x != minSpeed)
        {
            if (local.x - slowdown < minSpeed)
            {
                local.x = minSpeed;
            }
            else
            {
                local.x -= slowdown;
            }
        }
        else if (horizontal == 1 && Math.Sign(localMoveDirection.x) == -1 && local.x != -minSpeed)
        {
            if (local.x + slowdown > -minSpeed)
            {
                local.x = -minSpeed;
            }
            else
            {
                local.x += slowdown;
            }
        }

        // If you are moving forwards or backwards, and the opposite direction key is pressed, decelerate to minimum speed
        if (vertical == -1 && Math.Sign(localMoveDirection.z) == 1 && local.z != minSpeed)
        {
            if (local.z - slowdown < minSpeed)
            {
                local.z = minSpeed;
            }
            else
            {
                local.z -= slowdown;
            }
        }
        else if (vertical == 1 && Math.Sign(localMoveDirection.z) == -1 && local.z != -minSpeed)
        {
            if (local.z + slowdown > -minSpeed)
            {
                local.z = -minSpeed;
            }
            else
            {
                local.z += slowdown;
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
            local.x = -minSpeed;
        }
        if (bumpRight)
        {
            local.x = minSpeed;
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
            local.z = -minSpeed;
        }
        if (bumpForward)
        {
            local.z = minSpeed;
        }

        // Apply air control to player's movement vector
        return transform.TransformDirection(local);
    }*/

    public static Vector3 ClampAngleX(Vector3 angle, float min, float max)
    {
        if (angle.z < 0)
            angle.z = 0;

        if (angle.z > max / 360)
            angle.z = max / 360;

        return new Vector3(0, Mathf.Clamp(angle.y, min / 360, max / 360), angle.z);
    }

    //void onStep()
    //{
    //    if (!controller.isGrounded)
    //    {
    //        return;
    //    }

    //    int i = UnityEngine.Random.Range(0, 2);
    //    int curFootstep = i;

    //    if (curFootstep == lastFootstep)
    //    {
    //        consecutiveFootsteps++;
    //        if (consecutiveFootsteps == 2)
    //        {
    //            i = (curFootstep + 1) % 2;
    //            consecutiveFootsteps = 0;
    //        }
    //    }
    //    else
    //    {
    //        consecutiveFootsteps = 0;
    //    }
    //    lastFootstep = curFootstep;

    //    audioSource.clip = footStep[i];
    //    audioSource.PlayOneShot(audioSource.clip);
    //}

    //private void stepSpeed(float speed)
    //{
    //    if (controller.velocity.sqrMagnitude > 0 && (dep_input.x != 0 || dep_input.y != 0))
    //    {
    //        stepCycle += (controller.velocity.magnitude + (speed * (isWalking ? 1f : runstepLengthen))) * Time.fixedDeltaTime;
    //    }

    //    if (!(stepCycle > nextStep))
    //    {
    //        return;
    //    }

    //    nextStep = stepCycle + stepInterval;

    //    onStep();
    //}

    //public void ResetPlayer()
    //{
    //    bumpRight = false;
    //    bumpLeft = false;
    //    bumpForward = false;
    //    bumpBackward = false;
    //    curSpeed = 0;
    //    stepCycle = 0f;
    //    nextStep = stepCycle / 2f;
    //    fallStartLevel = transform.position.y;
    //    moveDirection = Vector3.zero;
    //    localMoveDirection = Vector3.zero;
    //    transform.position = moveDirection;


    //    // TODO: Move below to respawn script
    //    //player.GetComponent<mouseLook>().Reset();
    //    //player.GetComponentInChildren<mouseLook>().Reset();
    //    //player.transform.position = spawnPosition;
    //}
}
