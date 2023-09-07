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
    private bool wantsToMove;
    private float rotationSpeed;
    private float acceleration;
    private float maxSpeed;

    public float walkingRotSpeed;
    public float walkingAccel;
    public float walkingMaxSpd;

    InputAction moveAction;

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
        sM.SetInitialState(idleState);
    }

    private void Start()
    {
        moveAction = InputManager.Instance.GetInputAction("Move");
        moveAction.Enable();

        sM.AddTransition(idleState, () => wantsToMove, walkingState);
        sM.AddTransition(walkingState, () => !wantsToMove, idleState);
    }

    private void Update()
    {
        wantsToMove = moveAction.ReadValue<Vector2>() != Vector2.zero;
    }

    public void SetMovementProperties(float rotSpeed, float accel, float maxSpeed)
    {
        rotationSpeed = rotSpeed;
        acceleration = accel;
        this.maxSpeed = maxSpeed;
    }

    public void MoveWithInput()
    {
        Move(moveAction.ReadValue<Vector2>());
    }

    private void Move(Vector2 dir)
    {
        if (dir != Vector2.zero)
        {
            Vector3 dir3 = new Vector3(dir.x, 0, dir.y);
            SmoothRotateTowards(dir3);
            Accelerate(dir3, acceleration * dir.magnitude, maxSpeed);
        }
    }

    private void Accelerate(Vector3 dir, float acceleration, float maxSpeed)
    {
        rb.velocity += dir * acceleration * Time.deltaTime;

        float mag = rb.velocity.magnitude;
        if (mag > maxSpeed)
        {
            rb.velocity *= maxSpeed / mag ;
        }
    }

    private void SmoothRotateTowards(Vector3 dir)
    {
        Quaternion targetRot = Quaternion.LookRotation(dir);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.deltaTime));
    }
}
