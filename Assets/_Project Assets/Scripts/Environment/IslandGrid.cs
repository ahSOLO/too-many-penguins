using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class IslandGrid : MonoBehaviour
{
    private enum SpawnDirection { Left, Top, Right, Bottom };
    
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private List<GridNode> nodes = new List<GridNode>();
    private GridNode root;
    private int startingLayers = 0;
    private float gridWidth = 3f;
    private float gridLength = 3f;
    private float overlapSphereSize = 15f;

    [SerializeField] private Material centerMaterial;
    [SerializeField] private Material sideMaterial;
    [SerializeField] private Material cornerMaterial;

    private int navMeshLastBakedNodeCount;
    private NavMeshSurface navMesh;

    private void Awake()
    {
        navMesh = GetComponent<NavMeshSurface>();
        
        root = Instantiate(platformPrefab, gameObject.transform).GetComponent<GridNode>();
        nodes.Add(root);
    }

    private void Start()
    {
        AddLayers(4);
    }

    private void Update()
    {
        if (navMeshLastBakedNodeCount != nodes.Count)
        {
            navMesh.BuildNavMesh();
            navMeshLastBakedNodeCount = nodes.Count;
        }
    }

    private void AddLayers(int number)
    {
        for (int i = 0; i < number; i++)
        {
            AddLayer();
        }
        foreach (GridNode node in nodes)
        {
            node.SearchEmptySides(gridWidth, gridLength);
        }
        foreach (GridNode node in nodes)
        {
            node.AssignMaterial(centerMaterial, sideMaterial, cornerMaterial);
        }
    }

    private void AddLayer()
    {
        startingLayers++;

        // Spawn on top right
        var nextSpawnPoint = transform.position + new Vector3(startingLayers * gridWidth, 0, startingLayers * gridLength);
        var cur = Instantiate(platformPrefab, nextSpawnPoint, Quaternion.identity, gameObject.transform).GetComponent<GridNode>();
        nodes.Add(cur);

        nextSpawnPoint += new Vector3(0, 0, -gridLength);

        // Fill in right side;
        FillSide(ref nextSpawnPoint, transform.position + new Vector3(startingLayers * gridWidth, 0, -startingLayers * gridLength), SpawnDirection.Bottom);

        // Fill in bottom side
        FillSide(ref nextSpawnPoint, transform.position + new Vector3(-startingLayers * gridWidth, 0, -startingLayers * gridLength), SpawnDirection.Left);

        // Fill in left side
        FillSide(ref nextSpawnPoint, transform.position + new Vector3(-startingLayers * gridWidth, 0, startingLayers * gridLength), SpawnDirection.Top);

        // Fill in top side
        FillSide(ref nextSpawnPoint, transform.position + new Vector3(startingLayers * gridWidth, 0, startingLayers * gridLength), SpawnDirection.Right);
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
                    nextSpawnPoint += new Vector3(-gridWidth, 0, 0);
                    break;
                case SpawnDirection.Top:
                    nextSpawnPoint += new Vector3(0, 0, gridLength);
                    break;
                case SpawnDirection.Right:
                    nextSpawnPoint += new Vector3(gridWidth, 0, 0);
                    break;
                case SpawnDirection.Bottom:
                    nextSpawnPoint += new Vector3(0, 0, -gridLength);
                    break;
            }

            i++;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ice Block"))
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
                    newPlatformLoc += new Vector3(-gridWidth, 0, 0);
                    break;
                case SpawnDirection.Top:
                    newPlatformLoc += new Vector3(0, 0, gridLength);
                    break;
                case SpawnDirection.Right:
                    newPlatformLoc += new Vector3(gridWidth, 0, 0);
                    break;
                case SpawnDirection.Bottom:
                    newPlatformLoc += new Vector3(0, 0, -gridLength);
                    break;
            }
            var newPlatform = Instantiate(platformPrefab, newPlatformLoc, Quaternion.identity, transform).GetComponent<GridNode>();
            nodes.Add(newPlatform);

            newPlatform.SearchEmptySides(gridWidth, gridLength);

            foreach (var hit in hits)
            {
                hit.GetComponent<GridNode>().AssignMaterial(centerMaterial, sideMaterial, cornerMaterial);
            }

            newPlatform.AssignMaterial(centerMaterial, sideMaterial, cornerMaterial);

            Destroy(other.gameObject);

            SpawnDirection GetDir(float angle) =>
                angle switch
                {
                    <= 90 and >= 0 => SpawnDirection.Top,
                    > 90 => SpawnDirection.Left,
                    < 0 and > -90 => SpawnDirection.Right,
                    < -90 => SpawnDirection.Bottom,
                    _ => SpawnDirection.Top,
                };
        }
    }
}
