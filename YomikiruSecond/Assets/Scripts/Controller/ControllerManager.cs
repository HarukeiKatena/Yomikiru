using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Yomikiru.Controller
{
    [CreateAssetMenu(menuName = "ScriptableObjects/ControllerManager")]
    public class ControllerManager : ScriptableObject
    {
        public const int MaxPlayerCound = 2;

        public int PlayerCount
        {
            get { return PlayerCount;}
            set => PlayerCount = Math.Min(value, MaxPlayerCound);
        }

        public Gamepad[] PlayerDevices = new Gamepad[MaxPlayerCound];

        public const int NotUsedKeybord = -1; //未使用時の値

        //キーボードを利用してるプレイヤーインデックス(使わない場合-1)
        public int KeybordPlayerIndex = NotUsedKeybord;

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