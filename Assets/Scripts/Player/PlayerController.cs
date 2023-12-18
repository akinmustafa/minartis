using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Use SerializeField method to prevent external access.
    [SerializeField] Transform playerCamera = null;
    [SerializeField] float mouseSensitivity = 3f;
    [SerializeField] bool lockCursor = true;

    [SerializeField] float walkSpeed = 6f;
    [SerializeField] float gravity = -25f;

    [SerializeField][Range(0.0f, 0.5f)] float moveSmoothness = 0.12f;
    [SerializeField][Range(0.0f, 0.5f)] float mouseSmoothness = 0.02f;

    float cameraPitch = 0;
    float velocityY = 0;
    CharacterController controller = null;

    Vector2 currentDir = Vector2.zero;
    Vector2 currentDirVelocity = Vector2.zero;

    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseDeltaVelocity = Vector2.zero;

    void Start()
    {
        //Object must have a Character Controller component.
        controller = GetComponent<CharacterController>();

        if (lockCursor)
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
    }

    void Update()
    {
        UpdateCamera();
        UpdateMovement();
    }

    void UpdateCamera()
    {
        //First person camera movement with mouse position.
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothness);

        cameraPitch -= currentMouseDelta.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -90.0f, 90.0f);

        playerCamera.localEulerAngles = Vector3.right * cameraPitch;
        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    void UpdateMovement()
    {
        //First person movement with smoothing.
        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();

        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothness);

        if (controller.isGrounded)
            velocityY = 0.0f;

        velocityY += gravity * Time.deltaTime;
		
        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * walkSpeed + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);
    }
}