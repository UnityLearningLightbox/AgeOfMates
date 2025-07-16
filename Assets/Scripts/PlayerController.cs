using UnityEngine;

[RequireComponent(typeof(CharacterController))] /// añade un character controller si falta en el personaje y hace que no haga falta tener un rigidbody
public class PlayerController : MonoBehaviour
{
    [Header("Player Movement")]
    float defaultPlayerSpeed;
    [SerializeField] float playerSpeed;
    [SerializeField] float runningSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] KeyCode runKey = KeyCode.LeftShift;

    [Header("Player Jump")]
    [SerializeField] float jumpForce;
    [SerializeField] bool isGrounded;
    [SerializeField] bool hasJumped = false;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius;

    [Header("Camera Settings")]
    public Transform cameraTransform;
    [SerializeField] Vector3 cameraOffset = new Vector3(0, 2, -3);
    [SerializeField] float cameraFollowSpeed = 10f;
    [SerializeField] float mouseSensibility;
    [SerializeField] float maxPitch;
    [SerializeField] float minPitch;
    float pitch;
    float yaw;

    [Header("Components")]
    [SerializeField] Rigidbody rb;
    //[SerializeField] Animator animator;

    [Header("TEST")]
    [SerializeField] Camera playerCamera;
    [SerializeField] float gravity = 10f;
    [SerializeField] float lookSpeed = 2f;
    [SerializeField] float lookXLimit = 45f;

    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0f;
    public bool canMove = true;
    CharacterController characterController;

    //[Header("TEST 2")]
    ////[SerializeField] CinemachineCamera fpCamera;
    //[SerializeField] Vector2 lookSensitivity = new Vector2(0.1f, 0.1f);
    //[SerializeField] float pitchLimit = 45f;
    //[SerializeField] float currentPitch = 0f;
    //[SerializeField] Vector2 lookInput;
    //public float CurrentPitch
    //{
    //    get => currentPitch;
    //    set
    //    {
    //        currentPitch = Mathf.Clamp(value, -pitchLimit, pitchLimit);
    //    }
    //}

    private void Start()
    {
        InitialSettings();
    }

    private void Update()
    {
        CameraRotation();
        //CameraRotation2();
    }

    private void FixedUpdate()
    {
        GroundChecker();

        PlayerMovement();
        //PlayerMovement2();

        PlayerJump();

        UpdateCameraMovement();
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    void InitialSettings()
    {
        characterController = GetComponent<CharacterController>();

        if (rb != null)
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = true;
        }

        //if(animator != null)
        //{
        //    animator = GetComponent<Animator>();
        //}

        defaultPlayerSpeed = playerSpeed;

        //yaw = transform.eulerAngles.y;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void PlayerMovement() /// Prueba nº1: como lo hicimos en clase
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;

        bool isMoving = input.magnitude >= 0.1f;
        bool isRunning = isMoving && Input.GetKey(runKey);

        if (isMoving)
        {
            Quaternion cameraRotation = Quaternion.Euler(0, yaw, 0);
            Vector3 moveDir = cameraRotation * input;
            moveDir.y = 0f;
            moveDir.Normalize();

            rb.MovePosition(rb.position + moveDir * playerSpeed * Time.fixedDeltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
        }

        if (isRunning)
        {
            playerSpeed = runningSpeed;

        }
        else
        {
            playerSpeed = defaultPlayerSpeed;
        }
    }

    void PlayerMovement2() /// Prueba nº2: visto en un turorial
    {
        //Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        //bool isMoving = input.magnitude >= 0.1f;
        bool isRunning = Input.GetKey(runKey);

        float curSpeedX = canMove ? (isRunning ? runningSpeed : playerSpeed) * Input.GetAxis("Vertical") : 0f;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : playerSpeed) * Input.GetAxis("Horizontal") : 0f;
        float movementDirectionY = moveDirection.y;

        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        /// Saltar
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpForce;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    void PlayerJump()
    {
        if (isGrounded && Input.GetButton("Jump"))
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            hasJumped = true;
        } else
        {
            hasJumped = false;
        }

        bool isInTheAir = !isGrounded;
        bool isFalling = isInTheAir && rb.linearVelocity.y < -0.1f;

        //animator.SetBool("isJumping", isInTheAir);
        //animator.SetBool("isFalling", isFalling);
    }

    void GroundChecker()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void CameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensibility;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensibility;

        float rightStickX = Input.GetAxis("RightStickHorizontal");
        float rightStickY = Input.GetAxis("RightStickVertical");

        if (Mathf.Abs(rightStickX) < 0.2f) rightStickX = 0f;
        if (Mathf.Abs(rightStickY) < 0.2f) rightStickY = 0f;

        float inputX = mouseX + rightStickX;
        float inputY = mouseY + rightStickY;

        yaw += inputX;
        pitch -= inputY;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        ///// TEST 3 /////
        //Quaternion cameraRotation = Quaternion.Euler(pitch, yaw, 0);
        //rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        //rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        //cameraTransform.localRotation = cameraRotation;
        //cameraTransform.rotation = cameraRotation;
        //playerCamera.transform.localRotation = cameraRotation;
        //transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);

        /////// TEST 1 ///////
        //Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
        //bool isMoving = input.magnitude >= 0.1f;
        //bool isRunning = isMoving && Input.GetKey(runKey);

        //if (isRunning)
        //{
        //    cameraOffset = new Vector3(0, 0.5f, 1.5f);
        //    pitch = Mathf.Clamp(pitch, -40, 40); ;
        //}
        //else
        //{
        //    pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        //    cameraOffset = new Vector3(0, 0.5f, 1f);
        //}

        ////// TEST 2 //////
        //rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        //rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        //playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        //transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }

    void CameraRotation2() /// Si se usa esta forma, no hace falta el UpdateCameraMovement()
    {
        //characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    void UpdateCameraMovement()
    {
        Quaternion cameraRotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = transform.position + cameraRotation * cameraOffset;

        cameraTransform.position = Vector3.Lerp(cameraTransform.position, desiredPosition, cameraFollowSpeed * Time.fixedDeltaTime);
        cameraTransform.rotation = cameraRotation;
    }
}
