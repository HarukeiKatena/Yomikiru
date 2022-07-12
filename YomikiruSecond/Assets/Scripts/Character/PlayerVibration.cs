using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Yomikiru.Characte.Management;

namespace Yomikiru.Character
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerVibration : MonoBehaviour
    {
        private List<GameObject> enemys;
        private Gamepad gamepad;

        [field: SerializeField] public float MaxDistance { get; private set; } = 5.0f;
        [field: SerializeField] public float MinDistance { get; private set; } = 0.0f;

        [Range(0.0f, 1.0f)]
        [SerializeField] private float VibrationMaxPower = 0.2f;

        private void Start()
        {
            //自分を除いた敵のオブジェクトを取得
            var cmobj = GameObject.Find("CharacterManagement");
            if (cmobj != null && cmobj.TryGetComponent(out CharacterManagement cm))
            {
                enemys = cm.GetCharacterObjectList(this.gameObject);
            }

            //自分のパッドを入手
            var gamepads = Gamepad.all;
            var pdevices = gameObject.GetComponent<PlayerInput>().devices;
            foreach (var device in pdevices)
            {
                foreach (var pad in gamepads.Where(x => x.deviceId == device.deviceId))
                {
                    gamepad = pad;
                }
            }
        }

        private void Update()
        {
            if (gamepad == null)
                return;

            var posi = transform.position;

            float length = 0.0f;
            foreach (var enemy in enemys)
            {
                float distance = (Vector3.Distance(enemy.transform.position, posi) / (MaxDistance - MinDistance));
                distance = 1.0f - Mathf.Clamp(distance, 0.0f, 1.0f);

                length = Mathf.Max(distance, length);
            }

            gamepad.SetMotorSpeeds(0.0f, length * VibrationMaxPower);
        }

        private void OnDisable()
        {
            gamepad.SetMotorSpeeds(0.0f, 0.0f);
        }
    }
}
