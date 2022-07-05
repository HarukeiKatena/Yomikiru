using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Yomikiru.Controller
{
    public class CheckController : MonoBehaviour
    {
        [SerializeField] private ControllerManager controller;

        public ControllerManager ControllerManager => controller;
        public IObservable<ControllerManager> ChangePlayerDevice => changePlayerDevice;
        private Subject<ControllerManager> changePlayerDevice = new Subject<ControllerManager>();

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

    }
}
