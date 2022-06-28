using UnityEngine;
using Cinemachine;
using JetBrains.Annotations;

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
        [field: SerializeField] public float CheckGroundDistance { get; private set; }

        [field: Header("Move")]
        [field: SerializeField] public float MinSpeed  { get; private set; }
        [field: SerializeField] public float MaxSpeed  { get; private set; }
        [field: SerializeField] public float Accel     { get; private set; }
        [field: SerializeField] public float Attenuate { get; private set; }

        [field: Header("Sprint")]
        [field: SerializeField] public float SprintMinSpeed  { get; private set; }
        [field: SerializeField] public float SprintMaxSpeed  { get; private set; }
        [field: SerializeField] public float SprintAccel     { get; private set; }
        [field: SerializeField] public float SprintAttenuate { get; private set; }

        [field: Header("Jump")]
        [field: SerializeField] public float Gravity { get; private set; }
        [field: SerializeField] public float GravityScale { get; private set; }
        [field: SerializeField] public float Mass { get; private set; }
        [field: SerializeField] public float JumpHeight { get; private set; }
        [field: SerializeField] public float JumpBouciness { get; private set; }

        [field: Header("Camera")]
        [field: SerializeField] public AxisState HorizontalAxis { get; private set; }
        [field: SerializeField] public AxisState VerticalAxis { get; private set; }

        [field: Header("Effect")]
        [field: SerializeField] public string WalkEffectName { get; private set; }
        [field: SerializeField] public string SprintEffectName { get; private set; }
        [field: SerializeField] public string LandingEffectName { get; private set; }
        [field: SerializeField] public string AttackEffectName { get; private set; }
        [field: SerializeField] public float WalkEffectDuration { get; private set; }
        [field: SerializeField] public float SprintEffectDuration { get; private set; }

    }
}
