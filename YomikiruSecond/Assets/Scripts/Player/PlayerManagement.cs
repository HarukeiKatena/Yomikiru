using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Cinemachine;
using Yomikiru.Enemy;
using Player;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using Yomikiru.Effect;
using Yomikiru.Sound;

public class PlayerManagement : MonoBehaviour
{
    [Header("カメラ")]
    [Tooltip("カメラプレハブ")]
    public GameObject CameraPrefub;

    [Header("プレイヤー")]
    [Tooltip("プレイヤープレハブ")]
    public GameObject PlayerPrefub;
    [Tooltip("開始位置")]
    public Transform[] StartPosition;

    [Header("敵")]
    public GameObject EnemyPregub;

    [Header("設定")]
    public IntroSequence Intro;
    public SoundManager soundManager;
    public EffectManager effectManager;

    //キャラクター情報
    [HideInInspector]
    private GameObject[] _character;
    private GameObject[] _camera;

    public GameObject[] CharacterCamera
    {
        get { return _camera; }
    }

    public GameObject[] Character
    {
        get { return _character; }
    }

    void Awake()
    {
        var pad = Gamepad.all;

        _character = new GameObject[2];
        _camera = new GameObject[ControllerManagement.PlayerCount];

        for (int i = 0; i < ControllerManagement.PlayerCount; i++)
        {
            //生成下データ一時保持
            PlayerInput input = null;

            //キーボードチェック
            if (ControllerManagement.KeybordPlayerIndex == i)
            {
                input = CreateKeybordMouse();
            }
            else
            {
                //ゲームパッドがある場合
                if (ControllerManagement.PlayerDevice[i] != null)
                {
                    input = CreatePlayerInput(ControllerManagement.PlayerDevice[i]);
                }
                else
                {
                    //パッドが割り当てられて無い場合使われてないやつをセットする
                    Gamepad spaceGamepad = null;
                    foreach (var gamepad in pad)
                    {
                        //使われてるかチェックする
                        int count = 0; //使われてる数
                        foreach (var player in ControllerManagement.PlayerDevice)
                            if (gamepad == player)
                                count++;

                        //使われてない場合ここに入れる
                        if (count == 0)
                        {
                            spaceGamepad = gamepad;
                            ControllerManagement.PlayerDevice[i] = gamepad;
                            break;
                        }
                    }

                    if (!(spaceGamepad is null))//パッドで作成する
                    {
                        input = CreatePlayerInput(spaceGamepad);
                    }
                    else if (ControllerManagement.KeybordPlayerIndex == -1)//キーボードで作成する
                    {
                        input = CreateKeybordMouse();
                        ControllerManagement.KeybordPlayerIndex = i;
                    }
                    else//空の状態で作成する
                    {
                        input = PlayerInput.Instantiate(PlayerPrefub);
                        input.user.UnpairDevices();
                    }
                }
            }

            //何もしない
            if(input is null)
                continue;

            //情報セット
            _character[i] = input.gameObject;

            //座標セット
            _character[i].transform.position = StartPosition[i].position;
            _character[i].transform.rotation = StartPosition[i].rotation;

            //FirstPersonControllerに情報送信
            TransmissionPlayerData(_character[i], i, ControllerManagement.KeybordPlayerIndex == i);

            //カメラ生成
            _camera[i] = Instantiate(CameraPrefub);
            TransmissionCameraData(_camera[i], _character[i], i);
        }

        //一人プレイの場合
        if (ControllerManagement.PlayerCount == 1)
        {
            GameObject enemy = Instantiate(EnemyPregub, StartPosition[1].position, StartPosition[1].rotation);

            //セット
            _character[1] = enemy;

            //必要なものをセット
            var aiEnemyMove = enemy.GetComponent<AIEnemyMove>();
            var aiEnemyAttack = enemy.GetComponent<AIEnemyAttack>();
            aiEnemyMove.PlayerManager = this;
            aiEnemyMove.EffectManager = this.GetComponent<EffectManager>();
            aiEnemyAttack.EffectManager = this.GetComponent<EffectManager>();

            enemy.GetComponent<AIEnemyBase>().DieEvent.Subscribe(_ =>
            {
                Character[1] = null;
            }).AddTo(this);
        }
    }

