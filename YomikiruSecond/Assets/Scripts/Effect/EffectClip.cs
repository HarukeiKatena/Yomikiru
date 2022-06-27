
using UnityEngine;


namespace Yomikiru.Effect
{
    [CreateAssetMenu(menuName = "ScriptableObjects/EffectClip")]
    public class EffectClip : ScriptableObject
    {
        [field: SerializeField] public GameObject EffectPrefub { get; private set; }
        [field: SerializeField] public int PlayMax { get; private set; }
        [field: SerializeField] public int LifeTime { get; private set; }
        [field: SerializeField] public bool isDynamic { get; private set; }

        [field: Header("Transformation")]
        [field: SerializeField] public Vector3 StartScale { get; private set; } = new Vector3(0.1f, 0.1f, 0.1f);
        [field: SerializeField] public Vector3 EndScale { get; private set; } = new Vector3(50.0f, 50.0f, 50.0f);
        [field: SerializeField] public float Time { get; private set; } = 4.0f;
        [field: SerializeField] public float Delay { get; private set; } = 0.0f;
    }
}
