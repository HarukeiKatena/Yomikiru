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
        private Vector3 velocity = Vector3.zero;
        private Vector2 direction = Vector2.zero;
        private Vector2 horizontalVelocity = Vector2.zero;
        private float verticalVelocity = 0.0f;
        private bool isSprint = false;
        private bool isMoving = false;
        private bool isJumping = false;
        private bool isGrounded = false;
        private RaycastHit GroundData;
        private IDisposable effectTask = null;

        public void OnJump()
        {
            if (isJumping) return;

            isJumping = true;
            isGrounded = false;
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

            isJumping = false;
            isGrounded = true;
            CheckIsGrounded();
            OnSprint(false);
        }

        private void Update()
        {
            velocity = Vector3.zero;

            MoveUpdate();
            JumpUpdate();

            controller.Move(velocity);

            CheckIsGrounded();
        }

        public void MoveUpdate()
        {
            float accel = isSprint ? table.SprintAccel : table.Accel;
            float minSpeed = isSprint ? table.SprintMinSpeed : table.MinSpeed;
            float maxSpeed = isSprint ? table.SprintMaxSpeed : table.MaxSpeed;
            float attenuate = isSprint ? table.SprintAttenuate : table.Attenuate;

            horizontalVelocity += direction * accel;

            if (horizontalVelocity.sqrMagnitude == 0.0f) return;

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

                velocity.x = dir.x * Time.deltaTime;
                velocity.z = dir.z * Time.deltaTime;

                isMoving = true;
            }
            else
            {
                isMoving = false;
            }

            horizontalVelocity *= attenuate;
        }

        public void JumpUpdate()
        {
            if (isGrounded)
            {
                if (isJumping && GroundData.distance <= table.CheckJumpDistance)
                {
                    velocity.y = -GroundData.distance;
                    verticalVelocity = 0.0f;
                    isJumping = false;
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

            velocity.y += verticalVelocity * Time.deltaTime;
            isGrounded = character.IsGrounded;
        }

        public void CheckIsGrounded()
        {
            Ray ray = new Ray(character.Foot.position + Vector3.up * (table.Radius / 2 + table.SkinWidth), Vector3.down);
            isGrounded = Physics.SphereCast(ray, table.Radius / 2, out GroundData, table.CheckJumpDistance + table.SkinWidth);
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
