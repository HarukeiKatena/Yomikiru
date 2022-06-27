using System;
using System.Security;
using UnityEngine;
using UniRx;
using Cinemachine;
using Yomikiru.Input;

namespace Yomikiru.Character
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(InputEvent))]
    public class PlayerCamera : MonoBehaviour
    {
        // 内部コンポーネント
        private Character character;
        private CharacterData table;
        private InputEvent inputEvent;

        // 内部パラメーター
        private AxisState horizontalAxis;
        private AxisState verticalAxis;
        private Quaternion horizontalRotation;
        private Quaternion verticalRotation;
        private Vector2 direction;

        public void OnLook(Vector2 dir)
        {
            direction = dir;
        }

        private void Awake()
        {
            TryGetComponent(out character);
        }

        private void Start()
        {
            table = character.Table;

            horizontalAxis = table.HorizontalAxis;
            verticalAxis = table.VerticalAxis;
        }

        private void Update()
        {
            horizontalAxis.m_InputAxisValue = direction.x;
            verticalAxis.m_InputAxisValue = direction.y;

            horizontalAxis.Update(Time.deltaTime);
            verticalAxis.Update(Time.deltaTime);

            horizontalRotation = Quaternion.AngleAxis(horizontalAxis.Value, Vector3.up);
            verticalRotation = Quaternion.AngleAxis(verticalAxis.Value, Vector3.right);

            transform.localRotation = horizontalRotation;
            character.Eye.localRotation = verticalRotation;
        }
    }
}
