using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class IslandGrid : Singleton<IslandGrid>
{
    private enum SpawnDirection { Left, Top, Right, Bottom };

    [SerializeField] private ColliderEvent iceBlockHitsWater;
    [SerializeField] private float newPlatformSpawnDelay;

    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private List<GridNode> nodes = new List<GridNode>();
    public GridNode root;
    private int startingLayers = 0;
    private float cellWidth = 3f;
    private float cellLength = 3f;
    private float overlapSphereSize = 15f;

    private int navMeshLastBakedNodeCount;
    private NavMeshSurface navMesh;

    protected override void Awake()
    {
        base.Awake();

        navMesh = GetComponent<NavMeshSurface>();
        
        root = Instantiate(platformPrefab, gameObject.transform).GetComponent<GridNode>();
        nodes.Add(root);
    }

    private void Start()
    {
        AddLayers(4);
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

    private void AddLayers(int number)
    {
        for (int i = 0; i < number; i++)
        {
            AddLayer();
        }
        foreach (GridNode node in nodes)
        {
            node.SearchEmptySides(cellWidth, cellLength);
        }
        foreach (GridNode node in nodes)
        {
            node.AssignMesh();
        }
    }

    private void AddLayer()
    {
        startingLayers++;

        // Spawn on top right
        var nextSpawnPoint = transform.position + new Vector3(startingLayers * cellWidth, 0, startingLayers * cellLength);
        var cur = Instantiate(platformPrefab, nextSpawnPoint, Quaternion.identity, gameObject.transform).GetComponent<GridNode>();
        nodes.Add(cur);

        nextSpawnPoint += new Vector3(0, 0, -cellLength);

        // Fill in right side;
        FillSide(ref nextSpawnPoint, transform.position + new Vector3(startingLayers * cellWidth, 0, -startingLayers * cellLength), SpawnDirection.Bottom);

        // Fill in bottom side
        FillSide(ref nextSpawnPoint, transform.position + new Vector3(-startingLayers * cellWidth, 0, -startingLayers * cellLength), SpawnDirection.Left);

        // Fill in left side
        FillSide(ref nextSpawnPoint, transform.position + new Vector3(-startingLayers * cellWidth, 0, startingLayers * cellLength), SpawnDirection.Top);

        // Fill in top side
        FillSide(ref nextSpawnPoint, transform.position + new Vector3(startingLayers * cellWidth, 0, startingLayers * cellLength), SpawnDirection.Right);
    }

    private void FillSide(ref Vector3 nextSpawnPoint, Vector3 endPoint, SpawnDirection spawnDirection)
    {
        var i = 0;
        while (nextSpawnPoint != endPoint && i < 30)
        {
            var newNode = Instantiate(platformPrefab, nextSpawnPoint, Quaternion.identity, gameObject.transform).GetComponent<GridNode>();
            nodes.Add(newNode);

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

            i++;
        }
    }

    private void OnIceBlockHitsWater(Collider other)
    {
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
            var newPlatform = Instantiate(platformPrefab, newPlatformLoc, Quaternion.identity, transform).GetComponent<GridNode>();
            nodes.Add(newPlatform);

            newPlatform.SearchEmptySides(cellWidth, cellLength);

            foreach (var hit in hits)
            {
                hit.GetComponent<GridNode>().AssignMesh();
            }

            newPlatform.AssignMesh();

            Destroy(other.gameObject);


        }, newPlatformSpawnDelay));
    }
}
