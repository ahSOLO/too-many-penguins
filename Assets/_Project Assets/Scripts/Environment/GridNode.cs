using UnityEngine;

public class GridNode: MonoBehaviour
{
    public GridNode T;
    public GridNode L;
    public GridNode R;
    public GridNode B;

    public MeshFilter mesh;
    public Renderer rend;

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
            var hitNode = hitInfo.collider.GetComponent<GridNode>();
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

    public void AssignMaterial(Material center, Material side, Material corner)
    {
        if ((T == null && R == null && L != null && B != null) ||
            (T == null && L == null && R != null && B != null) ||
            (B == null && L == null && R != null && T != null) ||
            (B == null && R == null && L != null && T != null))
        {
            rend.material = corner;
        }
        else if ((T != null && R != null && L != null && B != null) ||
                (T != null && R == null && L == null && B != null) ||
                (T == null && R != null && L != null && B == null))
        {
            rend.material = center;
        }
        else
        {
            rend.material = side;
        }
    }
}
