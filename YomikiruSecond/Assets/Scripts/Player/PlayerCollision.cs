using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [HideInInspector]
    public GameObject hitCollision;

    private void OnTriggerEnter(Collider other)
    {
        hitCollision = other.gameObject;
    }
}
