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
        private Vector2 look;

        private void Awake()
        {
            TryGetComponent(out character);
            TryGetComponent(out inputEvent);
        }

        private void Start()
        {
            table = character.Table;

            horizontalAxis = table.HorizontalAxis;
            verticalAxis = table.VerticalAxis;

            inputEvent.OnLook.Subscribe(dir => look = dir);
        }

        public void CameraUpdate()
        {
            horizontalAxis.m_InputAxisValue = look.x;
            verticalAxis.m_InputAxisValue = look.y;

            horizontalAxis.Update(Time.deltaTime);
            verticalAxis.Update(Time.deltaTime);

            horizontalRotation = Quaternion.AngleAxis(horizontalAxis.Value, Vector3.up);
            verticalRotation = Quaternion.AngleAxis(verticalAxis.Value, Vector3.right);

            transform.localRotation = horizontalRotation;
            character.Eye.localRotation = verticalRotation;
        }
    }
}
