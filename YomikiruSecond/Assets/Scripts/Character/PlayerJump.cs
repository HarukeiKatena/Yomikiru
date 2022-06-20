using System;
using UnityEngine;
using UniRx;
using Yomikiru.Input;

namespace Yomikiru.Character
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(InputEvent))]
    public class PlayerJump : MonoBehaviour
    {
        // イベント（発行）
        private readonly Subject<Unit> onPlayerJump = new Subject<Unit>();
        private readonly Subject<Unit> onPlayerLanding = new Subject<Unit>();

        // イベント（講読）
        public IObservable<Unit> OnPlayerJump => onPlayerJump;
        public IObservable<Unit> OnPlayerLanding => onPlayerLanding;

        // 内部コンポーネント
        private Character character;
        private CharacterData table;
        private CharacterController controller;

        // 内部パラメーター
        private float velocity = 0.0f;
        private bool isGrounded = false;

        private void Awake()
        {
            TryGetComponent(out character);
            TryGetComponent(out controller);
        }

        private void Start()
        {
            table = character.Table;
        }

        public void FallFixedUpdate(Unit unit)
        {
            if (character.IsGrounded)
            {
                if (isGrounded is false)
                {
                    velocity = 0.0f;

                    RaycastHit hit;
                    Ray ray = new Ray(character.Foot.position + Vector3.up * 0.1f, Vector3.down);
                    bool isHit = Physics.Raycast(ray, out hit, table.StepOffset + 0.1f);
                    if (isHit)
                    {
                        controller.Move(hit.distance * Vector3.down);
                    }

                    onPlayerLanding.OnNext(Unit.Default);
                }
            }
            else
            {
                velocity += table.Gravity * table.GravityScale * Time.fixedDeltaTime;
            }

            controller.Move(Vector3.up * velocity * Time.fixedDeltaTime);

            isGrounded = character.IsGrounded;
        }

        public void JumpStart(Unit unit)
        {
            if (character.IsGrounded is false) return;

            isGrounded = true;

            velocity = Mathf.Sqrt(table.JumpHeight * table.Gravity * table.GravityScale * -2.0f);

            onPlayerJump.OnNext(Unit.Default);
        }
    }
}
