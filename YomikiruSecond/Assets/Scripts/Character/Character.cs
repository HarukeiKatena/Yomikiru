using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.InputSystem;

namespace Yomikiru.Character
{
    [RequireComponent(typeof(CharacterController))]
    public class Character : MonoBehaviour
    {
        // 外部パラメーター
        [field: Header("Data")]
        [field: SerializeField] public CharacterData Table { get; private set; }

        // 公開パラメーター
        public bool IsGrounded { get; private set; }

        // 内部オブジェクト
        [field: Header("Inner Object")]
        [field: SerializeField] public Transform Eye { get; private set; }
        [field: SerializeField] public Transform Hand { get; private set; }
        [field: SerializeField] public Transform Foot { get; private set; }
        [field: SerializeField] public GameObject Visual { get; private set; }

        // 外部オブジェクト
        public GameObject Manager { get; private set; }

        // 内部コンポーネント
        private CharacterController controller;

        private void Awake()
        {
            TryGetComponent(out controller);
        }

        private void Start()
        {
            controller.slopeLimit = Table.SlopeLimit;
            controller.stepOffset = Table.StepOffset;
            controller.skinWidth = Table.SkinWidth;
            controller.minMoveDistance = Table.MinMoveDistance;
            controller.center = Table.Center;
            controller.radius = Table.Radius;
            controller.height = Table.Height;

            IsGrounded = false;

            Manager = GameObject.Find("Manager");
        }

        private void Update()
        {
            IsGroundedCheck();
        }

        private void IsGroundedCheck()
        {
            RaycastHit hit;
            Ray ray = new Ray(Foot.position + Vector3.up * (Table.Radius + 0.1f), Vector3.down);
            IsGrounded = Physics.SphereCast(ray, Table.Radius, out hit, Table.StepOffset + 0.1f);
        }
    }
}