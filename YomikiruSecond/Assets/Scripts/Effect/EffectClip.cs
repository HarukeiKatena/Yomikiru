using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yomikiru.Utility;

namespace Yomikiru.Effect
{
    [CreateAssetMenu(menuName = "ScriptableObjects/EffectClip")]
    public class EffectClip : ScriptableObject
    {
        [field: SerializeField] public GameObject EffectPrefub { get; private set; }
        [field: SerializeField] public int PlayMax { get; private set; }
        [field: SerializeField] public int LifeTime { get; private set; }
        [field: SerializeField] public bool isDynamic { get; private set; }
        [field: SerializeField]public TransformationData Transformation { get; private set; }
    }
}
