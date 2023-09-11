using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(StateMachine), typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    private Rigidbody rb;
    private StateMachine sM;
    private Collider col;

    private PlayerIdle idleState;
    private PlayerWalking walkingState;
    private PlayerChargingDash chargingDashState;
    private PlayerDashing dashingState;
    private PlayerResting restingState;
    private bool wantsToMove;
    private float rotationSpeed;
    private float acceleration;
    private float maxSpeed;
    private bool wantsToDash;
    private float dashTimer;
    private bool dashOver;
    private float bellyCurrentChargeTime;
    private float restTime;

    [Header("Walking Params")]
    public float walkingRotSpeed;
    public float walkingAccel;
    public float walkingMaxSpd;

    [Header("Belly Dash Params")]
    public float bellyMinChargeTime;
    public float bellyMaxChargeTime;
    public float bellyDashAccel;
    public float bellyDashMinDuration;
    public float bellyDashMaxDuration;
    public float bellyDashMinSpeed;
    public float bellyDashMaxSpeed;
    public float afterDashRestTime;
    public float afterDashRestDecelerationTarget;

    InputAction moveAction;
    InputAction bellyDashAction;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            enabled = false;
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        
        rb = GetComponent<Rigidbody>();
        sM = GetComponent<StateMachine>();
        col = GetComponentInChildren<Collider>();
        idleState = new PlayerIdle(this);
        walkingState = new PlayerWalking(this);
        chargingDashState = new PlayerChargingDash(this);
        dashingState = new PlayerDashing(this);
        restingState = new PlayerResting(this);
        sM.SetInitialState(idleState);
    }

    private void Start()
    {
        moveAction = InputManager.Instance.GetInputAction("Move");
        moveAction.Enable();

        bellyDashAction = InputManager.Instance.GetInputAction("Belly Dash");
        bellyDashAction.Enable();

        sM.AddTransition(idleState, () => wantsToMove, walkingState);
        sM.AddTransition(walkingState, () => !wantsToMove, idleState);
        sM.AddTransition(idleState, () => wantsToDash, chargingDashState);
        sM.AddTransition(walkingState, () => wantsToDash, chargingDashState);
        sM.AddTransition(chargingDashState, () => !wantsToDash, dashingState);
        sM.AddTransition(dashingState, () => dashOver, restingState);
        sM.AddTransition(restingState, () => restTime <= 0, idleState);
    }

    private void Update()
    {
        wantsToMove = moveAction.ReadValue<Vector2>() != Vector2.zero;
        wantsToDash = bellyDashAction.ReadValue<float>() == 1f;
    }

    public void SetMovementProperties(float rotSpeed, float accel, float maxSpeed)
    {
        rotationSpeed = rotSpeed;
        acceleration = accel;
        this.maxSpeed = maxSpeed;
    }

    public void MoveWithInput()
    {
        var dir = moveAction.ReadValue<Vector2>();
        if (dir != Vector2.zero)
        {
            Move(dir);
        }
    }

    public void ResetDashChargeTime()
    {
        bellyCurrentChargeTime = 0f;
    }

    public void DashCharge()
    {
        bellyCurrentChargeTime += Time.deltaTime;
    }

    public void DashStart()
    {
        dashOver = false;
        rb.constraints = rb.constraints | RigidbodyConstraints.FreezeRotationY;
        float chargeMultiplier = (Mathf.Clamp(bellyCurrentChargeTime, bellyMinChargeTime, bellyMaxChargeTime) - bellyMinChargeTime) / (bellyMaxChargeTime - bellyMinChargeTime);
        dashTimer = Mathf.Lerp(bellyDashMinDuration, bellyDashMaxDuration, chargeMultiplier);
        SetMovementProperties(0f, bellyDashAccel, Mathf.Lerp(bellyDashMinSpeed, bellyDashMaxSpeed, chargeMultiplier));
    }

    public void DashTick()
    {
        if (dashTimer <= 0)
        {
            dashOver = true;
            rb.constraints = rb.constraints & ~RigidbodyConstraints.FreezeRotationY;
            return;
        }
        
        Move(new Vector2(transform.forward.x, transform.forward.z), false);
        dashTimer -= Time.deltaTime;
    }

    public void RestStart()
    {
        restTime = afterDashRestTime;
    }

    public void RestTick()
    {
        restTime -= Time.deltaTime;
        Decelerate(afterDashRestDecelerationTarget);
    }

    private void Move(Vector2 dir, bool relativeToCam = true)
    {
        Vector3 dir3 = relativeToCam ? Quaternion.Euler(new Vector3(0f, Camera.main.transform.rotation.eulerAngles.y, 0f)) * new Vector3(dir.x, 0, dir.y) : new Vector3(dir.x, 0, dir.y);
        SmoothRotateTowards(dir3);
        Accelerate(dir3, acceleration * dir.magnitude, maxSpeed);
    }

    private void Accelerate(Vector3 dir, float acceleration, float maxSpeed)
    {
        rb.velocity += dir * acceleration * Time.deltaTime;

        float mag = new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude;
        if (mag > maxSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.x * maxSpeed / mag, rb.velocity.y, rb.velocity.z * maxSpeed / mag);
        }
    }

    private void Decelerate(float deceleration)
    {
        var target = rb.velocity * deceleration;
        rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(target.x, rb.velocity.y, target.z), Time.deltaTime);
    }

    private void SmoothRotateTowards(Vector3 dir)
    {
        Quaternion targetRot = Quaternion.LookRotation(dir);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.deltaTime));
    }
}
