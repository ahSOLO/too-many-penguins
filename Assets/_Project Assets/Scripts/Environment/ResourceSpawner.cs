using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(LevelManager))]
public class ResourceSpawner : MonoBehaviour
{
    [SerializeField] private GameObject resourcePrefab;
    [SerializeField] private float preSpawnAnimationDisplacement;
    [SerializeField] private float spawnAnimationSpeed;

    private LevelManager levelManager;
    private Coroutine currentCoroutine;

    private void Awake()
    {
        levelManager = GetComponent<LevelManager>();
    }

    public void SpawnResource(int startingResources)
    {
        HashSet<GridNode> exemptNodes = new HashSet<GridNode>();
        foreach (Transform resourceTransform in levelManager.resourceParent)
        {
            var resource = resourceTransform.gameObject.GetComponent<Resource>();
            exemptNodes.Add(resource.attachedNode);
            AddExemption(resource.attachedNode.L);
            AddExemption(resource.attachedNode.T);
            AddExemption(resource.attachedNode.R);
            AddExemption(resource.attachedNode.B);
            AddExemption(resource.attachedNode.T?.R);
            AddExemption(resource.attachedNode.T?.L);
            AddExemption(resource.attachedNode.B?.R);
            AddExemption(resource.attachedNode.B?.L);
        }
        
        AddExemption(IslandGrid.Instance.root);

        void AddExemption(GridNode exemptNode)
        {
            if (exemptNode != null)
            {
                exemptNodes.Add(exemptNode);
            }
        }

        var tile = IslandGrid.Instance.GetRandomTile(exemptNodes, (node) => node.IsTrueCenterTile());

        if (tile != null)
        {
            SFXController.Instance.PlayOneShot(SFXController.Instance.crystalSpawn, new Vector3(tile.transform.position.x, 0f, tile.transform.position.z));
            
            var resource = Instantiate(resourcePrefab, levelManager.resourceParent, false).GetComponent<Resource>();
            resource.transform.position = new Vector3(tile.transform.position.x, resource.transform.position.y - preSpawnAnimationDisplacement, tile.transform.position.z);
            resource.transform.localScale = Vector3.zero;
            StartCoroutine(Utility.ExpandGO(resource.transform, 1f, spawnAnimationSpeed));
            StartCoroutine(Utility.MoveLocalTransformOverTime(resource.transform, new Vector3(resource.transform.position.x, resource.transform.position.y + preSpawnAnimationDisplacement, resource.transform.position.z), spawnAnimationSpeed));
            resource.RemainingResources = startingResources;
            tile.occupied = true;
            resource.attachedNode = tile;
        }
    }
}