using UnityEngine;
public class FPSGrappleMeleeController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 6f;
    public float sprintSpeed = 10f;
    public float jumpForce = 6f;
    public float gravity = -20f;
    [Header("Mouse")]
    public float mouseSensitivity = 200f;
    public Transform cameraPivot;
    [Header("Melee")]
    public float meleeRange = 2f;
    public float meleeDamage = 25f;
    public float meleeCooldown = 0.5f;
    private float nextMeleeTime;
    [Header("Grapple")]
    public float grappleRange = 30f;
    public float grapplePullSpeed = 25f;
    public LayerMask grappleMask;
    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation;
    private bool isGrappling;
    private Vector3 grapplePoint;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        Look();
        Move();
        HandleMelee();
        HandleGrapple();
    }
    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime * 100f;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime * 100f;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
    void Move()
    {
        if (isGrappling)
        {
            Vector3 dir = (grapplePoint - transform.position).normalized;
            controller.Move(dir * grapplePullSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, grapplePoint) < 2f)
                isGrappling = false;
            return;
        }
        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
            velocity.y = jumpForce;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(move * speed * Time.deltaTime);
        controller.Move(velocity * Time.deltaTime);
    }
    void HandleMelee()
    {
        if (Input.GetMouseButtonDown(0) && Time.time > nextMeleeTime)
        {
            nextMeleeTime = Time.time + meleeCooldown;
            Ray ray = new Ray(cameraPivot.position, cameraPivot.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, meleeRange))
            {
                Debug.Log("Hit: " + hit.collider.name);
                if (hit.collider.TryGetComponent<IDamageable>(out var dmg))
                {
                    dmg.TakeDamage(meleeDamage);
                }
            }
        }
    }
    void HandleGrapple()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = new Ray(cameraPivot.position, cameraPivot.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, grappleRange, grappleMask))
            {
                grapplePoint = hit.point;
                isGrappling = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            isGrappling = false;
        }
    }
}
