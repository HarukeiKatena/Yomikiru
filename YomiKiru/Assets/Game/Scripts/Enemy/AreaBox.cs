using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaBox : MonoBehaviour
{
    [SerializeField] private Vector3 _size;
    public Vector3 Size{
        get { return _size; }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0, 0, 1f);
        Gizmos.DrawWireCube (transform.position, _size);
    }
}
