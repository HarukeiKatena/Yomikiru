using System;
using UnityEngine;
using UniRx;
using Cysharp.Threading;
using Cysharp.Threading.Tasks;
using Yomikiru.Effect;
using Yomikiru.Input;
using Yomikiru.Sound;

namespace Yomikiru.Character
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(InputEvent))]
    public class PlayerMove : MonoBehaviour
    {
        // イベント（発行）
        private readonly Subject<Vector3> onPlayerWalk = new Subject<Vector3>();
        private readonly Subject<Vector3> onPlayerSprint = new Subject<Vector3>();

        // イベント（講読）
        public IObservable<Vector3> OnPlayerWalk => onPlayerWalk;
        public IObservable<Vector3> OnPlayerSprint => onPlayerSprint;

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

            inputEvent.OnMove.Subscribe(dir => direction = dir);
            inputEvent.OnSprint.Subscribe(b => SprintChanged(b));
        }

        private void Update()
        {
            MoveUpdate();
        }

        public void MoveUpdate()
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
                onPlayerSprint.OnNext(transform.position);
            }
            else
            {
                character.effectManager.Play(table.WalkEffectName, transform.position);
                onPlayerWalk.OnNext(transform.position);
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
