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

            if (gamepad != null)
            {
                gamepad.SetMotorSpeeds(1.0f, 1.0f);
            }
        }
    }
}
