using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Unity.Cinemachine;

public class PlayerDash : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;
    public Animator animator;
    public Slider dashBar;
    public Trisl trail;
    public Transform cameraTarget;

    [Header("Audio")]
    public AudioSource dashSound;
    public AudioSource wallBreakSound;

    [Header("Dash Effect")]
    public ParticleSystem dashEffect1;

    [Header("Dash")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.3f;
    public float maxDashPoints = 8f;
    public float dashCost = 2f;
    public float rechargeSpeed = 1f;

    [Header("Dash Bar Colors")]
    public Color canDashColor = Color.red;
    public Color cannotDashColor = Color.white;

    [Header("Camera")]
    public float cameraBackAmount = 0.5f;
    public float cameraMoveSpeed = 8f;
    public float cameraReturnSpeed = 8f;

    [Header("FOV")]
    public CinemachineCamera thirdPersonAimCamera;
    public float normalFOV = 65f;
    public float dashFOV = 80f;
    public float fovChangeSpeed = 8f;

    [Header("Breakable Wall")]
    public float breakRayDistance = 2f;
    public LayerMask breakableWallLayer;
    public GameObject brokenWallPrefab;

    float dashPoints;
    float dashTimer;
    Vector3 cameraStartPosition;
    bool isDashing;

    void Start()
    {
        dashPoints = maxDashPoints;
        cameraStartPosition = cameraTarget.localPosition;

        dashBar.minValue = 0;
        dashBar.maxValue = maxDashPoints;

        thirdPersonAimCamera.Lens.FieldOfView = normalFOV;

        dashEffect1.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        UpdateDashBar();
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        Recharge();

        if (Keyboard.current.spaceKey.wasPressedThisFrame &&
            !isDashing &&
            dashPoints >= dashCost)
        {
            StartDash();
        }

        if (isDashing)
        {
            controller.Move(transform.forward * dashSpeed * Time.deltaTime);
            CheckBreakableWall();

            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0)
                StopDash();
        }

        UpdateCamera();
        UpdateFOV();
    }

    void StartDash()
    {
        isDashing = true;
        dashPoints = Mathf.Max(0, dashPoints - dashCost);
        dashTimer = dashDuration;

        animator.SetBool("Dashing", true);

        trail?.StartTrail();
        dashSound?.Play();
        dashEffect1?.Play();

        UpdateDashBar();
    }

    void StopDash()
    {
        isDashing = false;

        animator.SetBool("Dashing", false);

        trail?.StopTrail();

        dashEffect1?.Stop(
            true,
            ParticleSystemStopBehavior.StopEmittingAndClear
        );
    }

    void CheckBreakableWall()
    {
        Vector3 origin = transform.position + Vector3.up;

        RaycastHit[] hits = Physics.RaycastAll(
            origin,
            transform.forward,
            breakRayDistance,
            breakableWallLayer
        );

        foreach (RaycastHit hit in hits)
        {
            hit.collider.gameObject.SetActive(false);

            wallBreakSound?.Play();

            if (brokenWallPrefab)
            {
                Instantiate(
                    brokenWallPrefab,
                    hit.collider.transform.position,
                    hit.collider.transform.rotation
                );
            }
        }
    }

    void Recharge()
    {
        if (dashPoints >= maxDashPoints) return;

        dashPoints = Mathf.Min(
            maxDashPoints,
            dashPoints + rechargeSpeed * Time.deltaTime
        );

        UpdateDashBar();
    }

    void UpdateDashBar()
    {
        if (!dashBar) return;

        dashBar.value = dashPoints;

        Image fill = dashBar.fillRect?.GetComponent<Image>();

        if (fill)
            fill.color =
                dashPoints >= dashCost
                    ? canDashColor
                    : cannotDashColor;
    }

    void UpdateCamera()
    {
        if (!cameraTarget) return;

        Vector3 target = isDashing
            ? cameraStartPosition + Vector3.back * cameraBackAmount
            : cameraStartPosition;

        float speed = isDashing
            ? cameraMoveSpeed
            : cameraReturnSpeed;

        cameraTarget.localPosition = Vector3.Lerp(
            cameraTarget.localPosition,
            target,
            speed * Time.deltaTime
        );
    }

    void UpdateFOV()
    {
        if (!thirdPersonAimCamera) return;

        float target = isDashing ? dashFOV : normalFOV;

        thirdPersonAimCamera.Lens.FieldOfView = Mathf.Lerp(
            thirdPersonAimCamera.Lens.FieldOfView,
            target,
            fovChangeSpeed * Time.deltaTime
        );
    }
}