using UnityEngine;

namespace Yomikiru.Effect
{
    [CreateAssetMenu(menuName = "ScriptableObjects/EffectClip")]
    public class EffectClip : ScriptableObject
    {
        [field: Header("EffectData")]
        public Mesh ObjectMesh;
        public Material EffectMaterial;

        [field: Header("StartPosture")]
        [field: SerializeField] public Vector3 BacePosition { get; private set; } = Vector3.zero;
        [field: SerializeField] public Quaternion BaceRotation { get; private set; } = Quaternion.identity;

        [field: Header("Scale")]
        [field: SerializeField] public Vector3 StartScale { get; private set; } = Vector3.zero;
        [field: SerializeField] public Vector3 EndScale { get; private set; } = new Vector3(50.0f, 50.0f, 50.0f);

        [field: Header("Time")]
        [field: SerializeField] public float Time { get; private set; } = 4.0f;
        [field: SerializeField] public float Delay { get; private set; } = 0.0f;
}
}
