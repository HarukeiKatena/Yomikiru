using System;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerManagement : MonoBehaviour
{
    //プレイヤー数
    public static int PlayerCount = 2;

    //プレイヤーごとのパッド情報を入れてるやつ
    public static Gamepad[] PlayerDevice = new Gamepad[2];

    //キーボードを利用してるプレイヤーインデックス(使わない場合-1)
    public static int KeybordPlayerIndex = -1;


    public static void SetOnePlayer()
    {
        PlayerCount = 1;
    }

    public static void SetTwoPlayer()
    {
        PlayerCount = 2;
    }

    // 全てのプレイヤー分だけのパッドまたはキーボードが登録されているか
    public static bool IsControllersForAllPlayersSet()
    {
        for (int i = 0; i < PlayerCount; i++)
        {
            var gamepad = PlayerDevice[i];
            if (gamepad == null && i != KeybordPlayerIndex) return false;
        }
        return true;
    }

    //プレイヤーデバイスクリア
    public static void ClearPlayerDevice()
    {
        for (int i = 0; i < PlayerDevice.Length; i++)
        {
            PlayerDevice[i] = null;
        }
    }
}
