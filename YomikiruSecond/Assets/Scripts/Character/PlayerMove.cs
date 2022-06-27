using System;
using UnityEngine;
using UniRx;
using Yomikiru.Input;

namespace Yomikiru.Character
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(InputEvent))]
    public class PlayerMove : MonoBehaviour
    {
        // 内部コンポーネント
        private Character character;
        private CharacterData table;
        private CharacterController controller;
        private InputEvent inputEvent;

        // 内部パラメーター
        private Vector2 direction = Vector2.zero;
        private Vector2 velocity = Vector2.zero;
        private bool isSprint = false;
        private bool isMoving = false;
        private IDisposable effectTask = null;

        private void Awake()
        {
            TryGetComponent(out character);
            TryGetComponent(out controller);
            TryGetComponent(out inputEvent);
        }

        private void Start()
        {
            table = character.Table;

            SprintChanged(false);
        }

        private void Update()
        {
            float accel = isSprint ? table.Accel : table.SprintAccel;
            float minSpeed = isSprint ? table.MinSpeed : table.SprintMinSpeed;
            float maxSpeed = isSprint ? table.MaxSpeed : table.SprintMaxSpeed;
            float attenuate = isSprint ? table.Attenuate : table.SprintAttenuate;

            velocity += direction * accel;

            if (velocity.sqrMagnitude > maxSpeed * maxSpeed)
            {
                velocity = velocity.normalized * maxSpeed;
            }

            if (velocity.sqrMagnitude >= minSpeed * minSpeed)
            {
                Quaternion horizontalRotation = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up);
                controller.Move(horizontalRotation * new Vector3(velocity.x, 0, velocity.y) * Time.deltaTime);

                isMoving = true;
            }
            else
            {
                isMoving = false;
            }

            velocity *= attenuate;
        }

        public void MoveEffect()
        {
            if (isMoving is false) return;

            if (isSprint)
            {
                character.effectManager.Play(table.SprintEffectName, transform.position);
            }
            else
            {
                character.effectManager.Play(table.WalkEffectName, transform.position);
            }
        }

        public void SprintChanged(bool value)
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
    }
}
