using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class DollyCartTestCode : MonoBehaviour
{
    [SerializeField]private CinemachineDollyCart dollyCart;

    // Start is called before the first frame update
    void Start()
    {
        dollyCart.m_Position = 0.0f;
    }
}
