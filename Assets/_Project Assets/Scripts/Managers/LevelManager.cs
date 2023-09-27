using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AgentSpawner), typeof(ResourceSpawner))]
public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private ColliderEvent playerHitsWater;
    [SerializeField] private ColliderEvent workerHitsWater;
    [SerializeField] private IntEvent currentWeightUpdate;
    [SerializeField] private VoidEvent gameLoss;

    [SerializeField] private float playerRespawnTime;
    [SerializeField] private float entityHitsWaterProcessingDelay;
    public float overLimitTime;
    private float workerSpawnTimer;
    private float resourceSpawnTimer;

    public Transform iceBlockParent;
    public Transform resourceParent;
    public Transform agentParent;
    public Transform agentPool;

    private AgentSpawner agentSpawner;
    [SerializeField] private float startingWorkerSpawnFrequency;
    [SerializeField] private float spawnFrequencyGrowthRate;
    [SerializeField] private int startingWorkerSpawnCountMin;
    [SerializeField] private int startingWorkerSpawnCountMax;
    private ResourceSpawner resourceSpawner;
    [SerializeField] private float startingResourceSpawnFrequency;
    [SerializeField] private float resourceSpawnFrequencyGrowthRate;
    [SerializeField] private int startingResourcesMin;
    [SerializeField] private int startingResourcesMax;

    protected override void Awake()
    {
        base.Awake();

        agentSpawner = GetComponent<AgentSpawner>();
        resourceSpawner = GetComponent<ResourceSpawner>();
    }

    private void OnEnable()
    {
        playerHitsWater.Register(OnPlayerHitsWater);
        workerHitsWater.Register(OnWorkerHitsWater);
        gameLoss.Register(OnGameLoss);
    }

    private void OnDisable()
    {
        playerHitsWater.Unregister(OnPlayerHitsWater);
        workerHitsWater.Unregister(OnWorkerHitsWater);
        gameLoss.Unregister(OnGameLoss);
    }

    private void Update()
    {
        workerSpawnTimer -= Time.deltaTime;
        if (workerSpawnTimer <= 0)
        {
            agentSpawner.SpawnWorkers(UnityEngine.Random.Range(startingWorkerSpawnCountMin, startingWorkerSpawnCountMax + 1));
            startingWorkerSpawnFrequency *= spawnFrequencyGrowthRate;
            workerSpawnTimer = startingWorkerSpawnFrequency;
        }

        resourceSpawnTimer -= Time.deltaTime;
        if (resourceSpawnTimer <= 0)
        {
            if (resourceParent.childCount < IslandGrid.Instance.GetMaxAllowedResources())
            {
                resourceSpawner.SpawnResource(UnityEngine.Random.Range(startingResourcesMin, startingResourcesMax));
            }
            startingResourceSpawnFrequency *= resourceSpawnFrequencyGrowthRate;
            resourceSpawnTimer = startingResourceSpawnFrequency;
        }
    }

    private void OnPlayerHitsWater()
    {
        StartCoroutine(Utility.DelayedAction(() =>
        {
            var playerGO = PlayerController.Instance.gameObject;
            StartCoroutine(Utility.DelayedAction(() =>
            {
                var respawnPosition = IslandGrid.Instance.root.transform.position;
                respawnPosition += new Vector3(0, IslandGrid.Instance.root.GetComponent<GridNode>().col.bounds.extents.y + PlayerController.Instance.rend.bounds.extents.y, 0);
                playerGO.transform.position = respawnPosition;
                playerGO.SetActive(true);
            }, playerRespawnTime));
            playerGO.SetActive(false);
        }, entityHitsWaterProcessingDelay));
    }

    private void OnWorkerHitsWater(Collider col)
    {
        if (col.GetComponentInParent<WorkerController>().spawnComplete)
        {
            col.attachedRigidbody.gameObject.transform.SetParent(agentPool.transform, true);
            currentWeightUpdate.Raise(agentParent.childCount);

            StartCoroutine(Utility.DelayedAction(() =>
            {
                col.attachedRigidbody.gameObject.SetActive(false);
            }, entityHitsWaterProcessingDelay));
        }
    }

    private void OnGameLoss()
    {
        LevelUIController.Instance.ShowGameOverUI("THE ISLAND SUNK");
        iceBlockParent.gameObject.SetActive(false);
        resourceParent.gameObject.SetActive(false);
        agentParent.gameObject.SetActive(false);
        IslandWeightController.Instance.SinkIsland();
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