    //設定されてるゲームパッドの情報を送信する
    private void TransmissionPlayerData(GameObject playerCharacter, int playerindex, bool EnableKeybordMouse)
    {
        //プレイヤー情報取得
        var player = playerCharacter.GetComponent<Player.PlayerPropSetting>();

        //必要な情報を送る
        player.PlayerIndex = playerindex;

        player.EnableKeybodeMouse = EnableKeybordMouse;
        player.playerMana = this;
        player.intro = Intro;
        player.Sound = soundManager;
        player.effectNamager = effectManager;

        //キャラクターコントローラーを一時的に動かなくする
        playerCharacter.GetComponent<CharacterController>().enabled = false;

        //死亡時の処理
        playerCharacter.GetComponent<Die>().DieIvent.Subscribe(x =>
        {
            Character[x] = null;
        }).AddTo(this);
    }

    //カメラ情報セット
    private void TransmissionCameraData(GameObject cameraobj, GameObject player, int playerindex)
    {
        int LayerIndex = LayerMask.NameToLayer("P" + (playerindex + 1));

        //CinemachineVirtualCamera貰う
        CinemachineVirtualCamera cinemavc =
            cameraobj.transform.GetChild(0).GetComponent<CinemachineVirtualCamera>();
        cinemavc.gameObject.layer = LayerIndex;//レイヤー変更する

        //CinemachineTargetのオブジェクト探してセットする
        foreach (Transform child in player.transform)
        {
            if (child.CompareTag("CinemachineTarget"))
            {
                cinemavc.Follow = child;
                player.GetComponent<Player.PlayerPropSetting>().CinemachineCameraTarget = child.gameObject;
                break;
            }
        }

        //カメラに必要な情報セットする
        Camera came = cameraobj.transform.GetChild(1).GetComponent<Camera>();
        came.cullingMask ^= 1 << LayerIndex;
        came.depth = (playerindex + 1) % ControllerManagement.PlayerCount;


        switch (ControllerManagement.PlayerCount)
        {
            case 1:
                came.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                break;
            case 2:
                came.rect = playerindex == 0 ? new Rect(0.0f, 0.5f, 1.0f, 0.5f) : new Rect(0.0f, 0.0f, 1.0f, 0.5f);
                break;

            default:
                break;
        }
    }

    PlayerInput CreateKeybordMouse()
    {
        PlayerInput input = PlayerInput.Instantiate(PlayerPrefub, pairWithDevices: new InputDevice[] { Keyboard.current, Mouse.current });
        input.user.UnpairDevices();
        InputUser.PerformPairingWithDevice(Keyboard.current, input.user);
        InputUser.PerformPairingWithDevice(Mouse.current, input.user);
        return input;
    }

    PlayerInput CreatePlayerInput(InputDevice device)
    {
        PlayerInput input = PlayerInput.Instantiate(PlayerPrefub, pairWithDevice: device);
        input.user.UnpairDevices();
        InputUser.PerformPairingWithDevice(device, input.user);
        return input;
    }

    PlayerInput CreatePlayerInput(InputDevice[] devices)
    {
        PlayerInput input = PlayerInput.Instantiate(PlayerPrefub, pairWithDevices: devices);
        input.user.UnpairDevices();
        foreach (var device in devices)
        {
            InputUser.PerformPairingWithDevice(device, input.user);
        }
        return input;
    }

    private IEnumerator StayInputSystem()
    {
        //すべてのデバイス取得
        var gamepads = Gamepad.all;
        var device = InputSystem.devices;

        //すべてのデバイスの入力を止める
        foreach (var gamepad in gamepads)
        {
            InputSystem.DisableDevice(gamepad);
        }
        yield return new WaitForSeconds(5.0f);

        //すべてのデバイスの入力を動かす
        foreach (var gamepad in gamepads)
        {
            InputSystem.EnableDevice(gamepad);
        }
    }

    //敵のオブジェクトを取得する
    public GameObject GetEnemy(int MyPlayerIndex)
    {
        return _character.Where((t, i) => MyPlayerIndex != i).Select(t => t.gameObject).FirstOrDefault();
    }

    //敵のオブジェクトを取得する(リスト)
    public List<GameObject> GetEnemyList(int MyPlayerIndex)
    {
        return _character.Where((t, i) => MyPlayerIndex != i).Select(t => t.gameObject).ToList();
    }

}