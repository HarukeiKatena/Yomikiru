using System;
using UnityEngine;
using UniRx;
using Unity.Mathematics;
using UnityEngine.Rendering;
using Yomikiru.Input;

namespace Yomikiru.Character
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(InputEvent))]
    public class PlayerMove : MonoBehaviour
    {
        // イベント（発行）
        private readonly Subject<Vector2> onPlayerMove = new Subject<Vector2>();

        // イベント（講読）
        public IObservable<Vector2> OnPlayerMove => onPlayerMove;

        // 内部コンポーネント
        private Character character;
        private CharacterData table;
        private CharacterController controller;
        private InputEvent inputEvent;

        // 内部パラメーター
        private Vector2 direction = Vector2.zero;
        private Vector2 velocity = Vector2.zero;

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
        }

        public void MoveUpdate(Unit unit)
        {
            velocity += direction * table.Accel;

            if (velocity.sqrMagnitude > table.MaxSpeed * table.MaxSpeed)
            {
                velocity = velocity.normalized * table.MaxSpeed;
            }

            if (velocity.sqrMagnitude >= table.MinSpeed * table.MinSpeed)
            {
                Quaternion horizontalRotation = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up);

                Vector3 dir = horizontalRotation * new Vector3(velocity.x, 0, velocity.y) * Time.deltaTime;

                RaycastHit hit;
                Vector3 point1 = transform.position + Vector3.up * table.Radius;
                Vector3 point2 = transform.position + Vector3.up * (table.Height - table.Radius);
                bool isHit = Physics.CapsuleCast(point1, point2, table.Radius, dir.normalized, out hit, dir.magnitude);

                if (isHit)
                {
                    dir = dir.normalized * hit.distance;
                }

                controller.Move(dir);

                onPlayerMove.OnNext(velocity);
            }

            velocity *= table.Attenuate;
        }
    }
}
