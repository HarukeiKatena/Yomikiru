using UnityEngine;
using Cinemachine;

namespace Yomikiru.Character
{
    [CreateAssetMenu(menuName = "ScriptableObjects/CharacterData")]
    public class CharacterData : ScriptableObject
    {
        [field: Header("Controller")]
        [field: SerializeField] public float SlopeLimit { get; private set; }
        [field: SerializeField] public float StepOffset { get; private set; }
        [field: SerializeField] public float SkinWidth { get; private set; }
        [field: SerializeField] public float MinMoveDistance { get; private set; }
        [field: SerializeField] public Vector3 Center { get; private set; }
        [field: SerializeField] public float Radius { get; private set; }
        [field: SerializeField] public float Height { get; private set; }
        
        [field: Header("Move")]
        [field: SerializeField] public float MinSpeed  { get; private set; }
        [field: SerializeField] public float MaxSpeed  { get; private set; }
        [field: SerializeField] public float Accel     { get; private set; }
        [field: SerializeField] public float Attenuate { get; private set; }

        [field: Header("Jump")]
        [field: SerializeField] public float Gravity { get; private set; }
        [field: SerializeField] public float GravityScale { get; private set; }
        [field: SerializeField] public float JumpHeight { get; private set; }

        [field: Header("Camera")]
        [field: SerializeField] public AxisState HorizontalAxis { get; private set; }
        [field: SerializeField] public AxisState VerticalAxis { get; private set; }
        
    }
}
