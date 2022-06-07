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

    //���O�ɍX�V���ꂽ�f�o�C�X�̏���n��
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

        //�Q�[���p�b�h�ŉE�̃{�^������͂��ꂽ�ꍇ
        if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            for (int i = 0; i < playerDevice.Length; i++)
            {
                if(playerDevice[i] == Gamepad.current)
                    break;

                if (keybordPlayerIndex == i || !(playerDevice[i] is null))
                    continue;

                playerDevice[i] = Gamepad.current;
                changePlayerDevice.OnNext(new PlayerDeviceData(i, playerDevice[i]));//�C�x���g�ʒm
                break;
            }
        }

        //�L�[�{�[�h�ŃG���^�[�������ꂽ�ꍇ
        if (Keyboard.current != null && Mouse.current != null &&
            Keyboard.current.enterKey.wasPressedThisFrame)
        {
            for (int i = 0; i < playerDevice.Length; i++)
            {
                //���o�^�̏ꏊ�ɃZ�b�g����
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
            //�p�b�h
            if (playerDevice[i] != null && playerDevice[i].buttonEast.wasPressedThisFrame)
            {
                playerDevice[i] = null;
                changePlayerDevice.OnNext(new PlayerDeviceData(i, playerDevice[i]));//�C�x���g�ʒm
            }

            //�L�[�{�[�h
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
