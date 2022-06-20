using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Yomikiru.Characte.Management
{
    [RequireComponent(typeof(PlayerInputManager))]

    public class CharacterManagement : MonoBehaviour
    {
        [SerializeField] private GameObject PlayerPrefab;
        [SerializeField] private GameObject EnemyPrefab;
        [SerializeField] private Transform[] StartPosition;

        [HideInInspector] public List<GameObject> CharacterList;

        private void Awake()
        {

        }
    }
}
