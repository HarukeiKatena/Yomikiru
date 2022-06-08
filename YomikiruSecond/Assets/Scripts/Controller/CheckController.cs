using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CheckController : MonoBehaviour
{
    public struct PlayerDeviceData
    {
        public int playerIndex;
        public Gamepad inputDevice;
        public bool ThisKeybord;

        public PlayerDeviceData(int index, Gamepad device, bool keybord = false)
        {
            playerIndex = index;
            inputDevice = device;
            ThisKeybord = keybord;
        }
    }

    //直前に更新されたデバイスの情報を渡す
    public IObservable<PlayerDeviceData> ChangePlayerDevice => changePlayerDevice;
    public Subject<PlayerDeviceData> changePlayerDevice = new Subject<PlayerDeviceData>();

    // Update is called once per frame
    void Update()
    {
        CheckInputDevice();
        CancelPlayerDevice();
    }

    void CheckInputDevice()
    {
        var playerDevice = ControllerManagement.PlayerDevice;
        ref var keybordPlayerIndex = ref ControllerManagement.KeybordPlayerIndex;

        //ゲームパッドで右のボタンを入力された場合
        if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            for (int i = 0; i < playerDevice.Length; i++)
            {
                if(playerDevice[i] == Gamepad.current)
                    break;

                if (keybordPlayerIndex == i || !(playerDevice[i] is null))
                    continue;

                playerDevice[i] = Gamepad.current;
                changePlayerDevice.OnNext(new PlayerDeviceData(i, playerDevice[i]));//イベント通知
                break;
            }
        }

        //キーボードでエンターが押された場合
        if (Keyboard.current != null && Mouse.current != null &&
            Keyboard.current.enterKey.wasPressedThisFrame)
        {
            for (int i = 0; i < playerDevice.Length; i++)
            {
                //未登録の場所にセットする
                if (playerDevice[i] != null)
                    continue;

                keybordPlayerIndex = i;
                changePlayerDevice.OnNext(new PlayerDeviceData(i, playerDevice[i], true));
                break;
            }
        }
    }

    void CancelPlayerDevice()
    {
        var playerDevice = ControllerManagement.PlayerDevice;
        ref var keybordPlayerIndex = ref ControllerManagement.KeybordPlayerIndex;

        for (int i = 0; i < playerDevice.Length; i++)
        {
            //パッド
            if (playerDevice[i] != null && playerDevice[i].buttonEast.wasPressedThisFrame)
            {
                playerDevice[i] = null;
                changePlayerDevice.OnNext(new PlayerDeviceData(i, playerDevice[i]));//イベント通知
            }

            //キーボード
            if (keybordPlayerIndex == i &&
                Keyboard.current != null &&
                Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                keybordPlayerIndex = -1;
                changePlayerDevice.OnNext(new PlayerDeviceData(i, playerDevice[i], false));
            }
        }
    }
}
