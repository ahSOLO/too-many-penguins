using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class IslandGrid : Singleton<IslandGrid>
{
    private enum SpawnDirection { Left, Top, Right, Bottom };

    [SerializeField] private ColliderEvent iceBlockHitsWater;
    [SerializeField] private IntEvent maxWeightUpdate;
    [SerializeField] private float tileWeightCapacity;
    [SerializeField] private float newPlatformSpawnDelay;
    [SerializeField] private float newPlatformAnimateDistance;
    [SerializeField] private float newPlatformAnimateSpeed;
    [SerializeField] private int spawnNewDecorationOdds;
    [SerializeField] private int initialDecorationSpawningOdds;

    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private List<GridNode> nodes = new List<GridNode>();
    public GridNode root;
    private int startingLayers;
    private float cellWidth = 3f;
    private float cellLength = 3f;
    private float overlapSphereSize = 12f;
    private float approxIslandRadius;

    private int navMeshLastBakedNodeCount;
    private NavMeshSurface navMesh;

    #region Unity Lifecycle

    protected override void Awake()
    {
        base.Awake();

        navMesh = GetComponent<NavMeshSurface>();
        
        root = Instantiate(platformPrefab, gameObject.transform).GetComponent<GridNode>();
        nodes.Add(root);
    }

    private void Start()
    {
        GenerateStartingIsland(2);
    }

    private void OnEnable()
    {
        iceBlockHitsWater.Register(OnIceBlockHitsWater);
    }

    private void Update()
    {
        if (navMeshLastBakedNodeCount != nodes.Count)
        {
            navMesh.BuildNavMesh();
            navMeshLastBakedNodeCount = nodes.Count;
        }
    }

    private void OnDisable()
    {
        iceBlockHitsWater.Unregister(OnIceBlockHitsWater);
    }

    #endregion
    #region Private Functions

    private void SpawnOrganicProtrusions(int number = 1)
    {
        var randomDirection = Utility.RandomFromEnum<SpawnDirection>();

        for (int i = 0; i < number; i++)
        {
            var cur = root;
            switch (randomDirection)
            {
                case SpawnDirection.Left:
                    var startPointL = cur.transform.position + new Vector3(-cellWidth * (startingLayers + 1), 0f, 0f);
                    FillSide(ref startPointL, startingLayers / 2 + 1, UnityEngine.Random.Range(0, 2) == 1 ? SpawnDirection.Top : SpawnDirection.Bottom);
                    break;
                case SpawnDirection.Top:
                    var startPointT = cur.transform.position + new Vector3(0f, 0f, cellLength * (startingLayers + 1));
                    FillSide(ref startPointT, startingLayers / 2 + 1, UnityEngine.Random.Range(0, 2) == 1 ? SpawnDirection.Left : SpawnDirection.Right);
                    break;
                case SpawnDirection.Right:
                    var startPointR = cur.transform.position + new Vector3(cellWidth * (startingLayers + 1), 0f, 0f);
                    FillSide(ref startPointR, startingLayers / 2 + 1, UnityEngine.Random.Range(0, 2) == 1 ? SpawnDirection.Top : SpawnDirection.Bottom);
                    break;
                case SpawnDirection.Bottom:
                    var startPointB = cur.transform.position + new Vector3(0f, 0f, -cellLength * (startingLayers + 1));
                    FillSide(ref startPointB, startingLayers / 2 + 1, UnityEngine.Random.Range(0, 2) == 1 ? SpawnDirection.Left : SpawnDirection.Right);
                    break;
                default:
                    break;
            }
            randomDirection = (SpawnDirection)(((int)randomDirection + 1) % Enum.GetValues(typeof(SpawnDirection)).Length);
        }
    }

    private void GenerateStartingIsland(int numberOfLayers)
    {
        approxIslandRadius = (numberOfLayers + 1f) * (cellWidth + cellLength) / 2f;
        for (int i = 0; i < numberOfLayers; i++)
        {
            AddLayer();
        }
        SpawnOrganicProtrusions(3);
        foreach (GridNode node in nodes)
        {
            node.SearchEmptySides(cellWidth, cellLength);
        }
        foreach (GridNode node in nodes)
        {
            node.AssignMesh();
            if (node.SidesFilled() >= 3 && node.SidesDecorated() <= 2)
            {
                var randomNumber = UnityEngine.Random.Range(0, initialDecorationSpawningOdds);
                if (randomNumber == initialDecorationSpawningOdds - 1)
                {
                    node.Decorate();
                }
            }
        }
        UpdateMaxWeight();
    }

    private void AddLayer()
    {
        startingLayers++;

        // Spawn on top right
        var nextSpawnPoint = transform.position + new Vector3(startingLayers * cellWidth, 0, startingLayers * cellLength);

        // Fill in right side;
        FillSide(ref nextSpawnPoint, startingLayers * 2, SpawnDirection.Bottom);

        // Fill in bottom side
        FillSide(ref nextSpawnPoint, startingLayers * 2, SpawnDirection.Left);

        // Fill in left side
        FillSide(ref nextSpawnPoint, startingLayers * 2, SpawnDirection.Top);

        // Fill in top side
        FillSide(ref nextSpawnPoint, startingLayers * 2, SpawnDirection.Right);
    }

    private void FillSide(ref Vector3 nextSpawnPoint, int length, SpawnDirection spawnDirection)
    {
        for (int i = 0; i < length; i++)
        {
            AddNode(nextSpawnPoint);

            switch (spawnDirection)
            {
                case SpawnDirection.Left:
                    nextSpawnPoint += new Vector3(-cellWidth, 0, 0);
                    break;
                case SpawnDirection.Top:
                    nextSpawnPoint += new Vector3(0, 0, cellLength);
                    break;
                case SpawnDirection.Right:
                    nextSpawnPoint += new Vector3(cellWidth, 0, 0);
                    break;
                case SpawnDirection.Bottom:
                    nextSpawnPoint += new Vector3(0, 0, -cellLength);
                    break;
            }
        }
    }

    private GridNode AddNode(Vector3 spawnPoint, bool remeasureRadius = false)
    {
        var newNode = Instantiate(platformPrefab, spawnPoint, Quaternion.identity, gameObject.transform).GetComponent<GridNode>();
        newNode.transform.localPosition = new Vector3(newNode.transform.localPosition.x, 0f, newNode.transform.localPosition.z);
        nodes.Add(newNode);

        if (remeasureRadius)
        {
            approxIslandRadius = Mathf.Max(approxIslandRadius, (newNode.transform.position - root.transform.position).magnitude);
        }
        return newNode;
    }

    private void OnIceBlockHitsWater(Collider other)
    {
        SFXController.Instance.PlayOneShot(SFXController.Instance.blockFallsWater, other.transform.position);
        
        var hits = Physics.OverlapSphere(other.transform.position, overlapSphereSize, LayerMask.GetMask("Platform"));
        if (hits.Length == 0)
        {
            return;
        }
        var closest = hits[0];
        var closestDist = (other.transform.position - closest.transform.position).sqrMagnitude;
        for (int i = 1; i < hits.Length; i++)
        {
            var curDist = (other.transform.position - hits[i].transform.position).sqrMagnitude;
            if ( curDist < closestDist)
            {
                closest = hits[i];
                closestDist = curDist; 
            }
        }
        var closestVec = other.transform.position - closest.transform.position;
        var closestVecAngle = Vector2.SignedAngle(new Vector2(1, 1), new Vector2(closestVec.x, closestVec.z));

        var newPlatformLoc = closest.transform.position;
        var spawnDir = GetDir(closestVecAngle);
        switch (spawnDir)
        {
            case SpawnDirection.Left:
                newPlatformLoc += new Vector3(-cellWidth, 0, 0);
                break;
            case SpawnDirection.Top:
                newPlatformLoc += new Vector3(0, 0, cellLength);
                break;
            case SpawnDirection.Right:
                newPlatformLoc += new Vector3(cellWidth, 0, 0);
                break;
            case SpawnDirection.Bottom:
                newPlatformLoc += new Vector3(0, 0, -cellLength);
                break;
        }

        SpawnDirection GetDir(float angle) =>
        angle switch
        {
            <= 90 and >= 0 => SpawnDirection.Top,
            > 90 => SpawnDirection.Left,
            < 0 and > -90 => SpawnDirection.Right,
            < -90 => SpawnDirection.Bottom,
            _ => SpawnDirection.Top,
        };

        StartCoroutine(Utility.DelayedAction(() =>
        {
            var newPlatform = AddNode(newPlatformLoc, remeasureRadius: true);
            UpdateMaxWeight();

            newPlatform.SearchEmptySides(cellWidth, cellLength);

            foreach (var hit in hits)
            {
                var hitNode = hit.GetComponentInParent<GridNode>();
                hitNode.AssignMesh();
                if (hitNode.SidesFilled() >= 3 && hitNode.SidesDecorated() <= 1)
                {
                    var randomNumber = UnityEngine.Random.Range(0, spawnNewDecorationOdds);
                    if (randomNumber == spawnNewDecorationOdds - 1)
                    {
                        hitNode.Decorate();
                    }
                }
            }

            newPlatform.AssignMesh(true, newPlatformAnimateDistance, newPlatformAnimateSpeed);

            SFXController.Instance.PlayOneShot(SFXController.Instance.islandGrows, newPlatform.transform.position);

            Destroy(other.gameObject);


        }, newPlatformSpawnDelay));
    }

    private void UpdateMaxWeight()
    {
        maxWeightUpdate.Raise(Mathf.RoundToInt(nodes.Count * tileWeightCapacity));
    }
    #endregion
    #region Public API
    public float GetApproxIslandRadius()
    {
        return approxIslandRadius;
    }

    public Vector3 GetCircleEdgePoint(float radius, float degreeRotation)
    {
        Quaternion rot = Quaternion.Euler(new Vector3(0f, degreeRotation, 0f));
        return transform.position + (rot * Vector3.forward * radius);
    }

    public GridNode GetRandomTile(HashSet<GridNode> exemptNodes, Func<GridNode, bool> tileTest = null)
    {
        var length = nodes.Count - exemptNodes.Count;
        var eligibleNodes = nodes.Except(exemptNodes);

        for (int i = 0; i < 30; i++)
        {
            var randomTile = eligibleNodes.ElementAt(UnityEngine.Random.Range(0, length));
            if (tileTest == null)
            {
                return randomTile;
            }
            else if (tileTest(randomTile) == true)
            {
                return randomTile;
            }
        }

        return null;
    }

    public int GetMaxAllowedResources()
    {
        return Mathf.Min(3, nodes.Count / 17);
    }
    #endregion
}
