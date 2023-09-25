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

    [SerializeField] private GameObject[] centerTiles;
    [SerializeField] private GameObject[] sideTiles;
    [SerializeField] private GameObject[] cornerTiles;

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

    public void AssignMesh(bool animate = false, float animateDistance = 0f, float animateDuration = 0f)
    {
        col.enabled = true;
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

        GameObject meshGO = null;

        if (sidesFilled == 2 && (
            (T == null && R == null) ||
            (T == null && L == null) ||
            (B == null && L == null) ||
            (B == null && R == null)))
        {
            if (transform.childCount == 0 || !transform.GetChild(0).CompareTag("Platform Corner"))
            {
                if (transform.childCount > 0)
                {
                    Destroy(transform.GetChild(0).gameObject);
                }
                var tile = Utility.RandomFromArray<GameObject>(cornerTiles);
                meshGO = Instantiate(tile, transform, false);
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
        }
        else if (sidesFilled == 3)
        {
            if (transform.childCount == 0 || !transform.GetChild(0).CompareTag("Platform Side"))
            {
                if (transform.childCount > 0)
                {
                    Destroy(transform.GetChild(0).gameObject);
                }
                var tile = Utility.RandomFromArray<GameObject>(sideTiles);
                meshGO = Instantiate(tile, transform, false);
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
            if (transform.childCount == 0 || !transform.GetChild(0).CompareTag("Platform Center"))
            {
                if (transform.childCount > 0)
                {
                    Destroy(transform.GetChild(0).gameObject);
                }
                var tile = Utility.RandomFromArray<GameObject>(centerTiles);
                meshGO = Instantiate(tile, transform, false);
            }
        }

        if (animate && meshGO != null)
        {
            var target = meshGO.transform.localPosition;
            meshGO.transform.localPosition += new Vector3(0f, -animateDistance, 0f);
            StartCoroutine(Utility.MoveLocalTransformOverTime(meshGO.transform, target, animateDuration));
        }
    }

    public bool IsTrueCenterTile()
    {
        return T != null && R != null & B != null & L != null;
    }
}
