using System;
using UnityEngine;

namespace Yomikiru.Character
{
    public class SwordCollision : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            var obj = other.gameObject;
            if (obj.CompareTag("Player"))
            {
                obj.GetComponent<Player>().Die();
            }
        }
    }
}
