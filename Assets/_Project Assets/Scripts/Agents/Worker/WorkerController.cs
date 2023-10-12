using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.AI;

public class WorkerController : AgentController
{
    [SerializeField] private GameObject hat;
    [SerializeField] private GameObject pickaxe;

    public IntEvent currentWeightUpdate;

    private WorkerSpawn spawnState;
    private WorkerIdle idleState;
    private WorkerWander wanderState;
    private WorkerFollow followState;
    private WorkerSeekResource seekResourceState;
    private WorkerHarvestResource harvestState;

    public float minWanderStartTime;
    public float maxWanderStartTime;
    public float maxWanderRange;
    public float navUpdateFrequency;
    public float maxSeekDistance;
    public float harvestDuration;
    public float maxHarvestDistance;
    public float harvestRate;
    public Resource TargetResource { get; set; }

    private bool wantsToWander;
    private bool wantsToFollow;
    private bool wantsToSeekResource;
    private bool wantsToHarvestResource;

    protected override void Awake()
    {
        base.Awake();

        spawnState = new WorkerSpawn(this);
        idleState = new WorkerIdle(this);
        wanderState = new WorkerWander(this);
        followState = new WorkerFollow(this);
        seekResourceState = new WorkerSeekResource(this);
        harvestState = new WorkerHarvestResource(this);
        
        sM.SetInitialState(spawnState);
    }

    private void Start()
    {
        sM.AddTransition(spawnState, IsGrounded, idleState);
        sM.AddTransition(idleState, () => wantsToFollow, followState);
        sM.AddTransition(idleState, () => wantsToSeekResource, seekResourceState);
        sM.AddTransition(idleState, () => wantsToWander, wanderState);
        sM.AddTransition(wanderState, () => wantsToFollow, followState);
        sM.AddTransition(wanderState, () => wantsToSeekResource, seekResourceState);
        sM.AddTransition(wanderState, () => !wantsToWander, idleState);
        sM.AddTransition(followState, () => wantsToSeekResource, seekResourceState);
        sM.AddTransition(seekResourceState, () => !wantsToSeekResource && !wantsToHarvestResource, idleState);
        sM.AddTransition(seekResourceState, () => wantsToFollow, followState);
        sM.AddTransition(seekResourceState, () => wantsToHarvestResource, harvestState);
        sM.AddTransition(harvestState, () => !wantsToHarvestResource, idleState);
        sM.AddTransition(harvestState, () => wantsToFollow, followState);
        sM.AddTransition(harvestState, () => wantsToSeekResource, seekResourceState);
    }


    public void FollowPlayer()
    {
        wantsToSeekResource = false;
        wantsToFollow = true;
        wantsToHarvestResource = false;
        wantsToWander = false;
    }

    public void SeekResource()
    {
        if (sM.GetCurrentState() is WorkerHarvestResource)
        {
            return;
        }

        wantsToFollow = false;
        wantsToSeekResource = true;
        wantsToHarvestResource = false;
        wantsToWander = false;
    }

    public void Wander()
    {
        wantsToWander = true;
    }

    public void WanderEnd()
    {
        wantsToWander = false;
    }

    public override void TogglePhysics(bool isEnabled, float resumeNavigationTime = 0.6f)
    {
        base.TogglePhysics(isEnabled, resumeNavigationTime);

        if (isEnabled)
        {
            WanderEnd();
        }
        
        if (!isEnabled && TargetResource != null)
        {
            SetNavDestination(TargetResource.transform.position);
        }
    }


    public void ResourceNotFound()
    {
        wantsToSeekResource = false;
        wantsToHarvestResource = false;
    }


    public void ToggleHarvestGear(bool isEnabled)
    {
        hat.SetActive(isEnabled);
        pickaxe.SetActive(isEnabled);
    }

    public void LocatedResource()
    {
        wantsToHarvestResource = true;
        wantsToSeekResource = false;
    }

    public void HarvestInterrupted()
    {
        wantsToHarvestResource = false;
        wantsToSeekResource = false;
    }
}
