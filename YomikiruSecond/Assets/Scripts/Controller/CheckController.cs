using System;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Yomikiru.Controller
{
    public class CheckController : MonoBehaviour
    {
        [SerializeField] private ControllerManager controller;

        [field: Header("JoinButton")]
        [field: SerializeField] public InputAction joinButton { get; private set; }

        [field: Header("ExitButton")]
        [field: SerializeField] public InputAction cancelButton { get; private set; }

        public ControllerManager ControllerManager => controller;
        public IObservable<ControllerManager> ChangePlayerDevice => changePlayerDevice;
        private Subject<ControllerManager> changePlayerDevice = new Subject<ControllerManager>();

        private void Start()
        {
            joinButton.performed += CheckInputDeviceEvent;
            cancelButton.performed += CancelPlayerDeviceEvent;
        }

        private void CheckInputDeviceEvent(InputAction.CallbackContext callback)
        {
            var device = callback.control.device;

            //入力されたデバイスがゲームパッドの場合
            foreach (var gamepad in Gamepad.all.Where(gamepad => gamepad.deviceId == device.deviceId))
            {
                for (int i = 0; i < controller.PlayerCount; i++)
                {
                    //同じやつがある場合抜ける
                    if(controller.PlayerDevices[i] == gamepad)
                        break;

                    //使われている場合抜ける
                    if (controller.KeybordPlayerIndex == i ||
                        controller.PlayerDevices[i] != null)
                        continue;

                    controller.PlayerDevices[i] = gamepad;
                    changePlayerDevice.OnNext(controller);//イベント通知
                    break;
                }
            }

            //入力されたデバイスがキーボードの場合
            if (Keyboard.current.deviceId == device.deviceId)
            {
                for (int i = 0; i < controller.PlayerCount; i++)
                {
                    //未登録の場所にセットする
                    if (controller.PlayerDevices[i] != null)
                        continue;

                    controller.KeybordPlayerIndex = i;
                    changePlayerDevice.OnNext(controller);
                    break;
                }
            }
        }

        private void CancelPlayerDeviceEvent(InputAction.CallbackContext callback)
        {
            var device = callback.control.device;
            for (int i = 0; i < controller.PlayerCount; i++)
            {
                //パッド
                if (controller.PlayerDevices[i] != null &&
                    device.deviceId == controller.PlayerDevices[i].deviceId)
                {
                    controller.PlayerDevices[i] = null;
                    changePlayerDevice.OnNext(controller);//イベント通知
                }

                //キーボード
                if (controller.KeybordPlayerIndex == i &&
                    device.deviceId == Keyboard.current.deviceId)
                {
                    controller.KeybordPlayerIndex = ControllerManager.NotUsedKeybord;
                    changePlayerDevice.OnNext(controller);
                }
            }
        }

        private void OnEnable()
        {
            joinButton.Enable();
            cancelButton.Enable();
        }

        private void OnDisable()
        {
            joinButton.Disable();
            cancelButton.Disable();
        }

        /*//何かあったとき用に残しておく
        private void Update()
        {
            CheckInputDevice();
            CancelPlayerDevice();
        }

        //入力を受け付けたコントローラーを取得して保存
        private void CheckInputDevice()
        {
            //ゲームパッドで右のボタンを入力された場合
            if (Gamepad.current != null &&
                Gamepad.current.buttonSouth.wasPressedThisFrame)
            {
                var pad = Gamepad.current;

                for (int i = 0; i < controller.PlayerCount; i++)
                {
                    //同じやつがある場合抜ける
                    if(controller.PlayerDevices[i] == pad)
                        break;

                    //使われている場合抜ける
                    if (controller.KeybordPlayerIndex == i ||
                        controller.PlayerDevices[i] != null)
                        continue;

                    controller.PlayerDevices[i] = pad;
                    changePlayerDevice.OnNext(controller);//イベント通知
                    break;
                }
            }

            //キーボードでエンターが押された場合
            if (Keyboard.current != null && Mouse.current != null &&
                Keyboard.current.enterKey.wasPressedThisFrame)
            {
                for (int i = 0; i < controller.PlayerCount; i++)
                {
                    //未登録の場所にセットする
                    if (controller.PlayerDevices[i] != null)
                        continue;

                    controller.KeybordPlayerIndex = i;
                    changePlayerDevice.OnNext(controller);
                    break;
                }
            }
        }

        //登録デバイスの削除申請を取得する
        void CancelPlayerDevice()
        {
            for (int i = 0; i < controller.PlayerCount; i++)
            {
                //パッド
                if (controller.PlayerDevices[i] != null && controller.PlayerDevices[i].buttonEast.wasPressedThisFrame)
                {
                    controller.PlayerDevices[i] = null;
                    changePlayerDevice.OnNext(controller);//イベント通知
                }

                //キーボード
                if (controller.KeybordPlayerIndex == i &&
                    Keyboard.current != null &&
                    Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    controller.KeybordPlayerIndex = ControllerManager.NotUsedKeybord;
                    changePlayerDevice.OnNext(controller);
                }
            }
        }
        */
    }
}
