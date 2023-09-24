using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private bool isCrouching = false;

    public float CrouchSpeed = 1.0f;
    private Animator animator;
    public Image HP_bar, MP_bar, ST_bar;
    public float HP_amount = 100;
    public float ST_amount = 100;
    public float MP_amount = 100;
    public float ustalost = 10f;

    private float MinYaw = -360;
    private float MaxYaw = 360;
    private float MinPitch = 0;
    private float MaxPitch = 0;
    private float LookSensitivity = 1;

    public float MoveSpeed = 10;
    public float SprintSpeed = 30;
    private float currMoveSpeed = 0;

    protected CharacterController movementController;
    protected Camera playerCamera;

    protected bool isControlling;
    protected float yaw;
    protected float pitch;

    private float gravity = 9.81f;
    private float verticalVelocity = 0;
    public float jumpForce = 8f;
    private bool isJumping = false;

    private bool isRunning = false;
    private float timeSinceNotRunning = 0;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();

        HP_bar.fillAmount = HP_amount / 100;
        ST_bar.fillAmount = ST_amount / 100;
        MP_bar.fillAmount = MP_amount / 100;

        Cursor.lockState = CursorLockMode.Locked;
        movementController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        isControlling = true;
        ToggleControl();
    }

    protected virtual void Update()
    {
         if (Input.GetKey(KeyCode.LeftControl))
    {
        // Спускаем камеру на 2f
        Vector3 newPosition = playerCamera.transform.localPosition;
        newPosition.y = 0.75f;
        playerCamera.transform.localPosition = newPosition;
    }
    else
    {
        // Возвращаем камеру на исходную позицию
        Vector3 newPosition = playerCamera.transform.localPosition;
        newPosition.y = 1.7f;
        playerCamera.transform.localPosition = newPosition;
    }

        if (Input.GetKey(KeyCode.C))
        {
            StartCrouch();
        }
        else
        {
            StopCrouch();
        }

        Vector3 direction = Vector3.zero;
        direction += transform.forward * Input.GetAxisRaw("Vertical");
        direction += transform.right * Input.GetAxisRaw("Horizontal");

        direction.Normalize();

        if (movementController.isGrounded)
        {
            verticalVelocity = 0;
            isJumping = false;
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            verticalVelocity = jumpForce;
            isJumping = true;
            ST_amount -= 4 * ustalost;
            ST_bar.fillAmount = ST_amount / 100;
        }

        if (Input.GetKey(KeyCode.LeftShift) && ST_amount > 0)
        {
            ST_amount = Mathf.Clamp(ST_amount, 0, 100);
            isRunning = true;
            ST_amount -= ustalost * Time.deltaTime;
            ST_bar.fillAmount = ST_amount / 100;
            timeSinceNotRunning = 0;
        }
        else
        {
            isRunning = false;
            timeSinceNotRunning += Time.deltaTime;

            if (timeSinceNotRunning >= 5f)
            {
                ST_amount = Mathf.Clamp(ST_amount, 0, 100);
                ST_amount += 20 * Time.deltaTime;
                ST_bar.fillAmount = ST_amount / 100;
            }
        }

        if (isRunning)
        {
            currMoveSpeed = SprintSpeed;
        }
        else
        {
            currMoveSpeed = MoveSpeed;
        }

        direction.y = verticalVelocity;

        movementController.Move(direction * Time.deltaTime * currMoveSpeed);

        yaw += Input.GetAxisRaw("Mouse X") * LookSensitivity;
        pitch -= Input.GetAxisRaw("Mouse Y") * LookSensitivity;

        yaw = ClampAngle(yaw, MinYaw, MaxYaw);
        pitch = ClampAngle(pitch, MinPitch, MaxPitch);

        Vector3 rotation = new Vector3(pitch, yaw, 0.0f);
        transform.rotation = Quaternion.Euler(rotation);    }


    protected float ClampAngle(float angle)
    {
        return ClampAngle(angle, 0, 360);
    }

    private void StartCrouch()
    {
        if (!isCrouching)
        {
            isCrouching = true;
            movementController.height = 1.0f;
            MoveSpeed = MoveSpeed / 2;
        }
    }

    private void StopCrouch()
    {
        if (isCrouching)
        {
            isCrouching = false;
            movementController.height = 2.0f;
            MoveSpeed = MoveSpeed;
        }
    }

    protected float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }

    protected void ToggleControl()
    {
        playerCamera.gameObject.SetActive(isControlling);

#if UNITY_5
        Cursor.lockState = (isControlling) ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isControlling;
#else
        Screen.lockCursor = isControlling;
#endif
    }
}
