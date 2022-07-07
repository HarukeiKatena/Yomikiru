using System;
using UnityEngine;
using Yomikiru.Effect;

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
        public RaycastHit GroundData { get; private set; }

        // 内部オブジェクト
        [field: Header("Inner Object")]
        [field: SerializeField] public Transform Eye { get; private set; }
        [field: SerializeField] public Transform Hand { get; private set; }
        [field: SerializeField] public Transform Foot { get; private set; }
        [field: SerializeField] public GameObject Visual { get; private set; }

        // 外部コンポーネント
        [field: Header("Manager")]
        // [field: SerializeField] public EffectChannel effectChannel;
        // [field: SerializeField] public AudioChannel audioChannel;

        // 内部コンポーネント
        private CharacterController controller;

        private void Awake()
        {
            TryGetComponent(out controller);
        }

        private void Start()
        {
            // Character Controllerに値を設定
            controller.slopeLimit = Table.SlopeLimit;
            controller.stepOffset = Table.StepOffset;
            controller.skinWidth = Table.SkinWidth;
            controller.minMoveDistance = Table.MinMoveDistance;
            controller.center = Table.Center;
            controller.radius = Table.Radius;
            controller.height = Table.Height;

            IsGrounded = true;
        }

        private void Update()
        {
            // 設置判定強化
            RaycastHit hit;
            Ray ray = new Ray(Foot.position + Vector3.up * (Table.Radius + Table.SkinWidth), Vector3.down);
            IsGrounded = Physics.SphereCast(ray, Table.Radius, out hit, Table.CheckGroundDistance + Table.SkinWidth);
            GroundData = hit;
        }
    }
}