using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private ColliderEvent playerHitsWater;
    [SerializeField] private ColliderEvent workerHitsWater;

    [SerializeField] private float playerRespawnTime;
    [SerializeField] private float entityHitsWaterProcessingDelay;

    public Transform iceBlockParent;
    public Transform resourceParent;
    public Transform agentParent;

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        playerHitsWater.Register(OnPlayerHitsWater);
        workerHitsWater.Register(OnWorkerHitsWater);
    }

    private void OnDisable()
    {
        playerHitsWater.Unregister(OnPlayerHitsWater);
        workerHitsWater.Unregister(OnWorkerHitsWater);
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
        StartCoroutine(Utility.DelayedAction(() =>
        {
            col.attachedRigidbody.gameObject.SetActive(false);
        }, entityHitsWaterProcessingDelay));
    }


    public IEnumerator ExpandGO(Transform tf, float targetSize, float expansionRate)
    {
        while (tf.localScale != Vector3.one * targetSize)
        {
            tf.localScale = Vector3.MoveTowards(tf.localScale, Vector3.one * targetSize, expansionRate * Time.deltaTime);
            yield return null;
        }
    }
}
