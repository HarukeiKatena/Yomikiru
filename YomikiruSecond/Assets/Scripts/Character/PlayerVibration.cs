using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Yomikiru.Characte.Management;

namespace Yomikiru.Character
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerVibration : MonoBehaviour
    {
        [field: SerializeField] public float MaxDistance { get; private set; } = 5.0f;
        [field: SerializeField] public float MinDistance { get; private set; } = 0.0f;

        [Range(0.0f, 1.0f)]
        [SerializeField] private float VibrationMaxPower = 0.2f;

        [SerializeField] private MatchInfo matchInfo;

        private List<GameObject> enemys;
        private Gamepad gamepad;

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
            foreach (var device in pdevices.OfType<Gamepad>())
            {
                gamepad = device as Gamepad;
            }

            matchInfo.OnStateChange.Subscribe(x =>
            {
                switch (x)
                {
                    case MatchState.Ingame://ゲーム中のみ振動するようにする
                        InputSystem.ResumeHaptics();
                        break;
                    default:
                        InputSystem.ResetHaptics();
                        InputSystem.PauseHaptics();
                        break;
                }
            });

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
