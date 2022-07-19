using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Yomikiru.Controller
{
    [CreateAssetMenu(menuName = "ScriptableObjects/ControllerManager")]
    public class ControllerManager : ScriptableObject
    {
        public const int MaxPlayerCount = 2;

        public int PlayerCount = MaxPlayerCount;

        public Gamepad[] PlayerDevices = new Gamepad[MaxPlayerCount];

        public const int NotUsedKeybord = -1; //未使用時の値

        //キーボードを利用してるプレイヤーインデックス(使わない場合-1)
        [HideInInspector] public int KeybordPlayerIndex = NotUsedKeybord;

        //プレイヤー数の設定
        public void SetOnePlayer()
            => PlayerCount = 1;

        public void SetTwoPlayer()
            => PlayerCount = 2;

        // 全てのプレイヤー分だけのパッドまたはキーボードが登録されているか
        public bool IsControllersForAllPlayersSet()
        {
            for (int i = 0; i < PlayerCount; i++)
            {
                var gamepad = PlayerDevices[i];
                if (gamepad == null && i != KeybordPlayerIndex)
                    return false;
            }

            return true;
        }

        //プレイヤーデバイスクリア
        public void ClearPlayerDevice()
        {
            for (int i = 0; i < PlayerDevices.Length; i++)
                PlayerDevices[i] = null;
        }
    }
}