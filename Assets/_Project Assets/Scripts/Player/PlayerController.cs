using System;
using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(StateMachine), typeof(Rigidbody))]
public class PlayerController : Singleton<PlayerController>
{
    public enum OrderType { Follow, Work };
    public OrderType currentOrder;

    private Rigidbody rb;
    public Renderer rend;
    private StateMachine sM;

    private PlayerIdle idleState;
    private PlayerWalking walkingState;
    private PlayerChargingDash chargingDashState;
    private PlayerDashing dashingState;
    private PlayerResting restingState;
    private PlayerOrdering orderingState;
    private PlayerFalling fallingState;
    private bool wantsToMove;
    private float rotationSpeed;
    private float acceleration;
    private float maxSpeed;
    private bool wantsToDash;
    private float dashTimer;
    private bool dashOver;
    private float bellyCurrentChargeTime;
    private float restTime;
    private bool wantsToOrder;

    [Header("Walking Params")]
    public float walkingRotSpeed;
    public float walkingAccel;
    public float walkingMaxSpd;
    public float groundedRaycastRadiusShrinkage;

    [Header("Belly Dash Params")]
    [SerializeField] private BoxCollider pushCollider;
    public float bellyMinChargeTime;
    public float bellyMaxChargeTime;
    public float chargingRotSpeed;
    public float bellyDashMinDuration;
    public float bellyDashMaxDuration;
    public float bellyDashMinForce;
    public float bellyDashMaxForce;
    public float afterDashRestTime;
    public float afterDashRestDecelerationTarget;

    [Header("Order Params")]
    public GameObject orderCircle;
    public float orderCircleStartRadius;
    public float orderCircleEndRadius;
    public float circleExpansionRate;

    InputAction moveAction;
    InputAction bellyDashAction;
    InputAction followOrderAction;
    InputAction workOrderAction;

    #region Unity Lifecycle
    protected override void Awake()
    {
        base.Awake();
        
        rb = GetComponent<Rigidbody>();
        sM = GetComponent<StateMachine>();

        idleState = new PlayerIdle(this);
        walkingState = new PlayerWalking(this);
        chargingDashState = new PlayerChargingDash(this);
        dashingState = new PlayerDashing(this);
        restingState = new PlayerResting(this);
        orderingState = new PlayerOrdering(this);
        fallingState = new PlayerFalling(this);

        sM.SetInitialState(idleState);
    }

    private void Start()
    {
        moveAction = InputManager.Instance.GetInputAction("Move");
        bellyDashAction = InputManager.Instance.GetInputAction("Belly Dash");
        followOrderAction = InputManager.Instance.GetInputAction("Follow Order");
        workOrderAction = InputManager.Instance.GetInputAction("Work Order");

        moveAction.Enable();
        bellyDashAction.Enable();
        followOrderAction.Enable();
        workOrderAction.Enable();

        sM.AddTransition(idleState, () => wantsToMove, walkingState);
        sM.AddTransition(walkingState, () => !wantsToMove, idleState);
        sM.AddTransition(idleState, () => wantsToDash, chargingDashState);
        sM.AddTransition(walkingState, () => wantsToDash, chargingDashState);
        sM.AddTransition(chargingDashState, () => !wantsToDash, dashingState);
        sM.AddTransition(dashingState, () => dashOver, restingState);
        sM.AddTransition(restingState, () => restTime <= 0, idleState);
        sM.AddTransition(idleState, () => wantsToOrder, orderingState);
        sM.AddTransition(walkingState, () => wantsToOrder, orderingState);
        sM.AddTransition(orderingState, () => !wantsToOrder, idleState);
        sM.AddTransition(fallingState, IsGrounded, idleState);

        sM.AddGlobalTransition(() => !IsGrounded(), fallingState);
    }

    private void Update()
    {
        wantsToMove = moveAction.ReadValue<Vector2>() != Vector2.zero;
        wantsToDash = bellyDashAction.ReadValue<float>() == 1f;

        if (followOrderAction.ReadValue<float>() == 1f)
        {
            wantsToOrder = true;
            currentOrder = OrderType.Follow;
        }
        else if (workOrderAction.ReadValue<float>() == 1f)
        {
            wantsToOrder = true;
            currentOrder = OrderType.Work;
        }
        else
        {
            wantsToOrder = false;
        }
    }

    #endregion

    #region Private Functions
    private void Move(Vector2 dir, bool relativeToCam = true)
    {
        if (dir == Vector2.zero)
        {
            return;
        }

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

    private bool IsGrounded()
    {
        var raycastLength = rend.bounds.extents.y + 0.1f;
        var layerMask = LayerMask.GetMask("Platform");
        return Physics.Raycast(new Vector3(transform.position.x + (rend.bounds.extents.x - groundedRaycastRadiusShrinkage), transform.position.y, transform.position.z), Vector3.down, raycastLength, layerMask) ||
            Physics.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z + (rend.bounds.extents.z - groundedRaycastRadiusShrinkage)), Vector3.down, raycastLength, layerMask) ||
            Physics.Raycast(new Vector3(transform.position.x - (rend.bounds.extents.x - groundedRaycastRadiusShrinkage), transform.position.y, transform.position.z), Vector3.down, raycastLength, layerMask) ||
            Physics.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z - (rend.bounds.extents.z - groundedRaycastRadiusShrinkage)), Vector3.down, raycastLength, layerMask);
    }

    #endregion

    #region Movement API

    public void SetMovementProperties(float rotSpeed, float accel, float maxSpeed)
    {
        rotationSpeed = rotSpeed;
        acceleration = accel;
        this.maxSpeed = maxSpeed;
    }

    public void MoveWithInput(Vector2? directionOverride = null)
    {
        if (acceleration == 0f && rotationSpeed == 0f)
        {
            return;
        }

        if (directionOverride != null)
        {
            Move((Vector2)directionOverride, false);
        }
        else
        {
            Vector2 dir = moveAction.ReadValue<Vector2>();
            Move(dir);
        }
    }

    #endregion

    #region Dash API
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
        pushCollider.enabled = true;
        dashOver = false;
        rb.constraints = rb.constraints | RigidbodyConstraints.FreezeRotationY;
        float chargeMultiplier = (Mathf.Clamp(bellyCurrentChargeTime, bellyMinChargeTime, bellyMaxChargeTime) - bellyMinChargeTime) / (bellyMaxChargeTime - bellyMinChargeTime);
        var dashForce = Mathf.Lerp(bellyDashMinForce, bellyDashMaxForce, chargeMultiplier);
        dashTimer = Mathf.Lerp(bellyDashMinDuration, bellyDashMaxDuration, chargeMultiplier);
        SetMovementProperties(0f, 0f, 0f);
        rb.AddForce(transform.forward * dashForce, ForceMode.VelocityChange);
    }

    public void DashTick()
    {
        if (dashTimer <= 0)
        {
            dashOver = true;
            rb.constraints = rb.constraints & ~RigidbodyConstraints.FreezeRotationY;
            pushCollider.enabled = false;
            return;
        }

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

    public bool PollMouseDown()
    {
        return Mouse.current.leftButton.isPressed;
    }
    #endregion

    #region Order API
    public void Order(float radius, OrderType type)
    {
        var cols = Physics.OverlapSphere(orderCircle.transform.position, radius, LayerMask.GetMask("Worker"));
        foreach (var worker in cols)
        {
            if (type == OrderType.Follow)
            {
                worker.GetComponentInParent<WorkerController>().FollowPlayer();
            }
            else if (type == OrderType.Work)
            {
                worker.GetComponentInParent<WorkerController>().SeekResource();
            }
        }
    }
    #endregion
}
