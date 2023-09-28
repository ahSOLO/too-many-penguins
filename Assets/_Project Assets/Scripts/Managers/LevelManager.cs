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
    [SerializeField] private VoidEvent gameWon;
    [SerializeField] private float gameTimeRemaining;

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

    private int timesPlayerFell;
    private bool levelCompleted = false;

    protected override void Awake()
    {
        base.Awake();

        agentSpawner = GetComponent<AgentSpawner>();
        resourceSpawner = GetComponent<ResourceSpawner>();
    }

    private void Start()
    {
        GameManager.Instance.gameIsPausable = true;
    }

    private void OnEnable()
    {
        playerHitsWater.Register(OnPlayerHitsWater);
        workerHitsWater.Register(OnWorkerHitsWater);
        gameLoss.Register(OnGameLoss);
        gameWon.Register(OnGameWon);
    }

    private void OnDisable()
    {
        playerHitsWater.Unregister(OnPlayerHitsWater);
        workerHitsWater.Unregister(OnWorkerHitsWater);
        gameLoss.Unregister(OnGameLoss);
        gameWon.Register(OnGameWon);
    }

    private void Update()
    {
        workerSpawnTimer -= Time.deltaTime;
        if (!levelCompleted && workerSpawnTimer <= 0)
        {
            agentSpawner.SpawnWorkers(UnityEngine.Random.Range(startingWorkerSpawnCountMin, startingWorkerSpawnCountMax + 1));
            startingWorkerSpawnFrequency *= spawnFrequencyGrowthRate;
            workerSpawnTimer = startingWorkerSpawnFrequency;
        }

        resourceSpawnTimer -= Time.deltaTime;
        if (!levelCompleted && resourceSpawnTimer <= 0)
        {
            if (resourceParent.childCount < IslandGrid.Instance.GetMaxAllowedResources())
            {
                resourceSpawner.SpawnResource(UnityEngine.Random.Range(startingResourcesMin, startingResourcesMax));
            }
            startingResourceSpawnFrequency *= resourceSpawnFrequencyGrowthRate;
            resourceSpawnTimer = startingResourceSpawnFrequency;
        }

        if (!levelCompleted)
        {
            gameTimeRemaining -= Time.deltaTime;
            LevelUIController.Instance.PresentTime(gameTimeRemaining);

            if (gameTimeRemaining <= 0)
            {
                gameWon.Raise();
            }
        }
    }

    private void OnPlayerHitsWater()
    {
        timesPlayerFell++;
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
        if (levelCompleted)
        {
            return;
        }

        CompleteLevel();
        LevelUIController.Instance.ShowGameOverUI("THE ISLAND SUNK");
        iceBlockParent.gameObject.SetActive(false);
        resourceParent.gameObject.SetActive(false);
        agentParent.gameObject.SetActive(false);
        IslandWeightController.Instance.SinkIsland();
    }

    private void OnGameWon()
    {
        if (levelCompleted)
        {
            return;
        }

        CompleteLevel();
        LevelUIController.Instance.ShowGameOverUI("YOU WIN!");
    }

    private void CompleteLevel()
    {
        levelCompleted = true;
        InputManager.Instance.TogglePlayerInput(false);
        var endingWeightScore = agentParent.childCount * 100;
        var averageWeightScore = IslandWeightController.Instance.CalculateAverageWeight() * 100;
        var fallingPenaltyScore = timesPlayerFell * -100;

        LevelUIController.Instance.AddScoreRow("Final Weight", endingWeightScore);
        LevelUIController.Instance.AddScoreRow("Average Weight", averageWeightScore);
        LevelUIController.Instance.AddScoreRow("Falling Penalty", fallingPenaltyScore);
        LevelUIController.Instance.AddScoreRow("Total Score", endingWeightScore + averageWeightScore + fallingPenaltyScore);

        LevelUIController.Instance.PresentScore(3f);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
