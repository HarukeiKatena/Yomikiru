using System;
using System.Collections.Generic;
using UnityEngine;
using Yomikiru.Utility;

namespace Yomikiru.Effect
{
    [Serializable]
    public struct State
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    };

    [Serializable]
    public class EffectData : System.Object
    {
        public GameObject prehub;
        public string name;
        public int playMax;
        public float lifeTime;
        public bool isDynamic;
        public TransformationData transform;
    };
    
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/EffectTable")]
    public class EffectTable : ScriptableObject
    {
        [Header("Data")]
        [SerializeField] public List<EffectData> _effectList = new List<EffectData>();
    }
}
