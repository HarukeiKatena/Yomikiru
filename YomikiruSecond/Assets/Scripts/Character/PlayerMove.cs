using System;
using Player;
using UnityEngine;
using UniRx;
using Yomikiru.Input;

namespace Yomikiru.Character
{
    [RequireComponent(typeof(Character))]
    public class PlayerMove : MonoBehaviour
    {
        // 内部コンポーネント
        private Character character;
        private CharacterData table;
        private CharacterController controller;

        // 内部パラメーター
        private Vector2 direction = Vector2.zero;
        private Vector2 horizontalVelocity = Vector2.zero;
        private float verticalVelocity = 0.0f;
        private bool isSprint = false;
        private bool isMoving = false;
        private bool isGrounded = false;
        private IDisposable effectTask = null;

        public void OnJump()
        {
            if (character.IsGrounded is false) return;

            isGrounded = true;

            verticalVelocity = Mathf.Sqrt(table.JumpHeight * table.Gravity * table.GravityScale * table.Mass * -2.0f);
        }

        public void OnMove(Vector2 dir)
        {
            direction = dir;
        }

        public void OnSprint(bool value)
        {
            isSprint = value;

            if (effectTask != null)
            {
                effectTask.Dispose();
                effectTask = null;
            }

            if (isSprint)
            {
                effectTask = Observable.Interval(TimeSpan.FromSeconds(table.SprintEffectDuration))
                    .Subscribe(_ => MoveEffect())
                    .AddTo(this);
            }
            else
            {
                effectTask = Observable.Interval(TimeSpan.FromSeconds(table.WalkEffectDuration))
                    .Subscribe(_ => MoveEffect())
                    .AddTo(this);
            }
        }

        private void Awake()
        {
            TryGetComponent(out character);
            TryGetComponent(out controller);
        }

        private void Start()
        {
            table = character.Table;

            OnSprint(false);
        }

        private void Update()
        {
            float accel = isSprint ? table.SprintAccel : table.Accel;
            float minSpeed = isSprint ? table.SprintMinSpeed : table.MinSpeed;
            float maxSpeed = isSprint ? table.SprintMaxSpeed : table.MaxSpeed;
            float attenuate = isSprint ? table.SprintAttenuate : table.Attenuate;
            Vector3 velocity = Vector3.zero;

            horizontalVelocity += direction * accel;

            if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
            {
                horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
            }

            if (horizontalVelocity.sqrMagnitude >= minSpeed * minSpeed)
            {
                Quaternion horizontalRotation = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up);

                Vector3 dir = horizontalRotation * new Vector3(horizontalVelocity.x, 0, horizontalVelocity.y);

                Ray ray = new Ray(character.Eye.position, dir.normalized);
                RaycastHit hit;
                bool isHit = Physics.Raycast(ray, out hit, horizontalVelocity.magnitude * Time.deltaTime);
                if (isHit)
                {
                    dir = dir.normalized * ((hit.distance - table.Radius));
                }

                velocity.x = dir.x;
                velocity.z = dir.z;

                isMoving = true;
            }
            else
            {
                isMoving = false;
            }

            horizontalVelocity *= attenuate;

            if (character.IsGrounded)
            {
                if (isGrounded is false)
                {
                    verticalVelocity = 0.0f;
                    velocity.y = -character.GroundData.distance / Time.deltaTime;
                }
            }
            else
            {
                Ray ray = new Ray(transform.position + Vector3.up * table.Height, Vector3.up);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 0.1f) && verticalVelocity > 0.0f)
                {
                    verticalVelocity *= -table.JumpBouciness;
                }

                verticalVelocity += table.Gravity * table.GravityScale * table.Mass * Time.deltaTime;
            }

            velocity.y += verticalVelocity;

            controller.Move(velocity * Time.deltaTime);
            isGrounded = character.IsGrounded;
        }

        public void MoveEffect()
        {
            if (isMoving is false) return;

            if (isSprint)
            {
                //character.effectManager.Play(table.SprintEffectName, transform.position);
            }
            else
            {
                //character.effectManager.Play(table.WalkEffectName, transform.position);
            }
        }


    }
}
