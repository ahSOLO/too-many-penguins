using System.Collections;
using UnityEngine;

public class GridNode: MonoBehaviour
{
    public GridNode T;
    public GridNode L;
    public GridNode R;
    public GridNode B;

    public Collider col;
    public bool occupied = false;

    [SerializeField] private Transform platformParent;
    [SerializeField] private Transform decorationsParent;

    [SerializeField] private GameObject[] centerTiles;
    [SerializeField] private GameObject[] sideTiles;
    [SerializeField] private GameObject[] cornerTiles;
    [SerializeField] private GameObject iceRock;
    [SerializeField] private GameObject snowPatch;

    private void Awake()
    {
        col = GetComponent<Collider>();
    }

    public void AppendTop(GridNode node)
    {
        if (T != null)
        {
            return;
        }
        
        T = node;
        node.B = this;
    }

    public void AppendRight(GridNode node)
    {
        if (R != null)
        {
            return;
        }

        R = node;
        node.L = this;
    }

    public void AppendLeft(GridNode node)
    {
        if (L != null)
        {
            return;
        }

        L = node;
        node.R = this;
    }

    public void AppendBottom(GridNode node)
    {
        if (B != null)
        {
            return;
        }

        B = node;
        node.T = this;
    }

    public void SearchEmptySides(float gridWidth, float gridLength)
    {
        if (T == null)
        {
            RaycastSearch(Vector3.forward, gridLength + 0.3f, gridWidth, gridLength);
        }
        if (R == null)
        {
            RaycastSearch(Vector3.right, gridWidth + 0.3f, gridWidth, gridLength);
        }
        if (B == null)
        {
            RaycastSearch(-Vector3.forward, gridLength + 0.3f, gridWidth, gridLength);
        }
        if (L == null)
        {
            RaycastSearch(-Vector3.right, gridWidth + 0.3f, gridWidth, gridLength);
        }
    }

    private void RaycastSearch(Vector3 dir, float length, float gridWidth, float gridLength)
    {
        Ray ray = new Ray(transform.position, dir);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, length, LayerMask.GetMask("Platform")))
        {
            var hitNode = hitInfo.collider.GetComponentInParent<GridNode>();
            if (dir.z == 1)
            {
                T = hitNode;
                hitNode.B = this;
            }
            else if (dir.x == 1)
            {
                R = hitNode;
                hitNode.L = this;
            }
            else if (dir.z == -1)
            {
                B = hitNode;
                hitNode.T = this;
            }
            else if (dir.x == -1)
            {
                L = hitNode;
                hitNode.R = this;
            }
        }
    }

    public void Decorate()
    {
        if (decorationsParent.childCount > 0)
        {
            return;
        }

        int numberOfRocks = UnityEngine.Random.Range(0, 3);
        int patchRandomizer = UnityEngine.Random.Range(0, 3);

        for (int i = 0; i < numberOfRocks; i++)
        {
            SpawnDecoration(iceRock, 1.2f);
        }

        if (patchRandomizer == 2)
        {
            SpawnDecoration(snowPatch, 0.6f);
        }
    }

    private GameObject SpawnDecoration(GameObject gO, float radius)
    {
        var randomPoint = Random.insideUnitCircle * radius;
        return Instantiate(gO, decorationsParent.position + new Vector3(randomPoint.x, 0, randomPoint.y), Quaternion.Euler(0f, UnityEngine.Random.Range(0, 360), 0f), decorationsParent);
    }

    public void AssignMesh(bool animate = false, float animateDistance = 0f, float animateSpeed = 0f)
    {
        col.enabled = true;
        int sidesFilled = SidesFilled();

        GameObject meshGO = null;

        if (sidesFilled == 2 && (
            (T == null && R == null) ||
            (T == null && L == null) ||
            (B == null && L == null) ||
            (B == null && R == null)))
        {
            if (platformParent.childCount == 0 || !platformParent.GetChild(0).CompareTag("Platform Corner"))
            {
                if (platformParent.childCount > 0)
                {
                    Destroy(platformParent.GetChild(0).gameObject);
                }
                var tile = Utility.RandomFromArray<GameObject>(cornerTiles);
                meshGO = Instantiate(tile, platformParent, false);
                var rotation =
                    T == null && R == null ? Quaternion.identity :
                    R == null && B == null? Quaternion.Euler(0f, 90f, 0f) :
                    B == null && L == null ? Quaternion.Euler(0f, 180f, 0f) :
                    Quaternion.Euler(0, -90f, 0f);
                meshGO.transform.rotation = rotation;
                if (meshGO.GetComponent<MeshCollider>() != null)
                {
                    col.enabled = false;
                }
            }
            else if (platformParent.GetChild(0).GetComponent<MeshCollider>() != null)
            {
                col.enabled = false;
            }
        }
        else if (sidesFilled == 3)
        {
            if (platformParent.childCount == 0 || !platformParent.GetChild(0).CompareTag("Platform Side"))
            {
                if (platformParent.childCount > 0)
                {
                    Destroy(platformParent.GetChild(0).gameObject);
                }
                var tile = Utility.RandomFromArray<GameObject>(sideTiles);
                meshGO = Instantiate(tile, platformParent, false);
                var rotation =
                    R == null ? Quaternion.identity :
                    B == null ? Quaternion.Euler(0f, 90f, 0f) :
                    L == null ? Quaternion.Euler(0f, 180f, 0f) :
                    Quaternion.Euler(0, -90f, 0f);
                meshGO.transform.rotation = rotation;
            }
        }
        else
        {
            if (platformParent.childCount == 0 || !platformParent.GetChild(0).CompareTag("Platform Center"))
            {
                if (platformParent.childCount > 0)
                {
                    Destroy(platformParent.GetChild(0).gameObject);
                }
                var tile = Utility.RandomFromArray<GameObject>(centerTiles);
                meshGO = Instantiate(tile, platformParent, false);
            }
        }

        if (animate && meshGO != null)
        {
            var target = platformParent.localPosition;
            platformParent.localPosition += new Vector3(0f, -animateDistance, 0f);
            StartCoroutine(Utility.MoveLocalTransformOverTime(platformParent, target, animateSpeed));
        }
    }

    public bool IsTrueCenterTile()
    {
        return T != null && R != null & B != null & L != null;
    }

    public int SidesFilled()
    {
        int sidesFilled = 0;
        if (T != null)
        {
            sidesFilled++;
        }
        if (R != null)
        {
            sidesFilled++;
        }
        if (B != null)
        {
            sidesFilled++;
        }
        if (L != null)
        {
            sidesFilled++;
        }
        return sidesFilled;
    }

    public bool isDecorated()
    {
        return decorationsParent.childCount > 0;
    }

    public int SidesDecorated()
    {
        int sidesDecorated = 0;
        if (T != null && T.isDecorated())
        {
            sidesDecorated++;
        }
        if (R != null && R.isDecorated())
        {
            sidesDecorated++;
        }
        if (B != null && B.isDecorated())
        {
            sidesDecorated++;
        }
        if (L != null && L.isDecorated())
        {
            sidesDecorated++;
        }

        return sidesDecorated;
    }
}
