using Unity.Cinemachine;
using UnityEngine;

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

    [Header("FirstPerson Settings")]
    [SerializeField] CinemachineCamera fpCamera;
    [SerializeField] float lookSpeed = 2f;
    [SerializeField] Transform lookAt;
    [SerializeField] Vector3 lookAtOffset;

    [Header("Components")]
    [SerializeField] Rigidbody rb;
    //[SerializeField] Animator animator;

    private void Start()
    {
        InitialSettings();
    }

    private void Update()
    {
        CameraRotation();
    }

    private void FixedUpdate()
    {
        GroundChecker();

        PlayerMovement();
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

        yaw = transform.eulerAngles.y;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void PlayerMovement()
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
        // Ratón
        float mouseX = Input.GetAxis("Mouse X") * mouseSensibility;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensibility;

        // Mando
        float rightStickX = Input.GetAxis("RightStickHorizontal");
        float rightStickY = Input.GetAxis("RightStickVertical");

        if (Mathf.Abs(rightStickX) < 0.2f) rightStickX = 0f;
        if (Mathf.Abs(rightStickY) < 0.2f) rightStickY = 0f;

        float inputX = mouseX + rightStickX;
        float inputY = mouseY + rightStickY;

        yaw += inputX;
        pitch -= inputY;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0); // Para que el personaje rote junto a la camara
    }

    void UpdateCameraMovement()
    {
        Quaternion cameraRotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = transform.position + cameraRotation * cameraOffset;

        fpCamera.transform.position = Vector3.Lerp(fpCamera.transform.position, desiredPosition, cameraFollowSpeed * Time.fixedDeltaTime);
        fpCamera.transform.rotation = cameraRotation;

        if (lookAt != null)
        {
            Vector3 lookAtPosition = fpCamera.transform.position + cameraRotation * lookAtOffset;
            lookAt.position = Vector3.Lerp(lookAt.position, lookAtPosition, cameraFollowSpeed * Time.fixedDeltaTime);
            lookAt.rotation = cameraRotation;
        }
    }
}
