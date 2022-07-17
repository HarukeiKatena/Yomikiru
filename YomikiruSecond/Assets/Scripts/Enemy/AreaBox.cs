using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yomikiru
{
    public class AreaBox : MonoBehaviour
    {
        [SerializeField] private Vector3 size;
        public Vector3 Size
        {
            get { return size; }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0, 0, 1f);
            Gizmos.DrawWireCube(transform.position, size);
        }
    }
}
