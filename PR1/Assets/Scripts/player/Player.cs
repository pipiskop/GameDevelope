using System.Collections;
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
    private float MinPitch = -90;
    private float MaxPitch = 90;
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

    public GameObject spell1Prefab;
    public GameObject spell2Prefab;
    public GameObject spell3Prefab;
    public GameObject spell4Prefab;
    public GameObject spell5Prefab;

    public float spell1Speed = 10f;
    public float spell2Speed = 5f;
    public float spell3Speed = 15f;
    public float spell4Speed = 12f;
    public float spell5Speed = 8f;

    public float spell1Cooldown = 3f;
    public float spell2Cooldown = 5f;
    public float spell3Cooldown = 8f;
    public float spell4Cooldown = 6f;
    public float spell5Cooldown = 10f;

    public float spell1ManaCost = 20f;
    public float spell2ManaCost = 20f;
    public float spell3ManaCost = 30f;
    public float spell4ManaCost = 25f;
    public float spell5ManaCost = 35f;

    public float nextspell1Time = 0f;
    public float nextspell2Time = 0f;
    public float nextspell3Time = 0f;
    public float nextspell4Time = 0f;
    public float nextspell5Time = 0f;

    private int selectedSpell = 1;

    private float timeSinceLastManaUse = 0f;
    private float manaRegenerationRate = 10f;

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
            Vector3 newPosition = playerCamera.transform.localPosition;
            newPosition.y = 0.75f;
            playerCamera.transform.localPosition = newPosition;
        }
        else
        {
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


        {
            // ... ¬аш существующий код

            // ѕроверка наличи€ действий с маной
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2) ||
                Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Alpha5))
            {
                timeSinceLastManaUse = 0; // ќбнул€ем врем€, так как мана была использована
            }
            else
            {
                // ≈сли не было действий с маной в течение 3 секунд, начинаем ее восстановление
                timeSinceLastManaUse += Time.deltaTime;
                if (timeSinceLastManaUse >= 3f)
                {
                    MP_amount += manaRegenerationRate * Time.deltaTime;
                    MP_amount = Mathf.Clamp(MP_amount, 0, 100);
                    MP_bar.fillAmount = MP_amount / 100;
                }
            }
        }

        direction.y = verticalVelocity;

        movementController.Move(direction * Time.deltaTime * currMoveSpeed);

        float mouseY = Input.GetAxis("Mouse Y");

        pitch -= mouseY * LookSensitivity;

        pitch = Mathf.Clamp(pitch, MinPitch, MaxPitch);

        Vector3 cameraRotation = new Vector3(pitch, 0.0f, 0.0f);
        playerCamera.transform.localRotation = Quaternion.Euler(cameraRotation);

        yaw += Input.GetAxisRaw("Mouse X") * LookSensitivity;
        pitch = Mathf.Clamp(pitch, MinPitch, MaxPitch);

        yaw = ClampAngle(yaw, MinYaw, MaxYaw);
        pitch = ClampAngle(pitch, MinPitch, MaxPitch);

        Vector3 rotation = new Vector3(0.0f, yaw, 0.0f);
        transform.rotation = Quaternion.Euler(rotation);

        if (Input.GetKeyDown(KeyCode.L))
        {
            GameObject spell1 = Instantiate(spell1Prefab, transform.position, transform.rotation);
            spell1.transform.forward = transform.forward;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedSpell = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedSpell = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedSpell = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedSpell = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            selectedSpell = 5;
        }

        if (Input.GetMouseButtonDown(0))
        {
            CastSpell();
        }
    }

    public void RestoreHealth(float amount)
    {
        HP_amount += amount;
        HP_amount = Mathf.Clamp(HP_amount, 0, 100);
        HP_bar.fillAmount = HP_amount / 100;
    }

    public void RestoreMana(float amount)
    {
        MP_amount += amount;
        MP_amount = Mathf.Clamp(MP_amount, 0, 100);
        MP_bar.fillAmount = MP_amount / 100;
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

    protected float ClampAngle(float angle)
    {
        return ClampAngle(angle, 0, 360);
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

    void CastSpell()
    {
        switch (selectedSpell)
        {
            case 1:
                Castspell1();
                break;
            case 2:
                Castspell2();
                break;
            case 3:
                Castspell3();
                break;
            case 4:
                Castspell4();
                break;
            case 5:
                Castspell5();
                break;
        }
    }

    void Castspell1()
    {
        if (Time.time >= nextspell1Time && MP_amount >= spell1ManaCost)
        {
            MP_amount -= spell1ManaCost;
            MP_bar.fillAmount = MP_amount / 100;

            GameObject spell1 = Instantiate(spell1Prefab, playerCamera.transform.position, playerCamera.transform.rotation);
            spell1.transform.forward = playerCamera.transform.forward;

            nextspell1Time = Time.time + spell1Cooldown;
        }
    }




    void Castspell2()
    {
        if (Time.time >= nextspell2Time && MP_amount >= spell2ManaCost)
        {
            MP_amount -= spell2ManaCost;
            MP_bar.fillAmount = MP_amount / 100;

            GameObject spell2 = Instantiate(spell2Prefab, playerCamera.transform.position, playerCamera.transform.rotation);
            spell2.transform.forward = playerCamera.transform.forward;

            nextspell2Time = Time.time + spell2Cooldown;
        }
    }

    void Castspell3()
    {
        if (Time.time >= nextspell3Time && MP_amount >= spell3ManaCost)
        {
            MP_amount -= spell3ManaCost;
            MP_bar.fillAmount = MP_amount / 100;

            GameObject spell3 = Instantiate(spell3Prefab, playerCamera.transform.position, playerCamera.transform.rotation);
            spell3.transform.forward = playerCamera.transform.forward;

            nextspell3Time = Time.time + spell3Cooldown;
        }
    }

    void Castspell4()
    {
        if (Time.time >= nextspell4Time && MP_amount >= spell4ManaCost)
        {
            MP_amount -= spell4ManaCost;
            MP_bar.fillAmount = MP_amount / 100;

            GameObject spell4 = Instantiate(spell4Prefab, playerCamera.transform.position, playerCamera.transform.rotation);
            spell4.transform.forward = playerCamera.transform.forward;

            nextspell4Time = Time.time + spell4Cooldown;
        }
    }

    void Castspell5()
    {
        if (Time.time >= nextspell5Time && MP_amount >= spell5ManaCost)
        {
            MP_amount -= spell5ManaCost;
            MP_bar.fillAmount = MP_amount / 100;

            GameObject spell5 = Instantiate(spell5Prefab, playerCamera.transform.position, playerCamera.transform.rotation);
            spell5.transform.forward = playerCamera.transform.forward;

            nextspell5Time = Time.time + spell5Cooldown;
        }
    }
}
