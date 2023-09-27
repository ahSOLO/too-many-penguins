using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private void OnEnable()
    {
        Rotate();
    }

    private void Update()
    {
        Rotate();
    }

    private void Rotate()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}
