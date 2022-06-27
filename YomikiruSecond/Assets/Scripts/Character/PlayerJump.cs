using System;
using UnityEngine;
using UniRx;
using Yomikiru.Input;

namespace Yomikiru.Character
{
    [RequireComponent(typeof(Character))]
    public class PlayerJump : MonoBehaviour
    {
        // 内部コンポーネント
        private Character character;
        private CharacterData table;
        private CharacterController controller;

        // 内部パラメーター
        private float velocity = 0.0f;
        private bool isGrounded = false;

        public void OnJump()
        {
            if (character.IsGrounded is false) return;

            isGrounded = true;

            velocity = Mathf.Sqrt(table.JumpHeight * table.Gravity * table.GravityScale * -2.0f);
        }

        private void Awake()
        {
            TryGetComponent(out character);
            TryGetComponent(out controller);
        }

        private void Start()
        {
            table = character.Table;
        }

        private void Update()
        {
            if (character.IsGrounded)
            {
                if (isGrounded is false)
                {
                    velocity = 0.0f;
                    controller.Move(character.GroundData.distance * Vector3.down);
                }
            }
            else
            {
                velocity += table.Gravity * table.GravityScale * Time.deltaTime;
            }

            controller.Move(Vector3.up * (velocity * Time.deltaTime));

            isGrounded = character.IsGrounded;
        }
    }
}
