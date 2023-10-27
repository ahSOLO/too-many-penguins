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
    [SerializeField] private ColliderEvent seaLionHitsWater;
    [SerializeField] private IntEvent currentWeightUpdate;
    [SerializeField] private VoidEvent gameLoss;
    [SerializeField] private VoidEvent gameWon;
    [SerializeField] private float gameTimeRemaining;
    private float startingGameTime;

    [SerializeField] private float playerRespawnTime;
    [SerializeField] private float entityHitsWaterProcessingDelay;
    public float overLimitTime;
    private float workerSpawnTimer;
    private float seaLionSpawnTimer;
    private float resourceSpawnTimer;
    [SerializeField] private float musicParamUpdateFrequency;
    private float musicParamUpdateTimer;

    public Transform iceBlockParent;
    public Transform resourceParent;
    public Transform agentParent;
    public Transform workerParent;
    public Transform seaLionParent;
    public Transform agentPool;

    private AgentSpawner agentSpawner;
    [SerializeField] private float startingWorkerSpawnFrequency;
    [SerializeField] private float workerSpawnFrequencyGrowthRate;
    [SerializeField] private int startingWorkerSpawnCountMin;
    [SerializeField] private int startingWorkerSpawnCountMax;
    [SerializeField] private float seaLionSpawnInitialDelay;
    [SerializeField] private float startingSeaLionSpawnFrequency;
    [SerializeField] private float seaLionSpawnFrequencyGrowthRate;
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

        seaLionSpawnTimer = seaLionSpawnInitialDelay;

        startingGameTime = gameTimeRemaining;
    }

    private void Start()
    {
        GameManager.Instance.gameIsPausable = true;
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("ElapsedTime", 0);
        MusicController.Instance.Play();
    }

    private void OnEnable()
    {
        playerHitsWater.Register(OnPlayerHitsWater);
        workerHitsWater.Register(OnWorkerHitsWater);
        seaLionHitsWater.Register(OnSeaLionHitsWater);
        gameLoss.Register(OnGameLoss);
        gameWon.Register(OnGameWon);
    }

    private void OnDisable()
    {
        playerHitsWater.Unregister(OnPlayerHitsWater);
        workerHitsWater.Unregister(OnWorkerHitsWater);
        seaLionHitsWater.Unregister(OnSeaLionHitsWater);
        gameLoss.Unregister(OnGameLoss);
        gameWon.Register(OnGameWon);
    }

    private void Update()
    {
        workerSpawnTimer -= Time.deltaTime;
        if (!levelCompleted && workerSpawnTimer <= 0)
        {
            agentSpawner.SpawnWorkers(UnityEngine.Random.Range(startingWorkerSpawnCountMin, startingWorkerSpawnCountMax + 1));
            startingWorkerSpawnFrequency *= workerSpawnFrequencyGrowthRate;
            workerSpawnTimer = startingWorkerSpawnFrequency;
        }

        seaLionSpawnTimer -= Time.deltaTime;
        if (!levelCompleted && seaLionSpawnTimer <= 0)
        {
            agentSpawner.SpawnSeaLion();
            startingSeaLionSpawnFrequency *= seaLionSpawnFrequencyGrowthRate;
            seaLionSpawnTimer = startingSeaLionSpawnFrequency;
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

        musicParamUpdateTimer -= Time.deltaTime;
        if (!levelCompleted && musicParamUpdateTimer <= 0)
        {
            MusicController.Instance.SetThreat(Mathf.Min(100f, IslandWeightController.Instance.GetCurrentWeightPercentage() * 100));
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("ElapsedTime", (1 - gameTimeRemaining / startingGameTime) * 100);
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
        SFXController.Instance.PlayInstance(SFXController.Instance.leaderFallsWater, ref SFXController.Instance.leaderFallsWaterInstance, PlayerController.Instance.transform);

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

                SFXController.Instance.StopInstance(SFXController.Instance.leaderFallsWaterInstance);
                SFXController.Instance.PlayOneShot(SFXController.Instance.leaderRespawn, respawnPosition);
            }, playerRespawnTime));
            playerGO.SetActive(false);
        }, entityHitsWaterProcessingDelay));
    }

    private void OnWorkerHitsWater(Collider col)
    {
        if (col.GetComponentInParent<WorkerController>().spawnComplete)
        {
            SFXController.Instance.PlayOneShot(SFXController.Instance.charHitsWater, PlayerController.Instance.transform.position);

            col.attachedRigidbody.gameObject.transform.SetParent(agentPool.transform, true);
            currentWeightUpdate.Raise(Mathf.RoundToInt(workerParent.childCount * IslandWeightController.Instance.penguinWeight + seaLionParent.childCount * IslandWeightController.Instance.seaLionWeight));

            StartCoroutine(Utility.DelayedAction(() =>
            {
                Destroy(col.attachedRigidbody.gameObject);
            }, entityHitsWaterProcessingDelay));
        }
    }

    private void OnSeaLionHitsWater(Collider col)
    {
        if (col.GetComponentInParent<SeaLionController>().spawnComplete)
        {
            SFXController.Instance.PlayOneShot(SFXController.Instance.charHitsWater, PlayerController.Instance.transform.position);

            col.attachedRigidbody.gameObject.transform.SetParent(agentPool.transform, true);
            currentWeightUpdate.Raise(Mathf.RoundToInt(workerParent.childCount * IslandWeightController.Instance.penguinWeight + seaLionParent.childCount * IslandWeightController.Instance.seaLionWeight));

            StartCoroutine(Utility.DelayedAction(() =>
            {
                Destroy(col.attachedRigidbody.gameObject);
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

        SFXController.Instance.PlayOneShot(SFXController.Instance.gameOverLose, Vector3.zero);
    }

    private void OnGameWon()
    {
        if (levelCompleted)
        {
            return;
        }

        CompleteLevel();
        LevelUIController.Instance.ShowGameOverUI("YOU WIN!");

        SFXController.Instance.PlayOneShot(SFXController.Instance.gameOverWin, Vector3.zero);
    }

    private void CompleteLevel()
    {
        levelCompleted = true;
        InputManager.Instance.TogglePlayerInput(false);
        var endingWeightScore = Mathf.RoundToInt(workerParent.childCount * IslandWeightController.Instance.penguinWeight * 100f + seaLionParent.childCount * IslandWeightController.Instance.seaLionWeight * 100f);
        var averageWeightScore = IslandWeightController.Instance.CalculateAverageWeight() * 100;
        var fallingPenaltyScore = timesPlayerFell * Mathf.RoundToInt(IslandWeightController.Instance.penguinWeight * -100);

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
